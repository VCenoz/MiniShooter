using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class controllerShrekNewInput : MonoBehaviour
{
    public GameObject finPartida;
    public Image staminaBarra;
    [SerializeField] float stamina, maxStamina, costeSprint, tasaRecargo;
    [SerializeField] float sensibilidadRaton = 15F;
    private float giroHorizontal, giroVertical, maxGradosGiro = 30;
    public Camera camara;
    [SerializeField] Vector3[] posicionesCamara;
    int indicePosicionCamara;

    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private float playerSpeed = 3.0f, jumpHeight = 10.0f, gravityValue = -20.81f, pushPower = 2.0F, velocidadSprint = 9;
    private Coroutine recargar;
    private Animator animator;
    private InputHandlerShrek input;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        input = GetComponent<InputHandlerShrek>();

        posicionesCamara = new Vector3[]
        {
            new Vector3(0f, 0.91f, -3.4f),
            new Vector3(0.01f, 0.91f, 0.32f),
            new Vector3(0.01f, 2.91f, -8.01f)
        };
        indicePosicionCamara = 0;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
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
            camara.transform.localPosition = posicionesCamara[indicePosicionCamara];
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
            DisparoRayCast();
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

    private void DisparoRayCast()
    {
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
}
