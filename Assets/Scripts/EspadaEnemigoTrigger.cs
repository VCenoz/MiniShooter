using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EspadaEnemigoTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Golpe al jugador desde la espada");

            other.gameObject.GetComponent<controllerShrekNewInput>().RecibirDaño(3f);
        }
    }
}
