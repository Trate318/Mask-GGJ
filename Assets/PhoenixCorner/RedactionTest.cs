using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class RedactionTest : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnLeftMouse(InputAction.CallbackContext context)
    {
        // if (context.performed)
        // {
        //     Vector3 mousePosition = context.ReadValue<Vector3>();
        //     Debug.Log(mousePosition);
        // }
    }
    
    public void OnPositionUpdate(InputAction.CallbackContext context)
    {
        // Vector3 mousePosition = context.ReadValue<Vector2>();
        // Debug.Log(mousePosition);
    }
}
