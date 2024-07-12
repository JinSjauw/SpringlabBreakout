using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Ami.BroAudio;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
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
    
    private Color startColor;
    private float fadeColorAlpha;

    private GameObject destructionFXPrefab;
    
    private Vector3 worldPosition;
    private Vector2Int gridPosition;
    private bool isDestroyed;
    private bool isHitDirectly;
    
    public BrickTypes BrickType => brickType;
    public IEnumerable<IBrick> Neighbours => neighbours;
    public bool WillHitNeighboursOnDeath => willHitNeighboursOnDeath;
    public int Lives => currentLives;
    public Vector2Int GridPosition => gridPosition;
    
    private void Awake()
    {
        brickRigidBody = GetComponent<Rigidbody2D>();
        brickCollider = GetComponent<Collider2D>();
        brickRenderer = GetComponent<SpriteRenderer>();
        brickType = BrickTypes.NORMAL;
    }

    private void Update()
    {
        //Make it more grey
        FadeOut();
    }

    #region Private Functions

    private void KnockOff(Vector3 source)
    {
        brickCollider.excludeLayers = LayerMask.GetMask("Default", "Brick");
        brickRigidBody.isKinematic = false;

        Vector3 knockBackForce = (transform.position - source).normalized;
        
        brickRigidBody.AddForce(knockBackForce * Random.Range(8, 22), ForceMode2D.Impulse);
        brickRigidBody.AddTorque(Random.Range(0.3f, 3), ForceMode2D.Impulse);
        
        isDestroyed = true;
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
        gameObject.SetActive(false);
    }
    
    private void PlayVFX()
    {
        GameObject destructionFX = objectPool.GetObject(destructionFXPrefab);
        destructionFX.transform.position = transform.position;
    }
    
    private void DestroyBrick()
    {
        Instantiate(breakFXPrefab, transform.position, quaternion.identity);
        
        isDestroyed = true;
        gameObject.SetActive(false);
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

    public void Initialize(Vector2Int gridPos, ObjectPool objPool, IEnumerable<Brick> neighboursCollection = null)
    {
        gridPosition = gridPos;
        objectPool = objPool;
        worldPosition = transform.position;
        
        if (neighboursCollection != null)
        {
            neighbours = neighboursCollection.ToList();
        }

        startColor = brickRenderer.color;
        breakColor += startColor;
        fadedColor *= startColor;
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
                brickRenderer.color = Color.yellow;
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

    #endregion
    
}
