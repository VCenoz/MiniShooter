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
    [SerializeField] float health, maxHealth = 3f;
    [SerializeField] float regeneracion = 1f;

    public GameObject jugador;
    [SerializeField] floatingHealthbar healthbar; //consigo una referencia al script para manejar la barra de vida
    Animator animator;

    private void Awake()
    {
        healthbar = GetComponentInChildren<floatingHealthbar>();
    }

    void Start()
    {
        health = maxHealth;
        healthbar.UpdateHealthBar(health, maxHealth); 
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

    private IEnumerator MorirYRespawnear()
    {
        animator.SetBool("Death", true);
        yield return new WaitForSeconds(5f); // Espera 2 segundos para que termine la animación

        respawnear();

        animator.SetBool("Death", false);
        health = maxHealth;//volvemos la vida a full
        healthbar.UpdateHealthBar(health, maxHealth);//volvemos a full la barra tambien
    }

    public void RecibirDaño(float cantidad)
    {
        Debug.Log("he recibido daño, se para la corrutina de regeneracion de vida");
        StopCoroutine(RecargarVida());
        Debug.Log("vida antes: " + health);
        health -= cantidad;
        Debug.Log("vida ahora: " + health);
        if (health < 0) health = 0;
        healthbar.UpdateHealthBar(health, maxHealth);

        if (health <= 0)
        {
            Debug.Log("vida es 0, muero");
            StartCoroutine(MorirYRespawnear());
        }
        else
        {
            Debug.Log("herido pero vida no es cero, comienzo la regeneracion");
            StartCoroutine(RecargarVida());
        }
    }

    private IEnumerator RecargarVida()
    {
        Debug.Log("comienza corrutina, espero 2 segundos antes de regenerar");
        yield return new WaitForSeconds(2f);

        while (health < maxHealth)
        {
            Debug.Log("entro en bucle, la vida aun no se ha llenado");
            Debug.Log("vida antes: " + health);
            health += regeneracion;
            Debug.Log("vida ahora: " + health);
            if (health > maxHealth) health = maxHealth;
            healthbar.UpdateHealthBar(health, maxHealth);
            yield return new WaitForSeconds(2f);
        }

        Debug.Log("termina bucle, la vida se ha llenado, termina la corrutina");
    }

}