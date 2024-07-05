using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GridManager : MonoBehaviour
{
    [SerializeField] private Vector2Int gridSize;
    [SerializeField] private Vector2 offset;
    [SerializeField] private Vector2 brickSize;
    
    [SerializeField] private GameObject brickPrefab;

    private GridCell[,] currentGrid;
    private Dictionary<Vector2Int, Brick> brickList;
    
    private void Awake()
    {
        GenerateGrid();
    }

    private void Start()
    {
        InitializeBricks();
    }

    private void SpawnBrick()
    {
        //Handle the brick spawning here
    }

    private IEnumerable<Brick> GetNeighbours(Brick source, int range = 1)
    {

        foreach (Brick neighbour in GetStraightNeighbours(source, range))
        {
            yield return neighbour;
        }
        
        
        
        /*Vector2Int gridPosition = source.GridPosition;
        //Get NWSE neighbours

        //North
        for (int i = 0; i < range; i++)
        {
            Vector2Int neighbourPosition = gridPosition + Vector2Int.up * (i + 1);

            if (neighbourPosition.y >= 0 && brickList.ContainsKey(neighbourPosition))
            {
                yield return brickList[neighbourPosition];
            }
        }

        //West
        for (int i = 0; i < range; i++)
        {
            Vector2Int neighbourPosition = gridPosition + Vector2Int.left * (i + 1);

            if (neighbourPosition.x >= 0 && brickList.ContainsKey(neighbourPosition))
            {
                yield return brickList[neighbourPosition];
            }
        }

        //South
        for (int i = 0; i < range; i++)
        {
            if (gridSize.y < range)
            {
                break;
            }

            Vector2Int neighbourPosition = gridPosition + Vector2Int.down * (i + 1);

            if (neighbourPosition.y <= gridSize.y && brickList.ContainsKey(neighbourPosition))
            {
                yield return brickList[neighbourPosition];
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

            if (neighbourPosition.x <= gridSize.x && brickList.ContainsKey(neighbourPosition))
            {
                yield return brickList[neighbourPosition];
            }
        }*/
    }

    private IEnumerable<Brick> GetStraightNeighbours(Brick source, int range = 1)
    {
        Vector2Int gridPosition = source.GridPosition;
        //Get NWSE neighbours
        
        //North
        for (int i = 0; i < range; i++)
        {
            Vector2Int neighbourPosition = gridPosition + Vector2Int.up * (i + 1);

            if (neighbourPosition.y >= 0 && brickList.ContainsKey(neighbourPosition))
            {
                yield return brickList[neighbourPosition];
            }
        }
        
        //West
        for (int i = 0; i < range; i++)
        {
            Vector2Int neighbourPosition = gridPosition + Vector2Int.left * (i + 1);

            if (neighbourPosition.x >= 0 && brickList.ContainsKey(neighbourPosition))
            {
                yield return brickList[neighbourPosition];
            }
        }
        
        //South
        for (int i = 0; i < range; i++)
        {
            if (gridSize.y < range)
            {
                break;
            }
            
            Vector2Int neighbourPosition = gridPosition + Vector2Int.down * (i + 1);

            if (neighbourPosition.y <= gridSize.y && brickList.ContainsKey(neighbourPosition))
            {
                yield return brickList[neighbourPosition];
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

            if (neighbourPosition.x <= gridSize.x && brickList.ContainsKey(neighbourPosition))
            {
                yield return brickList[neighbourPosition];
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
            Vector2Int neighbourPosition = gridPosition + Vector2Int.up * (i + 1);

            if (neighbourPosition.y >= 0 && brickList.ContainsKey(neighbourPosition))
            {
                yield return brickList[neighbourPosition];
            }
        }
        
        //North East
        for (int i = 0; i < range; i++)
        {
            Vector2Int neighbourPosition = gridPosition + Vector2Int.left * (i + 1);

            if (neighbourPosition.x >= 0 && brickList.ContainsKey(neighbourPosition))
            {
                yield return brickList[neighbourPosition];
            }
        }
        
        //South West
       
        //South East
    }
    
    public void GenerateGrid()
    {
        currentGrid = new GridCell[gridSize.x, gridSize.y];
        brickList = new Dictionary<Vector2Int, Brick>();
        
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                GridCell newCell = new GridCell(new Vector2Int(x, y), transform.position + new Vector3(((gridSize.x - 1) * .5f - x) * (offset.x + brickSize.x),
                                -y * (offset.y + brickSize.y),
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
            
                brickList.Add(newCell.gridPosition, spawnedBrick);
            }
        }
    }

    public void InitializeBricks()
    {
        int amountExplosive = 0;
        int amountMaxExplosive = 5;
        
        foreach (KeyValuePair<Vector2Int, Brick> brickData in brickList)
        {
            Vector2Int cellPosition = brickData.Key;
            Brick spawnedBrick = brickData.Value;
            
            Array brickTypeArray = Enum.GetValues(typeof(BrickTypes));

            //Need to rethink spawning in random types of bricks.
            
            BrickTypes randomBrickType = (BrickTypes)brickTypeArray.GetValue(Random.Range(0, brickTypeArray.Length - 1));

            if (randomBrickType != BrickTypes.NORMAL)
            {
                amountExplosive++;
            }

            if (amountMaxExplosive <= amountExplosive)
            {
                randomBrickType = BrickTypes.NORMAL;
            }
            
            //For each brick that is explosive get their neighbours
            
            if (randomBrickType != BrickTypes.NORMAL)
            {
                spawnedBrick.Initialize(cellPosition, randomBrickType, GetNeighbours(spawnedBrick, randomBrickType == BrickTypes.SUPEREXPLOSIVE ? 2 : 1));
            }
            else
            {
                spawnedBrick.Initialize(cellPosition, randomBrickType);
            }
        }
    }
    
    
}
