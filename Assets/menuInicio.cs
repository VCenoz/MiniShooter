using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class menuInicio : MonoBehaviour
{
    public GameObject panelMenuInicio;
    public GameObject panelMenuOpciones;
    public GameObject panelMenuVolumen;
    public GameObject panelMenuControles;

    public Slider volumenSlider;
    public GameObject audioSources;


    public InputActionAsset inputActions; //archivo inputActions
    private List<InputAction> Actions = new List<InputAction>(); //array con todas las Action de mi inputActions
    InputActionRebindingExtensions.RebindingOperation rebindingOperation;

    public TextMeshProUGUI labelCamara;
    public TextMeshProUGUI labelAgacharse;
    public TextMeshProUGUI labelSaltar;
    public TextMeshProUGUI labelSprint;
    public TextMeshProUGUI labelAdelante;
    public TextMeshProUGUI labelAtras;
    public TextMeshProUGUI labelIzquierda;
    public TextMeshProUGUI labelDerecha;


    void Start() 
    {   //inicializo el array
        Actions = new List<InputAction>();

        //relleno el array de Actions
        foreach (var map in inputActions.actionMaps) //por cada map
        {
            foreach (var action in map.actions) //relleno los Actions
            {
                Actions.Add(action);
            }
        }

        //al comenzar el juego, cargo el JSON guardado en el PLayerPrefs con los cambios que hizo el usuario, para cada Action
        foreach (var action in Actions)
        {
            string saved = PlayerPrefs.GetString("rebinds_" + action.name, string.Empty);
            if (!string.IsNullOrEmpty(saved))
                action.LoadBindingOverridesFromJson(saved);
        }

        //actualizo tambien los labels a mano
        labelCamara.text = Actions.Find(a => a.name == "CambiarCamara").GetBindingDisplayString(0).ToUpper();
        labelAgacharse.text = Actions.Find(a => a.name == "Crouch").GetBindingDisplayString(0).ToUpper();
        labelSaltar.text = Actions.Find(a => a.name == "Saltar").GetBindingDisplayString(0).ToUpper();
        labelSprint.text = Actions.Find(a => a.name == "Sprint").GetBindingDisplayString(0).ToUpper();

        InputAction mover = Actions.Find(a => a.name == "Moverse");
        labelAdelante.text = mover.GetBindingDisplayString(conseguirIndexMovimiento(mover, "up")).ToUpper();
        labelAtras.text = mover.GetBindingDisplayString(conseguirIndexMovimiento(mover, "down")).ToUpper();
        labelIzquierda.text = mover.GetBindingDisplayString(conseguirIndexMovimiento(mover, "left")).ToUpper();
        labelDerecha.text = mover.GetBindingDisplayString(conseguirIndexMovimiento(mover, "right")).ToUpper();


    }

    void Update()
    {
        
    }

    public void Jugar()
    {
        SceneManager.LoadScene("MiniShooter", LoadSceneMode.Single);
    }

    public void ShowMenuOpciones()
    {
        panelMenuOpciones.SetActive(true);
    }
    public void VolverOpciones()
    {
        panelMenuOpciones.SetActive(false);
    }

    public void ShowMenuVolumen()
    {
        panelMenuVolumen.SetActive(true);
    }
    public void CambiarVolumenSlider() //llamo a este metodo en el evento de cambio de valor del slider (editor)
    {
        //utilizo la clase estatica para guardar el nuevo valor
        OpcionesUsuario.volumen = volumenSlider.value;
        OpcionesUsuario.GuardarEnPrefs();
        OpcionesUsuario.AplicarVolumenAlMenu(audioSources);
    }
    public void VolverVolumen() 
    {
        panelMenuVolumen.SetActive(false);
    } 

    public void ShowMenuControles()
    {
        panelMenuControles.SetActive(true);
    }
    public void VolverControles()
    {
        panelMenuControles.SetActive(false);
    }

    public void Salir()
    {
        Application.Quit();
    }

    public void EditarInputActions(InputAction action, int bindingIndex, TextMeshProUGUI label)
    {
        //comprobar si el binding es composite (teclas de movimiento)
        if (action.bindings[bindingIndex].isComposite)
        {
            return;
        }

        action.Disable(); //se inhabilita para poder realizar cambios
        
        rebindingOperation = action.PerformInteractiveRebinding(bindingIndex)//se le pasa el indice del binding que queremos editar para este Action
            .WithControlsExcluding("Mouse") // evitar usar clicks de mouse como input
            .WithCancelingThrough("<Keyboard>/escape") // permite al usuario cancelar con esc
            .OnMatchWaitForAnother(0.2f) //espera 2 segundos antes de asignar por si se ha presionado una tecla por accidente
            .OnComplete(operation => //se llama cuando el usuario presiona una tecla valida y se completa el rebinding
            {
                label.text = action.GetBindingDisplayString(bindingIndex).ToUpper(); //asigna al label la nueva tecla, en mayuscula

                //hago que los cambios se guarden en PlayerPrefs como JSON (no se puede editat archivo inputactions mas que en el editor)
                string rebinds = action.SaveBindingOverridesAsJson();
                PlayerPrefs.SetString("rebinds_" + action.name, rebinds);
                PlayerPrefs.Save();

                Debug.Log($"Rebinding complete! {action.name}[{bindingIndex}] = {action.bindings[bindingIndex].effectivePath}");
                action.Enable(); // Re-enable the action
                operation.Dispose(); // Clean up
            })
            .OnCancel(operation => //se llama cuando el usuario cancela la operacion (esc)
            {
                Debug.Log("Rebinding cancelado por el usuario.");
                action.Enable();
                operation.Dispose();
            })
            .Start(); //comienza la operacion de rebinding
    }

    public void CambiarBotonCamara()
    {
        InputAction action = Actions.Find(a => a.name == "CambiarCamara");
        EditarInputActions(action, 0, labelCamara);
    }
    public void CambiarBotonAgacharse()
    {
        InputAction action = Actions.Find(a => a.name == "Crouch");
        EditarInputActions(action, 0, labelAgacharse);
    }
    public void CambiarBotonSaltar()
    {
        InputAction action = Actions.Find(a => a.name == "Saltar");
        EditarInputActions(action, 0, labelSaltar);
    }
    public void CambiarBotonSprint()
    {
        InputAction action = Actions.Find(a => a.name == "Sprint");
        EditarInputActions(action, 0, labelSprint);
    }

    //recorro con loop el Action Movimiento para saber cual es el index del binding que quiero (up, down, left, right). La asignacion por codigo de estos bindings requiren indices
    public int conseguirIndexMovimiento(InputAction action, string name)
    {
        for (int i = 0; i < action.bindings.Count; i++)
        {
            if (action.bindings[i].name == name && action.bindings[i].isPartOfComposite) return i;
        }
        return -1;
    } 
    public void CambiarBotonAdelante()
    {
        InputAction action = Actions.Find(a => a.name == "Moverse"); //del action Moverse
        int index = conseguirIndexMovimiento(action, "up"); // con el indice de "up"
        EditarInputActions(action, index, labelAdelante);

    }
    public void CambiarBotonAtras()
    {
        InputAction action = Actions.Find(a => a.name == "Moverse"); //del action Moverse
        int index = conseguirIndexMovimiento(action, "down"); // con el indice de "up"
        EditarInputActions(action, index, labelAtras);
    }
    public void CambiarBotonIzquierda()
    {
        InputAction action = Actions.Find(a => a.name == "Moverse"); //del action Moverse
        int index = conseguirIndexMovimiento(action, "left"); // con el indice de "up"
        EditarInputActions(action, index, labelIzquierda);
    }
    public void CambiarBotonDerecha()
    {
        InputAction action = Actions.Find(a => a.name == "Moverse"); //del action Moverse
        int index = conseguirIndexMovimiento(action, "right"); // con el indice de "up"
        EditarInputActions(action, index, labelDerecha);
    }
}
