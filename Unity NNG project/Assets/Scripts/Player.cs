using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; //use Unity New Input System

public class Player : MonoBehaviour
{
    [Header("Player Movement")]

    [SerializeField, Range(0f, 100f)] private float _maxVelocity = 10f; //speed cap
    [SerializeField] private float _velocity; //rate of velocity based on acceleration
    [SerializeField] private Vector3 _currentVelocity, _desiredVelocity;
    [SerializeField, Range(0f, 100f)] private float _acceleration = 10f;
    [SerializeField, Range(0f, 100f)] private float _deceleration = 20f;

    [SerializeField] private float _currentTraction; 
    [SerializeField, Range(0f, 50f)] private float _traction = 4f; //how much drag/traction to stop sliding
    [SerializeField, Range(0f, 1f)] private float _tractionTimer = 0.35f; //drag/traction timer: lower = less time to control slide

    [Space]
    [Header("Player Coordinates")]

    [SerializeField] private Vector2 _currentInput; //cache current input
    [SerializeField] private Vector3 _currentDirection; //cache vector3 convert of vector2 input
    [SerializeField] private Vector3 _lastDirection; //cache last input to compare
    private bool _directionChange => (_currentDirection != _lastDirection); //trigger for stopping sliding

    [Space]
    [Header("Player Components")]
    //private PlayerInput _controls; //cache playerinput
    private Rigidbody2D _body; //cache rigidbody
    private Animator _animation; //cache animator


    // Start is called before the first frame update
    private void Awake()
    {
        //_controls = GetComponent<PlayerInput>(); //cache playerinput component into variable
        _animation = GetComponent<Animator>(); //cache animator component into variable
        _body = GetComponent<Rigidbody2D>(); //cache rigidbody2d component into variable

    }
    
    // Update is called once per frame
    void FixedUpdate()
    {
        OnMove(); //call movement when input is called
    }

    public void OnInput(InputAction.CallbackContext context)
    {

        _currentInput = context.ReadValue<Vector2>(); //read input value (Vector2)
        _currentInput = Vector2.ClampMagnitude(_currentInput, 1f); //normalize input
        _currentDirection = new Vector3(_currentInput.x, _currentInput.y, 0); //_currentInput in Vector3

        StartCoroutine(PreviousInputRecord(_tractionTimer));

    }

    public void OnMove()
    {
        _desiredVelocity = (_currentDirection * _maxVelocity); //assign speed cap
        _currentVelocity = _body.velocity; //send rigidbody.velocity to velocity
        _currentTraction = _body.drag; //traction is linear drag, wow


        ApplyPhysics();

        //move current velocity to desired velocity at accel rate (on x axis)
        _currentVelocity.x = Mathf.MoveTowards(_currentVelocity.x, _desiredVelocity.x, _velocity);
        //move current velocity to desired velocity at accel rate (on y axis)
        _currentVelocity.y = Mathf.MoveTowards(_currentVelocity.y, _desiredVelocity.y, _velocity); 

        if (_velocity == 0f){ //if we're not moving
            _currentVelocity = Vector3.zero; //zero out rigidbody.velocity to avoid negative numbers
        }

        OnMoveAnimate(_currentVelocity.x, _currentVelocity.y); //animate based on velocity
        _body.velocity = _currentVelocity; //send velocity back to rigidbody.velocity
        _body.drag = _currentTraction;

    }

    IEnumerator PreviousInputRecord(float _tractionTimer)
    {
        yield return new WaitForSeconds(_tractionTimer);
        _lastDirection = new Vector3(_currentDirection.x, _currentDirection.y, 0);
    }

    void ApplyPhysics()
    {
        if (_currentDirection != Vector3.zero) //if there are inputs
        {
            _velocity = _acceleration * Time.deltaTime; //accelerate

            if (_directionChange) //control slide when changing direction
            {
                _currentTraction = _traction; //by using traction
            } else { _currentTraction = 0f; } 
        }
        else if (_currentDirection == Vector3.zero) //&& _currentTraction == 0f) //if there arent inputs
        {
            _velocity = _deceleration * Time.deltaTime; //decelerate
            
            if (_currentVelocity == Vector3.zero)
            {
                _velocity = 0f; //if no input, _velocity = 0
            }
        }

        
    }

    public void OnMoveAnimate(float _velocityX, float _velocityY) //z will be added eventually
    {

        _velocityX = Mathf.Abs(_velocityX); //returns absolute (-1 = 1) of velocity on x 
        _velocityY = Mathf.Abs(_velocityY); //returns absolute (-1 = 1) of velocity on y

        if (_currentDirection != Vector3.zero)
        {
            _animation.SetFloat("Move X", _currentDirection.x); //set blend tree Move X to calculate based on input on X axis
            _animation.SetFloat("Move Y", _currentDirection.y); //the above but on the Y
        }


        if (_velocityX >= 3.5f || _velocityY >= 3.5f) //if velocity is greater than or equal to 3.5
        {
            _animation.SetBool("Running", true);
            _animation.SetBool("Walking", false);
        }
        if (_velocityX < 3.5f && _velocityX > 0 || _velocityY < 3.5f && _velocityY > 0) //if velocity is less than 3.5 and greater than 0
        {
            _animation.SetBool("Running", false);
            _animation.SetBool("Walking", true);
        }
        if (_velocityX == 0 && _velocityY == 0) //if velocity is less than or equal to 0
        {
            _animation.SetBool("Walking", false);
            _animation.SetBool("Running", false);
        }
    }
}
