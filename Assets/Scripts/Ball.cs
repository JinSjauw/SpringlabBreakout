using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Simple class that controls the ball
/// </summary>
public class Ball : MonoBehaviour
{
	[SerializeField] private Transform ballSprite;
	[SerializeField] private float speed = 5f;
	
	private Rigidbody2D ballRigidBody;
	private SpringComponent ballSpringComponent;
	private TrailRenderer ballTrailRenderer;
	
	private Vector3 startScale;
	private float trailStartWidth;
	
	private void Awake()
	{
		ballRigidBody = GetComponent<Rigidbody2D>();
		ballSpringComponent = GetComponent<SpringComponent>();
		ballTrailRenderer = GetComponent<TrailRenderer>();
		
		ballSpringComponent.SetEquilibriumPosition(1);
		startScale = ballSprite.localScale;
		trailStartWidth = ballTrailRenderer.startWidth;
	}

	private IEnumerator Start()
	{
		Vector3 force = Vector3.zero;

		yield return new WaitForSeconds(2);

		force.x = Random.Range(-1f, 1f);
		force.y = -1f;

		ballRigidBody.AddForce(force.normalized * speed);
	}

	private void FixedUpdate()
	{
		Move();
	}

	private void Update()
	{
		ballSprite.localScale = ballSpringComponent.Position * startScale;
		ballTrailRenderer.widthMultiplier = trailStartWidth * ballSpringComponent.Position;
	}

	//Maybe rewrite this in the case of the ball travelling too fast
	private void OnCollisionEnter2D(Collision2D collision)
	{
		
		ballSpringComponent.Nudge(1.25f);
		
		if (collision.gameObject.CompareTag("Wall"))
		{
			return;
		}
		
		//Make this a bounce function. This way I could make hit stop.
		if (collision.gameObject.CompareTag("Player"))
		{
			Bounce(collision);
			
			return;
		}
		
		if (collision.gameObject.TryGetComponent(out IBrick source))
		{
			foreach (IBrick brick in BrickResolver.ResolveBricksToDestroy(source))
			{
				Brick brickToDestroy = (Brick)brick;
				if (brickToDestroy.WillHitNeighboursOnDeath && !brickToDestroy.IsDestroyed)
				{
					foreach (IBrick brickNeighbours in BrickResolver.ResolveBricksToDestroy(brickToDestroy)) { }
				}
			}
		}
	}

	private void Move()
	{
		ballRigidBody.velocity = ballRigidBody.velocity.normalized * speed;
	}

	private void Bounce(Collision2D collision)
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
	}
}
