using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Configuraciones : MonoBehaviour
{
    [SerializeField] AudioSource audioReload;
    private bool puedeDisparar;
    public int balasTotales = 500;
    public int balas = 10;
    public float disparosPorSegundo = 3f;
    public float tiempoRecarga = 2f;
    private Coroutine recargar;
    private bool recargando = false;

    public TextMeshProUGUI balasTexto;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        balasTexto.text = balasTotales + " : " + balas;

        if (balas <= 0)
        {
            StartCoroutine(Recargar()); 
        }
    }


    IEnumerator Recargar()
    {
        if (balasTotales > 0 && !recargando)
        {
            recargando = true;
            audioReload.Play();
            Debug.Log("recargando...");
            yield return new WaitForSeconds(tiempoRecarga);

            balasTotales -= 10;
            balas = 10;
            Debug.Log("comenzando recarga");
            balasTexto.text = "Balas: " + balas;
            recargando = false;


        }
    }
}
