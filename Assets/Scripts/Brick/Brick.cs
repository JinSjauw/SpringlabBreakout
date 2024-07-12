using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Ami.BroAudio;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class Brick : MonoBehaviour, IBrick
{
    
    [SerializeField] private int maxLives;
    [SerializeField] private BrickTypes brickType;

    [SerializeField] private Color breakColor;
    [SerializeField] private Color fadedColor;
    [SerializeField] private float fadeToForeGroundTime;

    [SerializeField] private SoundID explosionSFX;
    
    [SerializeField] private GameObject explosiveFXPrefab;
    [SerializeField] private GameObject superExplosiveFXPrefab;
    [SerializeField] private GameObject breakFXPrefab;
    
    //IBrick fields
    private List<Brick> neighbours;
    private bool willHitNeighboursOnDeath;
    private int currentLives;

    private ObjectPool objectPool;
    private Rigidbody2D brickRigidBody;
    private Collider2D brickCollider;
    private SpriteRenderer brickRenderer;
    private Spring2DComponent brickSpring;
    
    private Color startColor;
    private float fadeColorAlpha;

    private GameObject destructionFXPrefab;
    
    private Vector3 targetPosition;
    private Vector2Int gridPosition;
    private bool isDestroyed;
    private bool isHitDirectly;
    private float spawnDelay = 0;
    
    public BrickTypes BrickType => brickType;
    public IEnumerable<IBrick> Neighbours => neighbours;
    public bool WillHitNeighboursOnDeath => willHitNeighboursOnDeath;
    public int Lives => currentLives;
    public Vector2Int GridPosition => gridPosition;

    public event EventHandler<Vector3> BrickExplodedEvent;
    public event EventHandler<Brick> BrickDestroyedEvent; 
    
    private void Awake()
    {
        brickRigidBody = GetComponent<Rigidbody2D>();
        brickCollider = GetComponent<Collider2D>();
        brickRenderer = GetComponent<SpriteRenderer>();
        brickSpring = GetComponent<Spring2DComponent>();
        
        brickSpring.SetEquilibriumPosition(0, 0);
        
        brickType = BrickTypes.NORMAL;
    }

    private void Update()
    {
        //Fade Out knocked bricks.
        FadeOut();
        
        if(isDestroyed) return;
        
        //Move to given position;

        if (spawnDelay > 0)
        {
            spawnDelay -= Time.deltaTime;    
            return;
        }
        
        if (spawnDelay <= 0 && (brickSpring.IsStoppedX || brickSpring.IsStoppedY))
        {
            brickSpring.Stop(false, false);
        }
        
        MoveToPosition();
    }

    #region Private Functions

    private void MoveToPosition()
    {
        transform.position = targetPosition + new Vector3(brickSpring.SpringValueX, brickSpring.SpringValueY, 0);
    }
    
    private void KnockOff(Vector3 source)
    {
        brickCollider.excludeLayers = LayerMask.GetMask("Default", "Brick");
        brickRigidBody.isKinematic = false;

        Vector3 knockBackForce = (transform.position - source).normalized;
        
        brickRigidBody.AddForce(knockBackForce * Random.Range(8, 22), ForceMode2D.Impulse);
        brickRigidBody.AddTorque(Random.Range(0.3f, 3), ForceMode2D.Impulse);

        isDestroyed = true;
        
        BrickDestroyedEvent?.Invoke(this, this);
    }
    private void Explode()
    {
        foreach (Brick brickToDestroy in neighbours)
        {
            if(brickToDestroy.currentLives > 0) continue;

            if (!brickToDestroy.willHitNeighboursOnDeath)
            {
                brickToDestroy.DestroyBrick();
            }
        }

        BroAudio.Play(explosionSFX);
        PlayVFX();
        
        BrickExplodedEvent?.Invoke(this, targetPosition);
        BrickDestroyedEvent?.Invoke(this, this);
        
        isDestroyed = true;
        gameObject.SetActive(false);
    }
    
    private void PlayVFX()
    {
        GameObject destructionFX = objectPool.GetObject(destructionFXPrefab);
        destructionFX.transform.position = transform.position;
    }
    
    private void DestroyBrick()
    {
        Instantiate(breakFXPrefab, targetPosition, quaternion.identity);
        
        isDestroyed = true;
        gameObject.SetActive(false);
        BrickDestroyedEvent?.Invoke(this, this);
    }
    
    private void FadeOut()
    {
        if (isDestroyed && gameObject.activeSelf)
        {
            fadeColorAlpha = Mathf.MoveTowards(fadeColorAlpha, 1, fadeToForeGroundTime * Time.deltaTime);
            if (fadeColorAlpha < 1)
            {
                brickRenderer.color = Color.Lerp(startColor, fadedColor, fadeColorAlpha);
            }
        }
    }
    private void DelayedExplode(float time)
    {
        StartCoroutine(DelayedHit(Explode, time));
    }
    private IEnumerator DelayedHit(Action hitAction, float waitTime = 1.5f)
    {
        yield return new WaitForSeconds(waitTime);

        hitAction?.Invoke();
    }
    
    #endregion

    #region Public Functions

    public void Initialize(Vector2Int gridPos, ObjectPool objPool, float delay = 0, IEnumerable<Brick> neighboursCollection = null)
    {
        gridPosition = gridPos;
        objectPool = objPool;
        targetPosition = transform.position;
        
        if (neighboursCollection != null)
        {
            neighbours = neighboursCollection.ToList();
        }

        spawnDelay = delay;
        
        startColor = brickRenderer.color;
        breakColor += startColor;
        fadedColor *= startColor;
        
        brickSpring.Nudge(0, 10);
        
        MoveToPosition();

        if (spawnDelay > 0)
        {
            brickSpring.Stop(true, true);
        }
    }
    public void SetBrickType(BrickTypes type)
    {
        brickType = type;
        
        switch (brickType)
        {
            case BrickTypes.NORMAL:
                maxLives = 0;
                break;
            case BrickTypes.POWERUP:
                maxLives = 1;
                break;
            case BrickTypes.EXPLOSIVE:
                maxLives = 0;
                break;
            case BrickTypes.SUPEREXPLOSIVE:
                maxLives = 0;
                break;
        }

        currentLives = maxLives;
        
        switch (brickType)
        {
            case BrickTypes.NORMAL:
                brickRenderer.color = Color.white;
                destructionFXPrefab = breakFXPrefab;
                break;
            case BrickTypes.POWERUP:
                brickRenderer.color = Color.green;
                destructionFXPrefab = breakFXPrefab;
                break;
            case BrickTypes.EXPLOSIVE:
                brickRenderer.color = Color.blue;
                willHitNeighboursOnDeath = true;
                destructionFXPrefab = explosiveFXPrefab;
                break;
            case BrickTypes.SUPEREXPLOSIVE:
                brickRenderer.color = Color.red;
                willHitNeighboursOnDeath = true;
                destructionFXPrefab = superExplosiveFXPrefab;
                break;
            default:
                brickRenderer.color = Color.magenta;
                break;
        }
    }
    public void OnResolveHit()
    {
        currentLives--;
        
        if (currentLives >= 0)
        {
            brickRenderer.color = Color.Lerp(breakColor, startColor, currentLives / (float)maxLives);
        }
    }
    public void Hit(Vector3 ballPosition)
    {
        if (!willHitNeighboursOnDeath && currentLives <= 0)
        {
            KnockOff(ballPosition);
            
            return;
        }

        float delay = 0;
        
        foreach (IBrick brick in BrickResolver.ResolveBricksToDestroy(this))
        {
            Brick brickToDestroy = (Brick)brick;
            if (brickToDestroy.isActiveAndEnabled && brickToDestroy.willHitNeighboursOnDeath)
            {
                brickToDestroy.DelayedExplode(delay);
                delay += .5f;
            }
        }
    }

    public void Push(float x, float y)
    {
        brickSpring.Nudge(x, y);
    }

    #endregion

}
