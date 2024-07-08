using System;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "Input Handler")]
public class InputHandler : ScriptableObject, DefaultInput.IGameplayActions, DefaultInput.IMenuActions
{

    public event EventHandler<float> MoveEvent;
    public event EventHandler SpawnBallEvent; 
    
    private DefaultInput input;
    
    private void OnEnable()
    {
        if (input == null)
        {
            input = new DefaultInput();
        }
        
        input.Gameplay.Enable();
        input.Gameplay.SetCallbacks(this);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            MoveEvent?.Invoke(this, context.ReadValue<float>());
        }

        if (context.canceled)
        {
            MoveEvent?.Invoke(this, 0);
        }
    }

    public void OnSpawnBall(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            SpawnBallEvent?.Invoke(this, EventArgs.Empty);
        }
    }

    public void OnOpenMenu(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("OpeningMenu!");
            //Disable Gameplay input & Enable Menu input;
        }
    }

    public void OnNavigate(InputAction.CallbackContext context)
    {
        
    }

    public void OnCloseMenu(InputAction.CallbackContext context)
    {
        //Disable Menu Input & Enable Gameplay Input
    }
}
