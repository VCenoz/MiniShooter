using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;

public class jugadorController : MonoBehaviour
{
    public GameObject finPartida;
    [SerializeField] float velocidadSprint = 15;
    [SerializeField] float sensibilidadRaton = 2.0F;
    private float giroVertical;
    private float giroHorizontal;
    private Camera camara;
    [SerializeField] float maxGradosGiro = 30;

    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private float playerSpeed = 2.0f;
    [SerializeField] float jumpHeight = 10.0f;
    [SerializeField] float gravityValue = -20.81f;

    public float pushPower = 2.0F;

    private void Start()
    {
        camara = Camera.main;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        DisparoRayCast();
        RotacionPersonaje();


        groundedPlayer = controller.isGrounded;


        Vector3 move = Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward;
        controller.Move(move * Time.deltaTime * playerSpeed);


        //Debug.Log(playerVelocity);

        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = -0.5f;
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        // Makes the player jump
        if (Input.GetKeyDown(KeyCode.Space) && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -2.0f * gravityValue);

        }

        if (Input.GetKey("left shift"))
        {
            playerSpeed = velocidadSprint;
        }
        else
        {
            playerSpeed = 2.0f;
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

    private void RotacionPersonaje()
    {
        giroHorizontal = sensibilidadRaton * Input.GetAxis("Mouse X");
        transform.Rotate(0, giroHorizontal, 0);
        giroVertical += sensibilidadRaton * Input.GetAxis("Mouse Y");
        giroVertical = Mathf.Clamp(giroVertical, -maxGradosGiro, maxGradosGiro);
        camara.transform.localRotation = Quaternion.Euler(-giroVertical, 0, 0);
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

}