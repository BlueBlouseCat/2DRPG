using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;
    private Rigidbody2D _rb;
    private Vector2 _moveInput;
    private Animator _animator;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator =  GetComponent<Animator>();
    }

    void Update()
    {
        _rb.velocity = _moveInput * _moveSpeed;
    }

    public void Move(InputAction.CallbackContext context)
    {
        _animator.SetBool("isWalking", true);

        if(context.canceled)
        {
            _animator.SetBool("isWalking", false);
            _animator.SetFloat("LastInputX", _moveInput.x);
            _animator.SetFloat("LastInputY", _moveInput.y);
        }
        _moveInput = context.ReadValue<Vector2>();
        _animator.SetFloat("InputX", _moveInput.x);
        _animator.SetFloat("InputY", _moveInput.y);
    }
}
