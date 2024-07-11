using UnityEngine;

/// <summary>
/// Simple behaviour to control the paddle.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PaddleController : MonoBehaviour
{
	[SerializeField] private InputHandler inputHandler;
	
	[SerializeField] private float moveSpeed = 10;
	
	private Rigidbody2D paddleRigidBody;

	private SpringComponent paddleSpring;
	private Vector3 startScale;
	
	private Vector2 moveDirection;
	
	
	private void Awake()
	{
		paddleRigidBody = GetComponent<Rigidbody2D>();
		paddleSpring = GetComponent<SpringComponent>();
		paddleSpring.SetEquilibriumPosition(1);
		paddleSpring.Nudge(1.5f);
		startScale = transform.localScale;
	}

	private void Start()
	{
		inputHandler.MoveEvent += HandleMoveInput;
	}

	private void FixedUpdate()
	{
		Move();
	}

	private void Update()
	{
		Squash(paddleSpring.SpringValue);
	}

	private void OnCollisionEnter2D(Collision2D other)
	{
		if (other.collider.CompareTag("Ball"))
		{
			paddleSpring.Nudge(.75f);
		}
	}

	private void Move()
	{
		Vector2 paddleVelocity = paddleRigidBody.velocity;
		Vector2 desiredVelocity = moveDirection * moveSpeed;
		
		Vector2 neededAcceleration = (desiredVelocity - paddleVelocity) / Time.fixedDeltaTime;
		
		paddleRigidBody.AddForce(neededAcceleration * paddleRigidBody.mass);
	}

	private void Squash(float springValue)
	{
		springValue = Mathf.Clamp(springValue, .2f, 2.25f);
		transform.localScale = new Vector3(startScale.x + springValue, startScale.y - (springValue - 1), startScale.z);
	}
	
	private void HandleMoveInput(object sender, float inputValue)
	{
		moveDirection.x = inputValue;
	}
}
