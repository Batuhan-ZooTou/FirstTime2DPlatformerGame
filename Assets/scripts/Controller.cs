using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
	/// <summary>
	/// Variables
	/// </summary>
	#region
	
	public Vector2 velocity;
	[Header("Jumping variables")]
	public int defaultAdditionalJumps = 1;
	public float airAcceleration;
	public float airAccelerationSlower;
	public float lowJumpMultiplier = 2f;
	public float fallMultiplier = 2.5f;
	[SerializeField] private float m_JumpForce;                          // zýplama gücü
	[Header("Hook variables")]
	public float ropeMaxCastDistance;
	public float hookSpeed;
	public float distance;
	[Header("dash variables")]
	[HideInInspector]public float lastImageXpos;
	public float distanceBetweenImage;
	public float dashPower;
	public float maxDash;
	[Header("WallJumping variables")]
	public float wallSlidingSpeed;
	public float wallCheckRadius;
	public float wallJumpHigh;
	public float wallJumpDistance;
	[Header("boolens")]
	public bool isDashing;
	public bool hugingWall;
	public bool wallSliding;
	public bool hooked;
	public bool m_Grounded;            // yere deyip deðmeme

	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;  // harekette keskinlik azaltýmý

	private Vector2 HookPosition;
	private Vector2 hookedPoint;
	private float dashTimer;
	private float hookTimer;
	private Vector3 m_Velocity = Vector3.zero;
	[Header("layer checks")]
	public LayerMask ropeLayerMask;
	[SerializeField] private LayerMask m_WhatIsGround;                          // zýplanabilir objeler

	[Header("check points")]
	[SerializeField] private Transform m_GroundCheck;                           // yere kontrol etme objesini tanýtma
	[SerializeField] private Transform wallCheckPoint;

	const float k_GroundedRadius = .2f; // yere deðmenin alaný
	private int additionalJumps;


	public Direction direction;
	public Animator animator;
	private Rigidbody2D m_Rigidbody2D;  //	rgb2 yi tanýmla
	private PlayerMovement playerMovement;
	private PlayerCombat playerCombat;
	#endregion

	/// <summary>
	///		Get Component
	/// </summary>
	private void Awake()
	{
		playerCombat = GetComponent<PlayerCombat>();
		playerMovement = GetComponent<PlayerMovement>();
		m_Rigidbody2D = GetComponent<Rigidbody2D>();
	}
	/// <summary>
	/// yere deðme olayýný kontrol etme
	/// </summary>
	private void FixedUpdate()
	{
		CheckIfWallSliding();
		CheckIfNearWall();
		CheckIfGrounded();
		CheckForAfterImage();
	}
	/// <summary>
	/// player movement kodundaki çalýþýcak olan methodun tanýmlanmasý
	/// </summary>
	/// <param name="move"></param> x ekseni hýzý
	public void Move(float move)
	{

		// Move the character by finding the target velocity | istediðimiz hýza göre oyuncuya hareket verme
		Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
		// And then smoothing it out and applying it to the character | karakterin rgb2 sinin velcocitysine yumuþaklýk eklemek
		m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
		if (playerCombat.isAttacking)
			return;
		// If the input is moving the player right and the player is facing left...
		if (!m_Grounded)
		{
			playerMovement.SetMovementState(PlayerMovement.MovementStates.Jumping);
		}
		else
		{
			playerMovement.SetMovementState(PlayerMovement.MovementStates.Idle);
			
		}
		if (move > 0)
		{
			playerMovement.facingDirection = PlayerMovement.FacingDirection.Right;
			playerMovement.SetMovementState(PlayerMovement.MovementStates.Running);
		}
		// Otherwise if the input is moving the player left and the player is facing right...
		else if (move < 0)
		{
			playerMovement.facingDirection = PlayerMovement.FacingDirection.Left;
			playerMovement.SetMovementState(PlayerMovement.MovementStates.Running);
		}


	}
	/// <summary>
	/// Jump methodu
	/// </summary>
	public void Jump()
	{
		// If the player should jump...
		if (m_Grounded || additionalJumps > 0 && wallSliding==false)
		{
            if (m_Rigidbody2D.velocity.y <0)
            {
				Vector2 addForce = new Vector2(0f, wallJumpHigh);
				m_Rigidbody2D.AddForce(addForce, ForceMode2D.Impulse);
				additionalJumps--;
			}
            else if (true)
            {
				// Add a vertical force to the player.
				m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
				additionalJumps--;
            }
		}
		else if (wallSliding == true && m_Grounded == false)
        {

			Vector2 addForce = new Vector2(direction.RoL.x*-wallJumpDistance,wallJumpHigh);
			m_Rigidbody2D.AddForce(addForce, ForceMode2D.Impulse);
        }
	}
	/// <summary>
	///  add some gravity when jump high or low jump like in hollow knight
	/// </summary>
	public void BetterJump()
	{
		//when going down
		if (m_Rigidbody2D.velocity.y < 0 && !Input.GetButton("Jump"))
		{
			m_Rigidbody2D.velocity += Vector2.up * Physics2D.gravity * (fallMultiplier - 1) * Time.deltaTime;
		}
		//when going up
		else if (m_Rigidbody2D.velocity.y > 0 && !Input.GetButton("Jump"))
		{
			m_Rigidbody2D.velocity += Vector2.up * Physics2D.gravity * (lowJumpMultiplier - 1) * Time.deltaTime;
		}
	}
	/// <summary>
	/// dashing ability
	/// </summary>
	public void Dashing()
	{
		//m_Rigidbody2D.velocity =new Vector2(dashPower*direction.RoL.x,m_Rigidbody2D.velocity.y);
		m_Rigidbody2D.AddForce(direction.RoL * dashPower * 2f, ForceMode2D.Impulse);
		playerMovement.dashState = PlayerMovement.DashState.Dashing;
	}
	/// <summary>
	/// dash colldown counter
	/// </summary>
	public void Recover()
	{
		dashTimer += Time.deltaTime;
		if (dashTimer >= maxDash)
		{
			isDashing = false;
			dashTimer = maxDash;
			playerMovement.dashState = PlayerMovement.DashState.Cooldown;
		}
	}
	/// <summary>
	/// set player ready to dash again if grounded
	/// </summary>
	public void Ready()
	{
		dashTimer = 0;
		
		playerMovement.dashState = PlayerMovement.DashState.Ready;
	}
	/// <summary>
	/// hooking method
	/// </summary>
	public void Hook()
	{
		hooked = false;
		HookPosition = new Vector2(transform.position.x, transform.position.y + 1);
		RaycastHit2D hit = Physics2D.Raycast(HookPosition, direction.aimDirection, ropeMaxCastDistance, ropeLayerMask);
		//shows angle of raycast
		//Debug.DrawRay(HookPosition, direction.aimDirection, raycolor, Time.deltaTime);
		if (hit.collider != null)
		{
			hooked = true;
			hookedPoint = new Vector2(hit.transform.position.x * hookSpeed, hit.collider.transform.position.y * hookSpeed);
			if (transform.position.y >= hookedPoint.y)
			{
				m_Rigidbody2D.velocity = Vector2.up * distance;
			}
			else
			{
				m_Rigidbody2D.AddForce(direction.aimDirection * hookSpeed, ForceMode2D.Impulse);
			}
			playerMovement.hookedState = PlayerMovement.HookedState.Hooked;
		}


	}
	/// <summary>
	/// cooldown of hooking
	/// </summary>
	public void Cooldown()
	{
		hooked = false;
		hookTimer += Time.deltaTime;
		if (hookTimer >= 2f)
		{
			hookTimer = 0f;
			playerMovement.hookedState = PlayerMovement.HookedState.Ready;
		}
	}
	/// <summary>
	/// cheks if grounded
	/// </summary>
	public void CheckIfGrounded()
	{
		m_Grounded = false;
		// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
		// This can be done using layers instead but Sample Assets will not overwrite your project settings.
		Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
			{
				m_Grounded = true;
				additionalJumps = defaultAdditionalJumps;
			}
			else
			{
				if (m_Grounded)
				{
					playerMovement.lastTimeGrounded = Time.time;
				}
				m_Grounded = false;
			}
		}
	}
	/// <summary>
	/// check if near wall
	/// </summary>
	public void CheckIfNearWall()
	{
		hugingWall = false;
		RaycastHit2D hit = Physics2D.Raycast(wallCheckPoint.position, direction.RoL, wallCheckRadius, m_WhatIsGround);
		if (hit.collider != null)
		{
			hugingWall = true;
		}
	}
	/// <summary>
	/// checks if sliding over wall
	/// </summary>
	public void CheckIfWallSliding()
	{
		if (hugingWall == true && m_Grounded == false && m_Rigidbody2D.velocity.y < 0)
		{
			wallSliding = true;
			additionalJumps = defaultAdditionalJumps;
		}
		else
		{
			wallSliding = false;
		}
	}
	/// <summary>
	/// if sliding true set vertical speed to sliding speed
	/// </summary>
	public void SetVerticalSpeedToWallSlidingSpeed()
	{
        if (wallSliding)
        {
            if (m_Rigidbody2D.velocity.y<(-wallSlidingSpeed))
            {
				m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, -wallSlidingSpeed);
            }
        }

	}
	/// <summary>
	/// change airacceleration speed
	/// </summary>
	public void SetPlayerAirAcceleration()
    {
		if (m_Grounded == false && wallSliding == false && playerMovement.horizontalMove != 0 && hooked == false)
        {
			Vector2 forceOnAir = new Vector2(airAcceleration * (playerMovement.horizontalMove/35), 0);
			m_Rigidbody2D.AddForce(forceOnAir);
			
		}
        else if (m_Grounded == false && wallSliding == false && playerMovement.horizontalMove == 0 && hooked == false)
        {
			m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x * airAccelerationSlower, m_Rigidbody2D.velocity.y);
        }
        
    }
	public void CheckForAfterImage()
    {
        if (isDashing)
        {
			if (Mathf.Abs(transform.position.x - lastImageXpos) > distanceBetweenImage)
			{
				AfterImagePool.Instance.GetFromPool();
				lastImageXpos = transform.position.x;
			}
		}
	}
    private void OnDrawGizmos()
    {
		Gizmos.DrawLine(wallCheckPoint.position,new Vector3(wallCheckPoint.position.x - wallCheckRadius,wallCheckPoint.position.y,wallCheckPoint.position.z));
    }

}
