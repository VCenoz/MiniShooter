using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandlerShrek : MonoBehaviour
{
    [HideInInspector] public Vector2 movimiento;
    [HideInInspector] public Vector2 mirar;

    public bool saltar;
    public bool sprint;
    public bool crouch;
    public bool disparar;
    public bool cambiarCamara;
    private ShrekInputActions acciones;

    private void Awake()
    {
        acciones = new ShrekInputActions();
    }
    private void OnEnable()
    {
        acciones.Enable();
    }

    private void OnDisable()
    {
        acciones.Disable();
    }
    public void OnMoverse(InputAction.CallbackContext context)
    {
        movimiento = context.ReadValue<Vector2>();
        //Debug.Log(movimiento);
    }

    public void OnMirar(InputAction.CallbackContext context) 
    {
        mirar = context.ReadValue<Vector2>();
        //Debug.Log(Mouse.current.position);
        //acciones.Jugador.Mirar.performed += ctx => Debug.Log( ctx.ReadValue<Vector2>() );
        //acciones.Jugador.Mirar.performed += ctx => Debug.Log("asdfasldkfjalsdkfj");

        //Debug.Log(mirar);

    }

    public void OnSaltar(InputAction.CallbackContext context)
    {
        if (context.performed)
            saltar = true;
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        sprint = context.ReadValueAsButton();
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        crouch = context.ReadValueAsButton();
    }

    public void OnCambiarCamara(InputAction.CallbackContext context)
    {
        if (context.performed)
            cambiarCamara = true;
    }

    public void OnDisparo(InputAction.CallbackContext context)
    {
        Debug.Log("ASDFASDFAS");
        if (context.performed)
            disparar = true;
    }

  //  public void OnMiraCrouch(InputAction.CallbackContext context)
  //  {
  //      miraCrouch = context.ReadValueAsButton();
  //  }

    public void ResetFrameInputs()
    {
        saltar = false;
        disparar = false;
        cambiarCamara = false;
    }
}
