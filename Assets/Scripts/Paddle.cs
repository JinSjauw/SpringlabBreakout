using UnityEngine;

/// <summary>
/// Simple behaviour to control the paddle.
/// </summary>
public class Paddle : MonoBehaviour
{
	[SerializeField] private Rigidbody2D paddleRigidBody;
	[SerializeField] private float moveSpeed = 2;

	private Vector3 moveDirection;

	protected void Update()
	{
		moveDirection = Vector3.zero;

		if (Input.GetKey(KeyCode.LeftArrow))
		{
			moveDirection = Vector3.left;
		}
		else if (Input.GetKey(KeyCode.RightArrow))
		{
			moveDirection = Vector3.right;
		}
	}

	protected void FixedUpdate()
	{
		paddleRigidBody.AddForce(moveDirection * moveSpeed);
	}
}
