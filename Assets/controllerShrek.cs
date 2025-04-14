using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;
using UnityEngine.UI;

public class controllerShrek : MonoBehaviour
{
    public GameObject finPartida;
    public Image staminaBarra;
    private Coroutine recargar;
    [SerializeField] float stamina, maxStamina, costeSprint, tasaRecargo;
    [SerializeField] float sensibilidadRaton = 2.0F;
    private float giroHorizontal, giroVertical, maxGradosGiro = 30;
    public Camera camara;
    [SerializeField]  Vector3[] posicionesCamara;
    int indicePosicionCamara;

    public CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private float playerSpeed = 2.0f, jumpHeight = 10.0f, gravityValue = -20.81f, pushPower = 2.0F, velocidadSprint = 15;

    Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();

        posicionesCamara = new Vector3[] { new Vector3(0f, 0.91f, -3.4f), new Vector3(0.01f, 0.91f, 0.32f), new Vector3(0.01f, 2.91f, -8.01f) };
        indicePosicionCamara = 0;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        DisparoRayCast();

        groundedPlayer = controller.isGrounded;

        animator.SetFloat("Walking", Input.GetAxis("Vertical"));
        animator.SetFloat("Movimiento Frontal", Input.GetAxis("Vertical") * Time.deltaTime * playerSpeed);
        animator.SetFloat("Movimiento Lateral", Input.GetAxis("Horizontal") * Time.deltaTime * playerSpeed);

        Vector3 move = Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward;
        controller.Move(move * Time.deltaTime * playerSpeed);

        transform.Rotate(0, Input.GetAxis("Horizontal"), 0) ;

        if (groundedPlayer && playerVelocity.y < 0)
        {
            animator.SetBool("Jumping", false);
            animator.SetBool("Falling", true);
            playerVelocity.y = -0.5f;
            animator.SetBool("Falling", false);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        // Makes the player jump
        if (Input.GetKeyDown(KeyCode.Space) && groundedPlayer)
        {
            animator.SetBool("Jumping", true);
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -2.0f * gravityValue);

        }

        if (Input.GetKey("left shift"))
        {
            if (stamina > 0)
            {
                animator.SetBool("Running", true);
                playerSpeed = velocidadSprint;
                stamina -= costeSprint * Time.deltaTime;
                if (stamina < 0) stamina = 0;
                staminaBarra.fillAmount = stamina / maxStamina;

                if (recargar != null) StopCoroutine(recargar);
                recargar = StartCoroutine(RecargarStamina());
            }
            else
            {
                playerSpeed = 2.0f;
                animator.SetBool("Running", false);
            }
        }
        else
        {
            playerSpeed = 2.0f;
            animator.SetBool("Running", false);
        }

        if (Input.GetKeyDown("c"))
        {
            camara.transform.localPosition = posicionesCamara[indicePosicionCamara];
            if (indicePosicionCamara >= posicionesCamara.Length - 1)
            {
                indicePosicionCamara = 0;
            }
            else
            {
                indicePosicionCamara++;
            }
        }

        if (Input.GetKey("v"))
        {
            animator.SetBool("Crouching", true);


        }
    }





    private void DisparoRayCast()
    {
        if (Input.GetMouseButton(0))
        {

            Ray r = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            Debug.DrawRay(r.origin, r.direction * 100, Color.magenta);
            RaycastHit hitInfo;

            if (Physics.Raycast(r, out hitInfo))
            {
                //Debug.Log(hitInfo.collider.gameObject.name);
                if (hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("Enemigo"))
                {

                    hitInfo.collider.gameObject.GetComponent<persecucionEnemigo>().respawnear();
                }

                //Debug.Log(hitInfo.collider.gameObject.name);
                if (hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("Disparable"))
                {
                    Rigidbody body = hitInfo.collider.attachedRigidbody;
                    if (body == null || body.isKinematic)
                        return;

                    Vector3 direction = body.transform.position - transform.position;
                    body.AddForceAtPosition(direction.normalized * 5f, hitInfo.point);

                }


            }
        }
    }


    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Disparable"))
        {
            Rigidbody body = hit.collider.attachedRigidbody;
            if (body == null || body.isKinematic)
                return;

            if (hit.moveDirection.y < -0.3F)
                return;

            Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
            //body.velocity = pushDir * pushPower;

            body.AddForce(pushDir * pushPower, ForceMode.Impulse);
        }

        if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Teletransporte"))
        {
            transform.position = Vector3.zero;
        }

    }

    private IEnumerator RecargarStamina()
    {
        yield return new WaitForSeconds(1f);
        while (stamina < maxStamina)
        {
            stamina += tasaRecargo / 10f;
            if (stamina > maxStamina) stamina = maxStamina;
            staminaBarra.fillAmount = stamina / maxStamina;
            yield return new WaitForSeconds(1f);
        }
    }

}