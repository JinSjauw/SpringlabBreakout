using System;
using System.Collections;
using UnityEngine;
using Ami.BroAudio;
using Random = UnityEngine.Random;

/// <summary>
/// Simple class that controls the ball
/// </summary>
public class BallController : MonoBehaviour
{
	[Header("Event Channel")]
	[SerializeField] private GameEventChannel gameEventChannel;

	[Header("SFX")]
	[SerializeField] private SoundID ballHitSFX;
	[SerializeField] private SoundID paddleHitSFX;
	[SerializeField] private SoundID ballLostSFX;
	
	[Header("VFX")]
	[SerializeField] private GameObject impactFXPrefab;
	[SerializeField] private GameObject paddleImpactFXPrefab;
	[SerializeField] private GameObject ballLostFXPrefab;
	
	[Header("General")]
	[SerializeField] private Transform ballSprite;
	[SerializeField] private float speed = 5f;

	private ObjectPool objectPool;
	private Rigidbody2D ballRigidBody;
	private SpringComponent ballSpring;
	private TrailRenderer ballTrailRenderer;

	private Vector3 startPosition;
	private Vector3 startScale;
	private float trailStartWidth;

	#region Unity Functions

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

	private void Start()
	{
		objectPool = FindObjectOfType<ObjectPool>();
		startPosition = transform.position;
		StartCoroutine(SpawnBall());
	}

	private IEnumerator SpawnBall()
	{
		ballSprite.gameObject.SetActive(false);
		ballTrailRenderer.emitting = false;
		
		PlayVFX(ballLostFXPrefab, startPosition);
		ballRigidBody.velocity = Vector3.zero;

		yield return new WaitForSeconds(0.5f);
		
		ballSprite.gameObject.SetActive(true);
		transform.position = startPosition;
		
		ballTrailRenderer.Clear();
		ballTrailRenderer.emitting = true;
		
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
	
	private void OnCollisionEnter2D(Collision2D collision)
	{
		Vector3 collisionPoint = collision.GetContact(0).point;

		if (collision.gameObject.CompareTag("Bottom"))
		{
			PlaySFX(ballLostSFX);
			PlayVFX(ballLostFXPrefab, collisionPoint);
			
			gameEventChannel.BallLost();
			
			StartCoroutine(SpawnBall());
		}
		
		PlaySFX(ballHitSFX);
		PlayVFX(impactFXPrefab, collisionPoint);
		ballSpring.Nudge(1.25f);
		
		if (collision.gameObject.CompareTag("Wall"))
		{
			gameEventChannel.BallBouncedOnWall();
			return;
		}
		
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
	
	#endregion

	#region Private Functions
	
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
	
	#endregion
}
