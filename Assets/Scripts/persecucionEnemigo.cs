using System.Collections;
using UnityEngine;

public class persecucionEnemigo : MonoBehaviour
{
    [SerializeField] AudioSource audioEnemigoHerido;
    [SerializeField] AudioSource audioEnemigoMuere;
    [SerializeField] float coordenadaXrespawn = 24f;
    [SerializeField] float coordenadaZrespawn = 24f;
    [SerializeField] float velocidad = 3f;
    [SerializeField] float distanciaPerseguir = 15f;
    [SerializeField] float distanciaAtaque = 0.5f;
    [SerializeField] float health, maxHealth = 3f;
    [SerializeField] float regeneracion = 1f;
    [SerializeField] BoxCollider triggerEspada;

    public GameObject jugador;
    [SerializeField] floatingHealthbar healthbar;

    Animator animator;
    Rigidbody rb;

    private Coroutine regeneracionActiva;

    private void Awake()
    {
        healthbar = GetComponentInChildren<floatingHealthbar>();
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        health = maxHealth;
        healthbar.UpdateHealthBar(health, maxHealth);
        animator = GetComponent<Animator>();

        // Aseguramos que el Rigidbody no rote ni se descontrole
        rb.freezeRotation = true;
    }

    void FixedUpdate()
    {
        Vector3 posEnemigo = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        Vector3 posJugador = new Vector3(jugador.transform.position.x, transform.position.y, jugador.transform.position.z);
        float distancia = Vector3.Distance(posEnemigo, posJugador);

        if (distancia <= distanciaAtaque)
        {
            // Forzamos un paso más hacia el jugador antes de atacar
            Vector3 direccion = (posJugador - transform.position).normalized;
            direccion.y = 0f; // por si acaso
            rb.MovePosition(rb.position + direccion * velocidad * Time.fixedDeltaTime * 0.5f); // medio paso

            animator.SetBool("Perseguir", false);
            animator.SetBool("Ataque", true);
        }
        else if (distancia <= distanciaPerseguir)
        {
            Vector3 direccionMovimiento = (posJugador - transform.position).normalized;
            direccionMovimiento.y = 0f;

            rb.MovePosition(rb.position + direccionMovimiento * velocidad * Time.fixedDeltaTime);

            if (direccionMovimiento != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(direccionMovimiento);

            animator.SetBool("Perseguir", true);
            animator.SetBool("Ataque", false);
        }
        else
        {
            animator.SetBool("Perseguir", false);
            animator.SetBool("Ataque", false);
        }
    }


    public void respawnear()
    {
        Vector3 respawn = new Vector3(Random.Range(-coordenadaXrespawn, coordenadaXrespawn), 0.5f, Random.Range(-coordenadaZrespawn, coordenadaZrespawn));
        rb.position = respawn; // usamos rb en lugar de transform
        GetComponent<Renderer>().material.color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        velocidad *= 1.3f;
    }

    private IEnumerator MorirYRespawnear()
    {
        audioEnemigoMuere.Play();
        animator.SetBool("Death", true);

        rb.constraints = RigidbodyConstraints.FreezeAll;

        yield return new WaitForSeconds(5f); // tiempo de espera para la animación de muerte

        respawnear();

        rb.constraints = RigidbodyConstraints.FreezeRotation;

        animator.SetBool("Death", false);
        health = maxHealth;
        healthbar.UpdateHealthBar(health, maxHealth);
    }

    public void RecibirDaño(float cantidad)
    {
        audioEnemigoHerido.Play();
        Debug.Log("he recibido daño, se para la corrutina de regeneracion de vida");

        // Detiene la regeneración anterior correctamente si está activa
        if (regeneracionActiva != null)
        {
            StopCoroutine(regeneracionActiva);
            regeneracionActiva = null;
        }

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
            regeneracionActiva = StartCoroutine(RecargarVida());
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