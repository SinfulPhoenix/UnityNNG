using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; //use Unity New Input System

public class Player : MonoBehaviour
{
    //Player Attributes
    [SerializeField] //show variable below so that it can be edited
    private float _playerHealth = 100f;
    [SerializeField]
    private float _currentPlayerHeath; //cache health for changes

    [SerializeField]
    private float _playerDefaultSpeed = 7f;
    [SerializeField]
    private float _currentPlayerSpeed; //cache speed for changes

    Vector3 _playerDirection; //cache player direction
    Vector3 _playerPosition; //cache player position
    Vector2 _playerInput; //cache input



    // Start is called before the first frame update
    void Start()
    {
        //smell my funky farts
    }

    // Update is called once per frame
    void Update()
    {
        OnMove(); //call movement when input is called
    }

    public void OnStart()
    {
        _currentPlayerSpeed = _playerDefaultSpeed; //setting _currentPlayerSpeed to _playerDefaultSpeed
    }

    public void OnDirection(InputAction.CallbackContext context)
    {
        _playerInput = context.ReadValue<Vector2>(); //read input given
        _playerDirection = new Vector3(_playerInput.x, _playerInput.y, 0); //move in direction based on that input
    }
    public void OnMove()
    {
        transform.Translate(_playerDirection * _currentPlayerSpeed * Time.deltaTime); //move in direction based on direction and speed
    }

}
