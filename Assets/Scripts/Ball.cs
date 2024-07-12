using System.Collections;
using UnityEngine;
using Ami.BroAudio;
using Random = UnityEngine.Random;

/// <summary>
/// Simple class that controls the ball
/// </summary>
public class Ball : MonoBehaviour
{
	[SerializeField] private GameEventChannel gameEventChannel;

	[SerializeField] private SoundID ballHitSFX;
	[SerializeField] private SoundID paddleHitSFX;
	[SerializeField] private SoundID ballLostSFX;
	
	[SerializeField] private GameObject impactFXPrefab;
	[SerializeField] private GameObject paddleImpactFXPrefab;
	
	[SerializeField] private Transform ballSprite;
	[SerializeField] private float speed = 5f;

	private ObjectPool objectPool;
	private Rigidbody2D ballRigidBody;
	private SpringComponent ballSpring;
	private TrailRenderer ballTrailRenderer;
	
	private Vector3 startScale;
	private float trailStartWidth;
	
	private void Awake()
	{
		ballRigidBody = GetComponent<Rigidbody2D>();
		ballSpring = GetComponent<SpringComponent>();
		ballTrailRenderer = GetComponent<TrailRenderer>();
		
		ballSpring.SetEquilibriumPosition(1);
		ballSpring.Nudge(-1);
		startScale = ballSprite.localScale;
		trailStartWidth = ballTrailRenderer.startWidth;
	}

	private IEnumerator Start()
	{
		objectPool = FindObjectOfType<ObjectPool>();
		
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
		HandleSquash();
	}

	//Maybe rewrite this in the case of the ball travelling too fast
	private void OnCollisionEnter2D(Collision2D collision)
	{
		Vector3 collisionPoint = collision.GetContact(0).point;
		PlaySFX(ballHitSFX);
		PlayVFX(impactFXPrefab, collisionPoint);
		ballSpring.Nudge(1.25f);
		
		if (collision.gameObject.CompareTag("Wall"))
		{
			gameEventChannel.BallBouncedOnWall();
			return;
		}
		
		//Make this a bounce function. This way I could make hit stop.
		if (collision.gameObject.CompareTag("Player"))
		{
			PlaySFX(paddleHitSFX);
			PlayVFX(paddleImpactFXPrefab, collisionPoint);
			Bounce(collision);
			
			return;
		}
		
		if (collision.gameObject.TryGetComponent(out IBrick source))
		{
			Brick brickToDestroy = (Brick)source;

			brickToDestroy.Hit(transform.position);
		}
	}

	private void PlaySFX(SoundID soundID)
	{
		BroAudio.Play(soundID);
	}
	
	private void PlayVFX(GameObject prefabFX, Vector3 position)
	{
		GameObject impactFX = objectPool.GetObject(prefabFX);
		impactFX.transform.position = position;
	}
	
	private void Move()
	{
		ballRigidBody.velocity = ballRigidBody.velocity.normalized * speed;
	}

	private void HandleSquash()
	{
		ballSprite.localScale = ballSpring.SpringValue * startScale;
		ballTrailRenderer.widthMultiplier = trailStartWidth * ballSpring.SpringValue;
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
