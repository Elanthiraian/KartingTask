using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class VehicleMovement : MonoBehaviour
{
    public ArcadeKart ArcadeKart;
    public Vector2 _moveValue;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void OnMove(InputAction.CallbackContext inputAction)
    {
        
        _moveValue = inputAction.ReadValue<Vector2>();
        var acc = _moveValue.y > 0 ? true : false;
        var brake = acc == true ? false : true;
        ArcadeKart.MoveVehicle(acc, brake, _moveValue.x);
    }
}
