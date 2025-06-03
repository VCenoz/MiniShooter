using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;

public class controllerConAnimaciones : MonoBehaviour
{
    public GameObject finPartida;
    [SerializeField] float velocidadSprint = 2;
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

    Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();

        camara = Camera.main;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        groundedPlayer = controller.isGrounded;


        animator.SetFloat("Walking", Input.GetAxis("Vertical"));

        Vector3 move = Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward;
        controller.Move(move * Time.deltaTime * playerSpeed);

        transform.Rotate(0, Input.GetAxis("Horizontal"), 0);


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
            animator.SetBool("Running", true);
            playerSpeed = velocidadSprint;
        }
        else
        {
            playerSpeed = 2.0f;
            animator.SetBool("Running", false);
        }

    }

}