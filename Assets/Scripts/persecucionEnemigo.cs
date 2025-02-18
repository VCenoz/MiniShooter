using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class persecucionEnemigo : MonoBehaviour
{
    [SerializeField] float coordenadaXrespawn = 24f;
    [SerializeField] float coordenadaZrespawn = 24f;
    [SerializeField] float velocidad = 3f;
    public GameObject jugador;

    void Start()
    {

    }


    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, jugador.transform.position, velocidad * Time.deltaTime);
    }

    public void respawnear()
    {
        Vector3 respawn = new Vector3(Random.Range(-coordenadaXrespawn, coordenadaXrespawn), 0.5f, Random.Range(-coordenadaZrespawn, coordenadaZrespawn));
        transform.position = respawn;
        GetComponent<Renderer>().material.color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        velocidad = velocidad * 1.3f; 
        
    }
}
