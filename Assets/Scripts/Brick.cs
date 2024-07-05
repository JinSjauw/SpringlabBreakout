using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Brick : MonoBehaviour, IBrick
{
    [SerializeField] private bool willHitNeighbours;
    [SerializeField] private int maxLives;
    [SerializeField] private BrickTypes brickType;

    private SpriteRenderer renderer;
    
    private Vector2Int gridPosition;
    [SerializeField] private int currentLives;
    
    [SerializeField] private List<Brick> neighbours;
    
    public IEnumerable<IBrick> Neighbours { get => neighbours; }
    public bool WillHitNeighboursOnDeath { get => willHitNeighbours; }
    public int Lives { get => currentLives; }
    public Vector2Int GridPosition { get => gridPosition; }
    
    private void Awake()
    {
        renderer = GetComponent<SpriteRenderer>();
        //currentLives = maxLives;
    }
    
    public void Initialize(Vector2Int positionOnGrid, BrickTypes type, IEnumerable<Brick> neighboursCollection = null)
    {
        gridPosition = positionOnGrid;
        brickType = type;

        if (neighboursCollection != null)
        {
            neighbours = neighboursCollection.ToList();
        }
        
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
        
        if (brickType == BrickTypes.NORMAL && gameObject.activeSelf)
        {
            Debug.Log("Hit! " + currentLives + " ID: " + gridPosition);
        }
        
        if (currentLives > 0)
        {
            GetComponent<SpriteRenderer>().color = Color.Lerp(Color.red, Color.white, currentLives / (float)maxLives);
        }
        else if(gameObject.activeSelf)
        {
            gameObject.SetActive(false);

            if (willHitNeighbours)
            {
                foreach (IBrick brick in BrickResolver.ResolveBricksToDestroy(this)) { }
            }
        }
    }
}
