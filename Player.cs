using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float _speed = 9.0f;
    [SerializeField] private float _gravity = 30.0f;
    [SerializeField] private float _jumpHeight = 12.0f;
    [SerializeField] private Transform _standupPosition;

    private CharacterController _controller;
    private Animator _anim;
    private Transform _model;

    private Vector3 _direction;
    private Vector3 _newStandingPosition;

    private bool _Jumping = false;
    private bool _idle = true;
    private bool _walking = false;
    private bool _running = false;
    private bool _hanging = false;
    private bool _standingUp = false;
    private bool _canMove = true;
    private bool _pressingButton = false;

    private void Start()
    {
        _controller = GetComponent<CharacterController>();
        if (_controller == null) Debug.LogError("_controller is NULL");

        _anim = GetComponentInChildren<Animator>();
        if (_anim == null) Debug.LogError("Animator is NULL");
    }

    private void Update()
    {
        if (_canMove)
        {
            IsMoving();
            IsJumping();
        }

        _direction.y -= _gravity * Time.deltaTime;
        _controller.Move(_direction * Time.deltaTime);

        if (_hanging)
            CheckHangingClimbing();
    }

    private void GotoBeginning()
    {
        
    }

    private void IsMoving()
    {
        if (_controller.isGrounded)
        {
            _Jumping = false; //If you are grounded, you are not jumping
            _hanging = false; //This sets in case cancelling Ledge Hanging
            _anim.SetBool("LedgeGrab", false);//This sets in case cancelling Ledge Hanging
            _anim.SetBool("Climbing", false);//This sets in case cancelling Ledge Hanging
            float h = Input.GetAxisRaw("Horizontal");
            _anim.SetFloat("Speed", Math.Abs(h));
            
            if (Math.Abs(h) == 1)
            {
                _idle = false;
                _walking = true;
            }

            if (h == 1 && transform.eulerAngles.y == 180)
            {
                transform.localEulerAngles = Vector3.zero;
            }

            if (h == -1 && transform.eulerAngles.y == 0)
            {
                transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, 180, transform.localEulerAngles.z);
            }

            if (Input.GetButton("Fire3"))
            {
                _speed = 18f;
                _anim.SetBool("Running", true);
                _running = true;
                _walking = false;
            }

            if (Input.GetButtonUp("Fire3"))
            {
                _speed = 9f;
                _anim.SetBool("Running", false);
                _running = false;
            }

            if (_controller.isGrounded && h == 0)
            {
                _idle = true;
                _walking = false;
                _running = false;
                _anim.SetBool("Running", false);
            }

            if (!Input.GetButton("Fire3"))
            {
                _speed = 9f;
                _running = false;
                _anim.SetBool("Running", false);
            }

            _direction = new Vector3(0f, 0f, h) * _speed;
        }
    }

    private void IsJumping()
    {
        if (Input.GetButtonDown("Jump") && !_Jumping)
        {
            _direction.y += _jumpHeight;
            _idle = false;
            _Jumping = true;
        }

        if (!_walking && !_running && _Jumping)
        {
            _anim.SetBool("Jumping", true);            
        }

        if ((_walking || _running) && _Jumping)
        {
            _anim.SetBool("Jumping", true);
        }

        if (!_Jumping)
        {
            _anim.SetBool("Jumping", false);
        }
    }

    public void ThrowHandsUp(Vector3 ledgeToGrab, Vector3 ledgeStandingPosition)
    {
        Debug.Log(ledgeToGrab);
        _controller.enabled = false;
        //ReverseCanMove();
        _running = false;
        _hanging = true;
        _anim.SetBool("LedgeGrab", true);
        _anim.SetFloat("Speed", 0);
        _anim.SetBool("Running", false);
        transform.position = ledgeToGrab;
        _newStandingPosition = ledgeStandingPosition; //This variable determines where the character will finally stand up.
    }

    private void CheckHangingClimbing()
    {
        if (Input.GetButtonDown("Use"))
        {
            _anim.SetBool("Climbing", true);
            _anim.SetBool("Jumping", false);
            _anim.SetBool("LedgeGrab", false);
            _Jumping = false;
            _hanging = false;
        }
        if (Input.GetButtonDown("Cancel"))
        {
            _anim.SetBool("Climbing", false);
            _anim.SetBool("Jumping", false);
            _anim.SetBool("LedgeGrab", true);
            _Jumping = false;
            _hanging = true;
            _controller.enabled = true;
        }
    }

    public void StopClimbing()
    {
        _anim.SetBool("Climbing", false);
        transform.position = _newStandingPosition; //This makes the Player go to where the Model is standing up
        _standingUp = true;
        StartCoroutine(WaitForStandCompletion());
    }

    public bool GetJumpingStatus()
    {
        return _Jumping;
    }

    public void ReverseCanMove()
    {
        _canMove = !_canMove;
    }

    public void ChangePressingButton()
    {
        _pressingButton = !_pressingButton;
    }

    IEnumerator WaitForStandCompletion()
    {
        while (_standingUp)
        {
            yield return new WaitForSeconds(3.05f);
            _standingUp = false;
            _controller.enabled = true;
            //ReverseCanMove();
        }
    }
}
