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
    private float _currentPlayerHealth; //cache health globally

    [SerializeField]
    private float _playerDefaultSpeed = 7f;
    [SerializeField]
    private float _currentPlayerSpeed; //cache speed globally

    Vector3 _playerDirection; //cache player direction globally
    Vector3 _playerPosition; //cache player position globally
    Vector2 _playerInput; //cache input globally

    private Animator _playerAnimation; //cache animator component globally



    // Start is called before the first frame update
    void Start()
    {
        //smell my funky farts
        OnStart();
    }

    // Update is called once per frame
    void Update()
    {
        OnMove(); //call movement when input is called
    }

    public void OnStart()
    {
        _currentPlayerSpeed = _playerDefaultSpeed; //setting _currentPlayerSpeed to _playerDefaultSpeed
        _playerAnimation = GetComponent<Animator>(); //cache animator component
    }

    public void OnDirection(InputAction.CallbackContext context)
    {
        _playerInput = context.ReadValue<Vector2>(); //read input given
        _playerDirection = new Vector3(_playerInput.x, _playerInput.y, 0); //move in direction based on that input
    }
    public void OnMove()
    {
        if (_playerInput != Vector2.zero) {
            transform.Translate(_playerDirection * _currentPlayerSpeed * Time.deltaTime); //move in direction based on direction and speed\

            _playerAnimation.SetFloat("Move X", _playerInput.x); //set blend tree Move X to calculate based on input on X axis
            _playerAnimation.SetFloat("Move Y", _playerInput.y); //the above but on the Y

            _playerAnimation.SetBool("Walking", true); //set walking bool to true
        } else
        {
            _playerAnimation.SetBool("Walking", false);
        }
    }

}
