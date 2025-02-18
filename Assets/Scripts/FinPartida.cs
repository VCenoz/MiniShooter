using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinPartida : MonoBehaviour
{

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Jugar()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1;

    }
    public void Salir()
    {
        Debug.Log("salir");
        Application.Quit();
    }
}
