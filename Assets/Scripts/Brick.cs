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
    private int currentLives;
    
    [SerializeField] private List<Brick> neighbours;
    
    public IEnumerable<IBrick> Neighbours { get => neighbours; }
    public bool WillHitNeighboursOnDeath { get => willHitNeighbours; }
    public int Lives { get => currentLives; }
    public Vector2Int GridPosition { get => gridPosition; }

    public event EventHandler<Brick> OnDestruction; 
    
    private void Awake()
    {
        renderer = GetComponent<SpriteRenderer>();
        //currentLives = maxLives;
    }

    private void OnNeighbourDestoyed(object sender, Brick e)
    {
        if (neighbours.Contains(e))
        {
            neighbours.Remove(e);
        }
    }
    
    public void Initialize(Vector2Int positionOnGrid, BrickTypes type, IEnumerable<Brick> neighboursCollection = null)
    {
        gridPosition = positionOnGrid;
        brickType = type;
        currentLives = brickType == BrickTypes.NORMAL ? maxLives : 0;


        if (neighboursCollection != null)
        {
            neighbours = neighboursCollection.ToList();
            
            foreach (Brick neighbour in neighbours)
            {
                neighbour.OnDestruction += OnNeighbourDestoyed;
            }
        }
        
        //Observer pattern.
        //When Brick dies remove it from neighbours list.
        
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
        Debug.Log(transform.position);
        if (currentLives > 0)
        {
            currentLives--;
            GetComponent<SpriteRenderer>().color = Color.Lerp(Color.red, Color.white, currentLives / (float)maxLives);
        }
        else if(gameObject.activeSelf)
        {
            //OnDestruction?.Invoke(this, this);
            gameObject.SetActive(false);
        }
    }
}
