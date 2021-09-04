using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; //use Unity New Input System

public class Player : MonoBehaviour
{
    [Header("Player Movement")]

    [SerializeField] private Vector3 velocity, desiredVelocity;
    [SerializeField, Range(0f, 100f)] private float _maxVelocity = 10f; //speed cap
    [SerializeField] private float _currentVelocity; //rate of velocity based on acceleration
    [SerializeField, Range(0f, 100f)] private float _acceleration = 10f;
    [SerializeField, Range(0f, 100f)] private float _deceleration = 10f;

    [Space]
    [Header("Player Coordinates")]
    public Vector2 _playerInput; //cache input
    public Vector3 _playerDirection; //cache player direction
    [Space]
    [Header("Player Components")]
    private PlayerInput _playerControls; //cache playerinput
    private Rigidbody2D _playerBody; //cache rigidbody
    private Animator _playerAnimation; //cache animator


    // Start is called before the first frame update
    private void Awake()
    {
        _playerControls = GetComponent<PlayerInput>(); //cache playerinput component into variable
        _playerAnimation = GetComponent<Animator>(); //cache animator component into variable
        _playerBody = GetComponent<Rigidbody2D>(); //cache rigidbody2d component into variable
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        OnMove(); //call movement when input is called
    }

    public void OnInput(InputAction.CallbackContext context)
    {
        _playerInput = context.ReadValue<Vector2>(); //read input value (Vector2)
        _playerInput = Vector2.ClampMagnitude(_playerInput, 1f); //normalize input
        _playerDirection = new Vector3(_playerInput.x, _playerInput.y, 0); //_playerInput in Vector3

        desiredVelocity = (_playerDirection * _maxVelocity); //assign speed cap
    }

    public void OnMove()
    {

        velocity = _playerBody.velocity; //send rigidbody.velocity to velocity

        //_currentVelocity = _acceleration * Time.deltaTime; //acceleration


        if (_playerDirection != Vector3.zero)
        {
            _currentVelocity = _acceleration * Time.deltaTime; //acceleration
        }
        else if (_playerDirection == Vector3.zero)
        {
            _currentVelocity = _deceleration * Time.deltaTime; //deceleration
        }


        velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, _currentVelocity); //move current velocity to desired velocity at accel rate (on x axis)
        velocity.y = Mathf.MoveTowards(velocity.y, desiredVelocity.y, _currentVelocity); //move current velocity to desired velocity at accel rate (on y axis)

        OnMoveAnimate(velocity.x, velocity.y); //animate based on velocity
        _playerBody.velocity = velocity; //send velocity back to rigidbody.velocity

    }

    public void OnMoveAnimate(float _velocityX, float _velocityY) //z will be added eventually
    {

        _velocityX = Mathf.Abs(_velocityX); //returns absolute (-1 = 1) of velocity on x 
        _velocityY = Mathf.Abs(_velocityY); //returns absolute (-1 = 1) of velocity on y

        if (_playerInput != Vector2.zero)
        {
            _playerAnimation.SetFloat("Move X", _playerDirection.x); //set blend tree Move X to calculate based on input on X axis
            _playerAnimation.SetFloat("Move Y", _playerDirection.y); //the above but on the Y
        }


        if (_velocityX >= 3.5f || _velocityY >= 3.5f) //if velocity is greater than or equal to 3.5
        {
            _playerAnimation.SetBool("Running", true);
            _playerAnimation.SetBool("Walking", false);
        }
        if (_velocityX < 3.5f && _velocityX > 0 || _velocityY < 3.5f && _velocityY > 0) //if velocity is less than 3.5 and greater than 0
        {
            _playerAnimation.SetBool("Running", false);
            _playerAnimation.SetBool("Walking", true);
        }
        if (_velocityX == 0 && _velocityY == 0) //if velocity is less than or equal to 0
        {
            _playerAnimation.SetBool("Walking", false);
            _playerAnimation.SetBool("Running", false);
        }
    }
}
