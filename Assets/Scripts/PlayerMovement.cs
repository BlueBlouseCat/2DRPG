using System.Collections;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Animator animator;
    private bool playingFootSteps = false;
    [SerializeField] private float footstepSpeed = 0.5f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator =  GetComponent<Animator>();
    }

    void Update()
    {
        if(PauseController.IsGamePaused)
        {
            if(rb.velocity != Vector2.zero)
            {
                rb.velocity = Vector2.zero; // 停止玩家移动
                StopMovementAnimations();
            }
            // 停止脚步声
            StopFootSteps();
            return;
        }
        rb.velocity = moveInput * moveSpeed;
        animator.SetBool("isWalking", rb.velocity.magnitude > 0);

        // 开启脚步声
        if(rb.velocity.magnitude > 0 && !playingFootSteps)
        {
            StartFootSteps();
        }
        else if(rb.velocity.magnitude == 0)
        {
            StopFootSteps();
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        if(context.canceled)
        {
            StopMovementAnimations();
        }
        moveInput = context.ReadValue<Vector2>();
        animator.SetFloat("InputX", moveInput.x);
        animator.SetFloat("InputY", moveInput.y);
    }

    void StopMovementAnimations()
    {
        animator.SetBool("isWalking", false);
        animator.SetFloat("LastInputX", moveInput.x);
        animator.SetFloat("LastInputY", moveInput.y);
    }

    private void StartFootSteps()
    {
        playingFootSteps = true;
        InvokeRepeating(nameof(PlayFootstep), 0f, footstepSpeed);
    }
    private void StopFootSteps()
    {
        playingFootSteps = false;
        CancelInvoke(nameof(PlayFootstep));
    }

    private void PlayFootstep()
    {
        SoundEffectManager.Play("Footstep", true);
    }
}
