using System.Collections;
using UnityEngine;

/// <summary>
/// Simple class that controlls the ball
/// </summary>
public class Ball : MonoBehaviour
{
	[SerializeField] private Rigidbody2D ballRigidBody;
	[SerializeField] private float speed = 5f;

	protected IEnumerator Start()
	{
		Vector3 force = Vector3.zero;

		yield return new WaitForSeconds(2);

		force.x = Random.Range(-1f, 1f);
		force.y = -1f;

		ballRigidBody.AddForce(force.normalized * speed);
	}

	protected void FixedUpdate()
	{
		ballRigidBody.velocity = ballRigidBody.velocity.normalized * speed;
	}

	protected void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.CompareTag("Wall"))
		{
			return;
		}
		
		if (collision.gameObject.CompareTag("Player"))
		{
			Vector2 paddlePosition = collision.transform.position;
			Vector2 contactPoint = collision.GetContact(0).point;

			float offset = paddlePosition.x - contactPoint.x;
			float maxOffset = collision.collider.bounds.size.x / 2;

			float currentAngle = Vector2.SignedAngle(Vector2.up, ballRigidBody.velocity);
			float bounceAngle = (offset / maxOffset) * 75f;
			float newAngle = Mathf.Clamp(currentAngle + bounceAngle, -75f, 75f);

			Quaternion rotation = Quaternion.AngleAxis(newAngle, Vector3.forward);
			ballRigidBody.velocity = rotation * Vector2.up * ballRigidBody.velocity.magnitude;

			return;
		}

		if (collision.gameObject.TryGetComponent(out IBrick hitBrick))
		{
			foreach (IBrick brick in BrickResolver.ResolveBricksToDestroy(hitBrick))
			{
				
			}
		}
		
		
	}
}
