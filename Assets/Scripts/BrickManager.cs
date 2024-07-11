using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BrickManager : MonoBehaviour
{
    private Dictionary<Vector2Int, Brick> bricksDictionary;
    private List<Brick> activeBricks;

    private ObjectPool objectPool;
    
    // Start is called before the first frame update
    void Start()
    {
        objectPool = FindObjectOfType<ObjectPool>();
        
        bricksDictionary = GridManager.Instance.SpawnLevel();
        InitializeBricks();
    }
    
    private void InitializeBricks()
    {
        activeBricks = new List<Brick>();
        List<Brick> normalBricksList = bricksDictionary.Values.ToList();
        
        //Designate share of random brick types
        SetBrickTypes(GetRandomBricks(ref normalBricksList, .25f), BrickTypes.EXPLOSIVE);
        SetBrickTypes(GetRandomBricks(ref normalBricksList, .1f), BrickTypes.SUPEREXPLOSIVE);
        SetBrickTypes(GetRandomBricks(ref normalBricksList, .4f), BrickTypes.POWERUP);
        
        foreach (KeyValuePair<Vector2Int, Brick> brickData in bricksDictionary)
        {
            Vector2Int cellPosition = brickData.Key;
            Brick spawnedBrick = brickData.Value;
            
            if (spawnedBrick.BrickType != BrickTypes.NORMAL)
            {
                spawnedBrick.Initialize(cellPosition, objectPool, GridManager.Instance.GetNeighbours(spawnedBrick, spawnedBrick.BrickType == BrickTypes.SUPEREXPLOSIVE ? 4 : 2, spawnedBrick.BrickType == BrickTypes.EXPLOSIVE));
            }
            else
            {
                spawnedBrick.Initialize(cellPosition, objectPool);
            }
            
            activeBricks.Add(spawnedBrick);
        }
    }
    
    private List<Brick> GetRandomBricks( ref List<Brick> bricksList, float percentage)
    {
        List<Brick> result = new List<Brick>();
        
        int amountToGet = (int)(bricksList.Count * percentage);

        if (amountToGet <= 0)
        {
            Debug.Log("Nothing Left In the List!");
            return result;
        }
        
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
}