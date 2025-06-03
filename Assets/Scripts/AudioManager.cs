using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        OpcionesUsuario.CargarDesdePrefs();

        foreach ( AudioSource audio in GetComponentsInChildren<AudioSource>())
        {
            audio.volume = OpcionesUsuario.volumen;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
