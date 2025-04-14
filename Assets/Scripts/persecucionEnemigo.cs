using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class persecucionEnemigo : MonoBehaviour
{
    [SerializeField] float coordenadaXrespawn = 24f;
    [SerializeField] float coordenadaZrespawn = 24f;
    [SerializeField] float velocidad = 3f;
    [SerializeField] float distanciaPerseguir = 15f;
    [SerializeField] float distanciaAtaque = 0.05f;

    public GameObject jugador;

    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        float distancia = Vector3.Distance(transform.position, jugador.transform.position);

        if (distancia <= distanciaAtaque)
        {
            // ataca
            animator.SetBool("Perseguir", false);
            animator.SetBool("Ataque", true);
            // .....
        }
        else if (distancia <= distanciaPerseguir)
        {
            // persigue
            

           
            Vector3 posicionJugador = new Vector3(jugador.transform.position.x, transform.position.y, jugador.transform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, posicionJugador, velocidad * Time.deltaTime);
            Vector3 direccionMovimiento = (posicionJugador - transform.position).normalized; //para la rotacion de enemigo
            transform.rotation = Quaternion.LookRotation(direccionMovimiento); //cambia la rotacion del enemigo 
            animator.SetBool("Perseguir", true);
            animator.SetBool("Ataque", false);
        }
        else
        {
            // idle
            animator.SetBool("Perseguir", false);
            animator.SetBool("Ataque", false);
        }
    }

    public void respawnear()
    {
        Vector3 respawn = new Vector3(Random.Range(-coordenadaXrespawn, coordenadaXrespawn), 0.5f, Random.Range(-coordenadaZrespawn, coordenadaZrespawn));
        transform.position = respawn;
        GetComponent<Renderer>().material.color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        velocidad = velocidad * 1.3f;
    }
}