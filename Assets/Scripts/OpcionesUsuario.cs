using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//clase estatica para poder acceder o guardar valores de volumen esde cualquier escena
public static class OpcionesUsuario
{
    public static float volumen = 1f;

    public static void CargarDesdePrefs()
    {
        volumen = PlayerPrefs.GetFloat("volumenGeneral", 1f);
    }

    public static void GuardarEnPrefs()
    {
        //creo la preferencia de usuario de volumen y la guardo
        PlayerPrefs.SetFloat("volumenGeneral", volumen);
        PlayerPrefs.Save();
    }

    public static void AplicarVolumenAlMenu(GameObject audioMenu)
    {
        AudioSource[] sonidos = audioMenu.GetComponentsInChildren<AudioSource>();
        foreach (AudioSource audioSource in sonidos)
        {
            audioSource.volume = volumen;
        }
    }
}

