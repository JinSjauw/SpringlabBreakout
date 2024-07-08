using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GridManager : MonoBehaviour
{
    [SerializeField] private Vector2Int gridSize;
    [SerializeField] private Vector2 offset;
    [SerializeField] private Vector2 brickSize;
    
    [SerializeField] private GameObject brickPrefab;

    private GridCell[,] currentGrid;
    private Dictionary<Vector2Int, Brick> brickDictionary;
    
    private void Awake()
    {
        GenerateGrid();
    }

    private void Start()
    {
        InitializeBricks();
    }

    //Get a list of random instantiated bricks.
    private List<Brick> GetRandomBricks( ref List<Brick> bricksList, float percentage)
    {
        List<Brick> result = new List<Brick>();
        
        int amountToGet = (int)(bricksList.Count * percentage);

        if (amountToGet <= 0)
        {
            Debug.Log("Nothing Left In the List!");
            return result;
        }
        
        Debug.Log("Getting Random Bricks! " + " Amount to get: " + amountToGet + " Brick count: " + bricksList.Count + " Percentage: " + percentage);
        
        for (int i = 0; i < amountToGet; i++)
        {
            Brick selectedBrick = bricksList[Random.Range(0, bricksList.Count - 1)];
            
            result.Add(selectedBrick);
            bricksList.Remove(selectedBrick);
        }
        
        return result;
    }

    private void SetBrickTypes(List<Brick> bricksToSet, BrickTypes type)
    {
        foreach (Brick brick in bricksToSet)
        {
            brick.SetBrickType(type);
        }
    }
    

    private IEnumerable<Brick> GetNeighbours(Brick source, int range = 1, bool diagonal = false)
    {
        if (diagonal)
        {
            foreach (Brick neighbour in GetDiagonalNeighbours(source, range))
            {
                yield return neighbour;
            }
        }
        else
        {
            foreach (Brick neighbour in GetStraightNeighbours(source, range))
            {
                yield return neighbour;
            }
        }
    }
    private IEnumerable<Brick> GetStraightNeighbours(Brick source, int range = 1)
    {
        Vector2Int gridPosition = source.GridPosition;
        //Get NWSE neighbours
        
        //North
        for (int i = 0; i < range; i++)
        {
            Vector2Int neighbourPosition = gridPosition + Vector2Int.down * (i + 1);

            if (neighbourPosition.y >= 0 && brickDictionary.ContainsKey(neighbourPosition))
            {
                yield return brickDictionary[neighbourPosition];
            }
        }
        
        //West
        for (int i = 0; i < range; i++)
        {
            Vector2Int neighbourPosition = gridPosition + Vector2Int.left * (i + 1);

            if (neighbourPosition.x >= 0 && brickDictionary.ContainsKey(neighbourPosition))
            {
                yield return brickDictionary[neighbourPosition];
            }
        }
        
        //South
        for (int i = 0; i < range; i++)
        {
            if (gridSize.y < range)
            {
                break;
            }
            
            Vector2Int neighbourPosition = gridPosition + Vector2Int.up * (i + 1);

            if (neighbourPosition.y <= gridSize.y && brickDictionary.ContainsKey(neighbourPosition))
            {
                yield return brickDictionary[neighbourPosition];
            }
        }
        
        //East
        for (int i = 0; i < range; i++)
        {
            if (gridSize.x < range)
            {
                break;
            }
            
            Vector2Int neighbourPosition = gridPosition + Vector2Int.right * (i + 1);

            if (neighbourPosition.x <= gridSize.x && brickDictionary.ContainsKey(neighbourPosition))
            {
                yield return brickDictionary[neighbourPosition];
            }
        }
    }
    private IEnumerable<Brick> GetDiagonalNeighbours(Brick source, int range = 1)
    {
        Vector2Int gridPosition = source.GridPosition;
        //Get NWSE neighbours
        
        //North West
        for (int i = 0; i < range; i++)
        {
            Vector2Int neighbourPosition = gridPosition + (Vector2Int.right + Vector2Int.down) * (i + 1);

            if (neighbourPosition.y >= 0 && neighbourPosition.x >= 0 && brickDictionary.ContainsKey(neighbourPosition))
            {
                yield return brickDictionary[neighbourPosition];
            }
        }
        
        //North East
        for (int i = 0; i < range; i++)
        {
            Vector2Int neighbourPosition = gridPosition + (Vector2Int.left + Vector2Int.down) * (i + 1);

            if (neighbourPosition.y <= 0 && neighbourPosition.x <= gridSize.x && brickDictionary.ContainsKey(neighbourPosition))
            {
                yield return brickDictionary[neighbourPosition];
            }
        }
        
        //South West
        for (int i = 0; i < range; i++)
        {
            Vector2Int neighbourPosition = gridPosition + (Vector2Int.right + Vector2Int.up) * (i + 1);

            if (neighbourPosition.y <= gridSize.y && neighbourPosition.x >= 0 && brickDictionary.ContainsKey(neighbourPosition))
            {
                yield return brickDictionary[neighbourPosition];
            }
        }
        
        //South East
        for (int i = 0; i < range; i++)
        {
            Vector2Int neighbourPosition = gridPosition + (Vector2Int.left + Vector2Int.up) * (i + 1);

            if (neighbourPosition.y <= gridSize.y && neighbourPosition.x <= gridSize.x && brickDictionary.ContainsKey(neighbourPosition))
            {
                yield return brickDictionary[neighbourPosition];
            }
        }
    }
    
    public void GenerateGrid()
    {
        //Delete last objects before making new ones
        currentGrid = new GridCell[gridSize.x, gridSize.y];
        brickDictionary = new Dictionary<Vector2Int, Brick>();
        
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                GridCell newCell = new GridCell(new Vector2Int(x, y), transform.position - new Vector3(((gridSize.x - 1) * .5f - x) * (offset.x + brickSize.x),
                                y * (offset.y + brickSize.y),
                                0));

                currentGrid[x, y] = newCell;
                
                GameObject brickObject = Instantiate(brickPrefab, transform);
                Brick spawnedBrick = brickObject.GetComponent<Brick>();
            
                if (spawnedBrick == null)
                {
                    Destroy(brickObject);
                    Debug.Log("Brick Component not found!");
                    return;
                }

                Transform brickTransform = spawnedBrick.transform;
                brickTransform.name = "Brick: " + newCell.gridPosition;
                brickTransform.position = newCell.worldPosition;
                brickTransform.localScale = brickSize;
            
                brickDictionary.Add(newCell.gridPosition, spawnedBrick);
            }
        }
    }

    public void InitializeBricks()
    {
        int amountExplosive = 0;
        int amountMaxExplosive = 5;


        List<Brick> normalBricksList = brickDictionary.Values.ToList();
        
        //Designate random brick types
        SetBrickTypes(GetRandomBricks(ref normalBricksList, .2f), BrickTypes.EXPLOSIVE);
        SetBrickTypes(GetRandomBricks(ref normalBricksList, .1f), BrickTypes.SUPEREXPLOSIVE);
        //SetBrickTypes(GetRandomBricks(ref normalBricksList, .4f), BrickTypes.POWERUP);
        
        foreach (KeyValuePair<Vector2Int, Brick> brickData in brickDictionary)
        {
            Vector2Int cellPosition = brickData.Key;
            Brick spawnedBrick = brickData.Value;
            
            if (spawnedBrick.BrickType != BrickTypes.NORMAL)
            {
                spawnedBrick.Initialize(cellPosition, GetNeighbours(spawnedBrick, spawnedBrick.BrickType == BrickTypes.SUPEREXPLOSIVE ? 4 : 2, spawnedBrick.BrickType == BrickTypes.EXPLOSIVE));
            }
            else
            {
                spawnedBrick.Initialize(cellPosition);
            }
        }
    }
    
    
}
