using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class teletransportador : MonoBehaviour
{
    [SerializeField ]GameObject jugador;
    [SerializeField] GameObject teleporter1;
    [SerializeField] GameObject teleporter2;
    [SerializeField] GameObject teleporter3;
    [SerializeField] GameObject teleporter4;



    GameObject[] teleporterArray;
    // Start is called before the first frame update
    void Start()
    {
        teleporterArray = new GameObject[] { teleporter1, teleporter2, teleporter3, teleporter4};
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")){
            other.gameObject.GetComponent<CharacterController>().enabled = false;
            int random;

            do
            {
                random = Random.Range(0, teleporterArray.Length);
                Debug.Log(transform.gameObject);
                Debug.Log(teleporterArray[random].gameObject);
            }
            while (transform.gameObject == teleporterArray[random].gameObject);

            other.transform.position = teleporterArray[random].transform.position + transform.forward;
            other.transform.rotation = teleporterArray[random].transform.rotation;
            other.gameObject.GetComponent<CharacterController>().enabled = true;


        }
    
    }
}
