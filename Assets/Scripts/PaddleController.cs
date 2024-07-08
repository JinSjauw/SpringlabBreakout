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
	private Vector2 moveDirection;

	private void Awake()
	{
		paddleRigidBody = GetComponent<Rigidbody2D>();
	}

	private void Start()
	{
		inputHandler.MoveEvent += HandleMoveInput;
	}

	private void FixedUpdate()
	{
		Move();
	}

	private void Move()
	{
		Vector2 paddleVelocity = paddleRigidBody.velocity;
		Vector2 desiredVelocity = moveDirection * moveSpeed;
		
		Vector2 neededAcceleration = (desiredVelocity - paddleVelocity) / Time.fixedDeltaTime;
		//neededAcceleration = Vector2.ClampMagnitude(neededAcceleration, maxAccel);
		
		Debug.Log("Need Acceleration" + neededAcceleration);
		
		paddleRigidBody.AddForce(neededAcceleration * paddleRigidBody.mass);
	}
	
	private void HandleMoveInput(object sender, float inputValue)
	{
		moveDirection.x = inputValue;
	}
}
