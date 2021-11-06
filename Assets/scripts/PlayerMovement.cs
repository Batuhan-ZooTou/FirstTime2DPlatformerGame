using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    /// <summary>
    /// Enums
    /// </summary>
    #region
    public enum MovementStates
    {
        Idle,
        Running,
        Jumping,
        Attacking,
        Death,
        Hurt
    }
    public enum FacingDirection
    {
        Right,
        Left
    }
    public enum DashState
    {
        Ready,
        Dashing,
        Cooldown
    }
    public enum HookedState
    {
        Ready,
        Hooked
    }
    #endregion

    /// <summary>
    /// Variables
    /// </summary>
    #region
    [Header("Movement States (enums)")]
    public MovementStates movementState;
    public FacingDirection facingDirection;
    public DashState dashState;
    public HookedState hookedState;

    [Header("Movement values")]
    public float runSpeed = 40f;
    public float horizontalMove = 0f;
    public float verticalMove = 0f;
    public float rememberGroundedFor;
    public float lastTimeGrounded;
    [Header("Boolens")]
    public bool facingRight = false;

    public Direction direction;
    private Controller controller;
    private PlayerCombat playerCombat;
    private SpriteRenderer spriteRenderer;
    private PlayerAnimation playerAnimation;
    #endregion
    /// <summary>
    /// Get Component
    /// </summary>
    private void Awake()
    {
        playerAnimation = GetComponent<PlayerAnimation>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        controller = GetComponent<Controller>();
        playerCombat = GetComponent<PlayerCombat>();
    }
    /// <summary>
    /// Calling methods per frame
    /// </summary>
    void FixedUpdate()
    {
        controller.SetPlayerAirAcceleration();
        controller.SetVerticalSpeedToWallSlidingSpeed();
        SetHookingState();
        PlayAnimationsBasedOnState();
        SetCharacterDirection();
        SetDashingState();
        controller.Move(horizontalMove * Time.fixedDeltaTime);
        controller.BetterJump();
    }
    
    /// <summary>
    /// Getting Input for attack and jump
    /// </summary>
    void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
        verticalMove = Input.GetAxisRaw("Vertical");

        if (Input.GetButtonDown("Jump") || Time.time - lastTimeGrounded <= rememberGroundedFor )
        {
            controller.Jump();
        }
        

        if (Input.GetButtonDown("Fire1"))
        {
            StartCoroutine(AttackOrder());
        }
    }
    /// <summary>
    /// Stopping spam attack
    /// </summary>
    /// <returns></returns>
    private IEnumerator AttackOrder()
    {
        if (playerCombat.isAttacking)
        {
            yield break;
        }
        playerCombat.isAttacking = true;
        SetMovementState(PlayerMovement.MovementStates.Attacking);
        playerAnimation.TriggerAttackAnimation();
        yield return new WaitForSeconds(1 / playerCombat.atkSpeed);
        playerCombat.Attack();
        playerCombat.isAttacking = false;
        yield break;

    }
    /// <summary>
    /// States
    /// </summary>
    #region
    private void SetCharacterDirection()
    {
        switch (facingDirection)
        {
            case FacingDirection.Right:
                Flip();
                break;
            case FacingDirection.Left:
                Flip();
                break;
        }
    }
    /// <summary>
    /// checks players direction and if it different from the way it goes scale it with -1 his posX 
    /// </summary>
    private void Flip()
    {
        if (horizontalMove>0 && !facingRight)
        {
            Vector2 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
            facingRight = true;
        }
        else if (horizontalMove < 0 && facingRight)
        {
            Vector2 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
            facingRight = false;
        }  
    }
    /// <summary>
    /// plays animation
    /// </summary>
    private void PlayAnimationsBasedOnState()
    {
        switch (movementState)
        {
            case MovementStates.Idle:
                playerAnimation.PlayIdleAnim();
                break;
            case MovementStates.Running:
                playerAnimation.PlayRunningAnim();
                break;
            case MovementStates.Jumping:
                playerAnimation.PlayJumpingAnim();
                break;
            case MovementStates.Attacking:
                break;
            case MovementStates.Death:
                break;
            case MovementStates.Hurt:
                break;
            default:
                break;
        }
    }
    /// <summary>
    /// sets movement state
    /// </summary>
    /// <param name="movementStates"></param>
    public void SetMovementState(MovementStates movementStates)
    {
        movementState = movementStates;
    }
    /// <summary>
    /// sets dashing states
    /// </summary>
    public void SetDashingState()
    {
        switch (dashState)
        {
            case DashState.Ready:
                if (Input.GetKey(KeyCode.X) && (direction.aimDirection.x == 1 || direction.aimDirection.x == -1 ) && horizontalMove!=0)
                {
                    controller.isDashing = true;
                    AfterImagePool.Instance.GetFromPool();
                    controller.lastImageXpos = transform.position.x;
                    controller.Dashing();
                }
                break;
            case DashState.Dashing:
                controller.Recover();             
                break;
            case DashState.Cooldown:
                if (controller.m_Grounded)
                {
                    controller.Ready();
                }              
                break;
        }
    }
    /// <summary>
    /// sets hooking states
    /// </summary>
    public void SetHookingState()
    {
        switch(hookedState)
        {
            case HookedState.Ready:
                if (Input.GetKey(KeyCode.LeftControl))
                {
                    controller.Hook();
                }
                break;
            case HookedState.Hooked:
                controller.Cooldown();
                break;

        }
    }
    #endregion
}
