using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class mecanicasJugador : MonoBehaviour
{

    public GameObject finPartida;
    public float velocidad = 5;
    public float fuerzaSalto = 5;

    [SerializeField] float sensibilidadRaton = 2.0F;

    private float giroVertical;
    private float giroHorizontal;

    private Camera camara;
    Rigidbody rb;


    [SerializeField] float maxGradosGiro = 30;

    void Start()
    {
        camara = Camera.main;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false; 
        rb = GetComponent<Rigidbody>();

    }

    // Update is called once per frame
    void Update()
    {
        DisparoRayCast();
        RotacionPersonaje();
        ControlSalto();
    }
    private void FixedUpdate()
    {
        float adelante = Input.GetAxis("Vertical") * velocidad;
        float lateral = Input.GetAxis("Horizontal") * velocidad;
        rb.velocity = transform.rotation * new Vector3(lateral, rb.velocity.y, adelante);
    }

    private void ControlSalto()
    {
        if (Input.GetKeyDown("space"))
        {
            rb.AddForce(Vector3.up * fuerzaSalto, ForceMode.Impulse);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Pared") || collision.collider.gameObject.layer == LayerMask.NameToLayer("Enemigo"))
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            finPartida.SetActive(true);
            Time.timeScale = 0;
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

}


