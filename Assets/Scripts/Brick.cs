using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Brick : MonoBehaviour, IBrick
{
    [SerializeField] private int maxLives;
    [SerializeField] private BrickTypes brickType;
    
    //IBrick fields
    private List<Brick> neighbours;
    private bool willHitNeighbours;
    private int currentLives;
    
    private Vector2Int gridPosition;
    private bool isDestroyed;
    
    private Rigidbody2D brickRigidBody;
    private Collider2D brickCollider;
    private SpriteRenderer brickRenderer;
    private Color startColor;
    
    public BrickTypes BrickType => brickType;
    public IEnumerable<IBrick> Neighbours => neighbours;
    public bool WillHitNeighboursOnDeath => willHitNeighbours;
    public int Lives => currentLives;
    public Vector2Int GridPosition => gridPosition;
    public bool IsDestroyed => isDestroyed;
    
    private void Awake()
    {
        brickRigidBody = GetComponent<Rigidbody2D>();
        brickCollider = GetComponent<Collider2D>();
        brickRenderer = GetComponent<SpriteRenderer>();
        brickType = BrickTypes.NORMAL;
    }

    private void KnockOff()
    {
        brickCollider.excludeLayers = LayerMask.GetMask("Default", "Brick");
        brickRigidBody.isKinematic = false;
        brickRigidBody.AddForce(Vector2.up * 5);
    }
    
    public void Initialize(Vector2Int positionOnGrid, IEnumerable<Brick> neighboursCollection = null)
    {
        gridPosition = positionOnGrid;
        //brickType = type;

        if (neighboursCollection != null)
        {
            neighbours = neighboursCollection.ToList();
        }

        startColor = brickRenderer.color;
    }
    public void SetBrickType(BrickTypes type)
    {
        brickType = type;
        
        switch (brickType)
        {
            case BrickTypes.NORMAL:
                currentLives = maxLives;
                break;
            case BrickTypes.EXPLOSIVE:
                currentLives = maxLives / 2;
                break;
            case BrickTypes.SUPEREXPLOSIVE:
                currentLives = 0;
                break;
        }
        
        switch (brickType)
        {
            case BrickTypes.NORMAL:
                brickRenderer.color = Color.white;
                break;
            case BrickTypes.EXPLOSIVE:
                brickRenderer.color = Color.blue;
                willHitNeighbours = true;
                break;
            case BrickTypes.SUPEREXPLOSIVE:
                brickRenderer.color = Color.yellow;
                willHitNeighbours = true;
                break;
            default:
                brickRenderer.color = Color.green;
                break;
        }
    }
    public void OnResolveHit()
    {
        currentLives--;
        
        if (currentLives > 0)
        {
            GetComponent<SpriteRenderer>().color = Color.Lerp(Color.red, startColor, currentLives / (float)maxLives);
        }
        else if(gameObject.activeSelf)
        {
            if (brickType == BrickTypes.NORMAL)
            {
                KnockOff();
            }
            else
            {
                gameObject.SetActive(false);
            }
            isDestroyed = true;
        }
    }
}
