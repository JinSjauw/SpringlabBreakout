using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that generates and manages the grid.
/// </summary>
public class GridManager : MonoBehaviour
{
    [SerializeField] private Vector2Int gridSize;
    [SerializeField] private Vector2 offset;
    [SerializeField] private Vector2 brickSize;
    
    [SerializeField] private GameObject brickPrefab;

    public static GridManager Instance;
    
    private GridCell[,] currentGrid;
    private Dictionary<Vector2Int, Brick> brickDictionary;
    private List<Brick> activeBricks;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;   
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

            if (brickDictionary.ContainsKey(neighbourPosition))
            {
                yield return brickDictionary[neighbourPosition];
            }
        }
        
        //West
        for (int i = 0; i < range; i++)
        {
            Vector2Int neighbourPosition = gridPosition + Vector2Int.left * (i + 1);

            if (brickDictionary.ContainsKey(neighbourPosition))
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

            if (brickDictionary.ContainsKey(neighbourPosition))
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

            if (brickDictionary.ContainsKey(neighbourPosition))
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
        for (int i = 0; i <= range; i++)
        {
            Vector2Int neighbourPosition = gridPosition + (Vector2Int.left + Vector2Int.down) * (i + 1);

            if (brickDictionary.ContainsKey(neighbourPosition))
            {
                yield return brickDictionary[neighbourPosition];
            }
        }
        
        //North East
        for (int i = 0; i <= range; i++)
        {
            Vector2Int neighbourPosition = gridPosition + (Vector2Int.right + Vector2Int.down) * (i + 1);

            if (brickDictionary.ContainsKey(neighbourPosition))
            {
                yield return brickDictionary[neighbourPosition];
            }
        }
        
        //South West
        for (int i = 0; i <= range; i++)
        {
            Vector2Int neighbourPosition = gridPosition + (Vector2Int.left + Vector2Int.up) * (i + 1);

            if (brickDictionary.ContainsKey(neighbourPosition))
            {
                yield return brickDictionary[neighbourPosition];
            }
        }
        
        //South East
        for (int i = 0; i <= range; i++)
        {
            Vector2Int neighbourPosition = gridPosition + (Vector2Int.right + Vector2Int.up) * (i + 1);

            if (brickDictionary.ContainsKey(neighbourPosition))
            {
                yield return brickDictionary[neighbourPosition];
            }
        }
    }
    private void GenerateGrid()
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
    
    public IEnumerable<Brick> GetNeighbours(Brick source, int range = 1, bool diagonal = false)
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
    
    public Dictionary<Vector2Int, Brick> SpawnLevel()
    {
        GenerateGrid();
        return brickDictionary;
    }
}
