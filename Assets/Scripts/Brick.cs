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
    
    private SpriteRenderer renderer;
    private Color startColor;
    
    public BrickTypes BrickType => brickType;
    public IEnumerable<IBrick> Neighbours => neighbours;
    public bool WillHitNeighboursOnDeath => willHitNeighbours;
    public int Lives => currentLives;
    public Vector2Int GridPosition => gridPosition;
    public bool IsDestroyed => isDestroyed;
    
    private void Awake()
    {
        renderer = GetComponent<SpriteRenderer>();
        brickType = BrickTypes.NORMAL;
    }
    
    public void Initialize(Vector2Int positionOnGrid, IEnumerable<Brick> neighboursCollection = null)
    {
        gridPosition = positionOnGrid;
        //brickType = type;

        if (neighboursCollection != null)
        {
            neighbours = neighboursCollection.ToList();
        }

        startColor = renderer.color;
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
                renderer.color = Color.white;
                break;
            case BrickTypes.EXPLOSIVE:
                renderer.color = Color.blue;
                willHitNeighbours = true;
                break;
            case BrickTypes.SUPEREXPLOSIVE:
                renderer.color = Color.yellow;
                willHitNeighbours = true;
                break;
            default:
                renderer.color = Color.green;
                break;
        }
    }

    public void OnResolveHit()
    {
        currentLives--;
        
        /*if (brickType == BrickTypes.NORMAL && gameObject.activeSelf)
        {
            Debug.Log("Hit! " + currentLives + " ID: " + gridPosition);
        }*/
        
        if (currentLives > 0)
        {
            GetComponent<SpriteRenderer>().color = Color.Lerp(Color.red, startColor, currentLives / (float)maxLives);
        }
        else if(gameObject.activeSelf)
        {
            gameObject.SetActive(false);

            isDestroyed = true;
            /*if (willHitNeighbours)
            {
                foreach (IBrick brick in BrickResolver.ResolveBricksToDestroy(this)) { }
            }*/
        }
    }
}
