using UnityEngine;
using UnityEngine.Events;

public class CharacterController2D : MonoBehaviour
{
	[SerializeField] private float JumpForce = 400f;							// Amount of force added when the player jumps.
	[Range(0, .3f)] [SerializeField] private float MovementSmoothing = .05f;	// How much to smooth out the movement
	[SerializeField] private bool AirControl = true;							// Whether or not a player can steer while jumping;
	[SerializeField] private LayerMask GroundLayer;							    // A mask determining what is ground to the character
	[SerializeField] private Transform GroundCheck;							    // A position marking where to check if the player is grounded.

	const float GroundedRadius = .2f;  // Radius of the overlap circle to determine if grounded
	private bool Grounded;            // Whether or not the player is grounded.
	private Rigidbody2D body;
	private bool FacingRight = true;  // For determining which way the player is currently facing.
	private Vector3 Velocity = Vector3.zero;

	[Header("Events")]
	[Space]

	public UnityEvent OnLandEvent;

	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }

	private void Awake()
	{
		body = GetComponent<Rigidbody2D>();

		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();
	}

	private void FixedUpdate()
	{
		bool wasGrounded = Grounded;
		Grounded = false;

		// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
		Collider2D[] colliders = Physics2D.OverlapCircleAll(GroundCheck.position, GroundedRadius, GroundLayer);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
			{
				Grounded = true;
				if (!wasGrounded)
					OnLandEvent.Invoke();
			}
		}
	}


	public void Move(float move, bool jump)
	{

		//only control the player if grounded or airControl is turned on
		if (Grounded || AirControl)
		{
			// Move the character by finding the target velocity
			Vector3 targetVelocity = new Vector2(move * 10f, body.velocity.y);
			// And then smoothing it out and applying it to the character
			body.velocity = Vector3.SmoothDamp(body.velocity, targetVelocity, ref Velocity, MovementSmoothing);

			// If the input is moving the player right and the player is facing left...
			if (move > 0 && !FacingRight)
			{
				// ... flip the player.
				Flip();
			}
			// Otherwise if the input is moving the player left and the player is facing right...
			else if (move < 0 && FacingRight)
			{
				// ... flip the player.
				Flip();
			}
		}
		// If the player should jump...
		if (Grounded && jump)
		{
			// Add a vertical force to the player.
			Grounded = false;
			body.AddForce(new Vector2(0f, JumpForce));
		}
	}


	private void Flip()
	{
		// Switch the way the player is labelled as facing.
		FacingRight = !FacingRight;

		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
}