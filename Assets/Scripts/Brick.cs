using System;
using System.Collections.Generic;
using UnityEngine;

public class Brick : MonoBehaviour, IBrick
{
    [SerializeField] private bool willHitNeighbours;
    [SerializeField] private int maxLives;
    [SerializeField] private BrickTypes brickType;

    private Vector2Int gridPosition;
    private int currentLives;
    
    public IEnumerable<IBrick> Neighbours { get; }
    public bool WillHitNeighboursOnDeath { get => willHitNeighbours; }
    public int Lives { get => currentLives; }
    public Vector2Int GridPosition { get => gridPosition; }

    private void Awake()
    {
        currentLives = maxLives;
    }

    public void Initialize(Vector2Int positionOnGrid, IEnumerable<IBrick> neighboursList, BrickTypes type)
    {
        gridPosition = positionOnGrid;
        brickType = type;
        currentLives = maxLives;
    }
    
    public void OnResolveHit()
    {
        Debug.Log(transform.position);
        if (currentLives > 0)
        {
            currentLives--;
            GetComponent<SpriteRenderer>().color = Color.Lerp(Color.red, Color.white, currentLives / (float)maxLives);
        }
        else
        {
            Destroy(gameObject);

        }
    }
}
