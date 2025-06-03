using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class controllerShrekNewInput : MonoBehaviour
{
    public GameObject finPartida;
    [SerializeField] AudioSource audioShrek;
    [SerializeField] AudioSource audioShoot;
    public Image staminaBarra;
    public Configuraciones configuraciones;
    [SerializeField] float stamina, maxStamina, costeSprint, tasaRecargo;
    [SerializeField] float sensibilidadRaton = 15F;
    private float giroHorizontal, giroVertical, maxGradosGiro = 30;
    [SerializeField] float health, maxHealth = 3f;
    public Camera camara;
    [SerializeField] Transform[] posicionesCamara;
    int indicePosicionCamara;

    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private float playerSpeed = 3.0f, jumpHeight = 4.0f, gravityValue = -20.81f, pushPower = 2.0F, velocidadSprint = 9;
    private Coroutine recargar;
    private Animator animator;
    private InputHandlerShrek input;
    private bool estaMuerto = false;


    private void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        input = GetComponent<InputHandlerShrek>();

        indicePosicionCamara = 0;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (estaMuerto) return; //si esta muerto no hace nada

        RotacionPersonaje();

        groundedPlayer = controller.isGrounded;

        animator.SetFloat("Movimiento Frontal", input.movimiento.y * playerSpeed);
        animator.SetFloat("Movimiento Lateral", input.movimiento.x * playerSpeed);

        Vector3 move = input.movimiento.x * transform.right + input.movimiento.y * transform.forward;
        controller.Move(move * Time.deltaTime * playerSpeed);

        if (groundedPlayer && playerVelocity.y < 0)
        {
            animator.SetBool("Jumping", false);
            animator.SetBool("Falling", false);
            playerVelocity.y = -0.5f;
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        if (input.saltar && groundedPlayer)
        {
            animator.SetBool("Jumping", true);
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2.0f * gravityValue);
        }

        if (input.sprint && stamina > 0)
        {
            animator.SetBool("Running", true);
            playerSpeed = velocidadSprint;
            stamina -= costeSprint * Time.deltaTime;
            stamina = Mathf.Max(0, stamina);
            staminaBarra.fillAmount = stamina / maxStamina;

            if (recargar != null) StopCoroutine(recargar);
            recargar = StartCoroutine(RecargarStamina());
        }
        else
        {
            animator.SetBool("Running", false);
            playerSpeed = 2.0f;
        }

        if (input.cambiarCamara)
        {
            camara.transform.localPosition = posicionesCamara[indicePosicionCamara].localPosition;
            indicePosicionCamara = (indicePosicionCamara + 1) % posicionesCamara.Length;
        }

        if (input.crouch)
        {
            animator.SetBool("Crouching", true);
            controller.height = 1.6f;
            controller.center = new Vector3(0f, 0.97f, 0f);
        }
        else if (animator.GetBool("Crouching") && mePuedoLevantar())
        {
            animator.SetBool("Crouching", false);
            controller.height = 2.51f;
            controller.center = new Vector3(0f, 1.39f, 0f);
        }

        if (Input.GetMouseButtonDown(0)) //inputAction WAS PRESSED THIS FRAME
        {
             StartCoroutine(DisparoRayCast());
        }

        input.ResetFrameInputs();
    }

    private void RotacionPersonaje()
    {
        giroHorizontal = sensibilidadRaton * Mouse.current.delta.ReadValue().x; 
        transform.Rotate(0, giroHorizontal, 0);

        giroVertical += sensibilidadRaton * Mouse.current.delta.ReadValue().y;
        giroVertical = Mathf.Clamp(giroVertical, -maxGradosGiro, maxGradosGiro);
        camara.transform.localRotation = Quaternion.Euler(-giroVertical, 0, 0);
    }

    private bool mePuedoLevantar()
    {
        float distanciaMinima = 1f;
        Vector3 origen = transform.position + Vector3.up * controller.height / 2f;
        return !Physics.Raycast(origen, Vector3.up, distanciaMinima);
    }

    private IEnumerator DisparoRayCast()
    {
        if (configuraciones.balas > 0)
        {
            audioShoot.Play();
            configuraciones.balas -= 1; //gasto una bala

            Ray r = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            Debug.DrawRay(r.origin, r.direction * 100, Color.magenta);
            if (Physics.Raycast(r, out RaycastHit hitInfo))
            {
                if (hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("Enemigo"))
                {
                    hitInfo.collider.gameObject.GetComponent<persecucionEnemigo>().RecibirDaño(1f);
                }
                else if (hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("Disparable"))
                {
                    Rigidbody body = hitInfo.collider.attachedRigidbody;
                    if (body != null && !body.isKinematic)
                    {
                        Vector3 direction = body.transform.position - transform.position;
                        body.AddForceAtPosition(direction.normalized * 5f, hitInfo.point);
                    }
                }
            }
        }
        
        else if (configuraciones.balas <= 0)
        {
            yield return new WaitForSeconds(configuraciones.tiempoRecarga);
        }
        

    }

    private IEnumerator RecargarStamina()
    {
        yield return new WaitForSeconds(1f);
        while (stamina < maxStamina)
        {
            stamina += tasaRecargo / 10f;
            stamina = Mathf.Min(stamina, maxStamina);
            staminaBarra.fillAmount = stamina / maxStamina;
            yield return new WaitForSeconds(1f);
        }
    }

    public void RecibirDaño(float cantidad)
    {
        audioShrek.Play();
        Debug.Log("vida antes: " + health);
        health -= cantidad;
        Debug.Log("vida ahora: " + health);
        if (health < 0) health = 0;

        if (health <= 0)
        {
            Debug.Log("vida es 0, muero");
            StartCoroutine(Morir());
        }
    }
    private IEnumerator Morir()
    {
        estaMuerto = true; // bloqueamos el movimiento en el updatw
        animator.SetBool("Death", true);
        yield return new WaitForSeconds(5f); // Espera 2 segundos para que termine la animación
        animator.SetBool("Death", false);

        //panel fin partida
        finPartida.SetActive(true); //activo el panel para que se muestre sobre la pantalla
        Cursor.lockState = CursorLockMode.Confined; //cambio el cursor de locked al centro de la pantalla a confined a toda la ventana, para poder pulsar botones
        Cursor.visible = true; //vuelvo a hacer visible el cursor

    }

    public void Reintentar()
    {
        SceneManager.LoadScene("MiniShooter", LoadSceneMode.Single);
    }

    public void MenuPrincipal()
    {
        SceneManager.LoadScene("menuPrincipal", LoadSceneMode.Single);
    }
}
