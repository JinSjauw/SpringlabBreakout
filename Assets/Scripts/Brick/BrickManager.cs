using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BrickManager : MonoBehaviour
{
    private Dictionary<Vector2Int, Brick> bricksDictionary;
    [SerializeField] private List<Brick> activeBricks;

    private ObjectPool objectPool;
    
    // Start is called before the first frame update
    void Start()
    {
        objectPool = FindObjectOfType<ObjectPool>();
        bricksDictionary = new Dictionary<Vector2Int, Brick>();

        StartCoroutine(SpawnLevel());
    }
    
    private void InitializeBricks()
    {
        activeBricks = new List<Brick>();
        List<Brick> normalBricksList = bricksDictionary.Values.ToList();
        
        //Designate grid % share of random brick types
        SetBrickTypes(GetRandomBricks(ref normalBricksList, .1f), BrickTypes.EXPLOSIVE);
        SetBrickTypes(GetRandomBricks(ref normalBricksList, .05f), BrickTypes.SUPEREXPLOSIVE);
        SetBrickTypes(GetRandomBricks(ref normalBricksList, .3f), BrickTypes.POWERUP);
        
        foreach (KeyValuePair<Vector2Int, Brick> brickData in bricksDictionary)
        {
            Vector2Int cellPosition = brickData.Key;
            Brick spawnedBrick = brickData.Value;

            float spawnDelay = Random.Range(0.0f, 1.5f);
            
            if (spawnedBrick.BrickType != BrickTypes.NORMAL)
            {
                spawnedBrick.Initialize(cellPosition, objectPool, spawnDelay,
                    GridManager.Instance.GetNeighbours(spawnedBrick, spawnedBrick.BrickType == BrickTypes.SUPEREXPLOSIVE ? 3 : 1, spawnedBrick.BrickType == BrickTypes.EXPLOSIVE));
                spawnedBrick.BrickExplodedEvent += BrickExplosion;
            }
            else
            {
                spawnedBrick.Initialize(cellPosition, objectPool, spawnDelay);
            }

            spawnedBrick.BrickDestroyedEvent += RemoveBrick;
            
            activeBricks.Add(spawnedBrick);
        }
    }

    private IEnumerator SpawnLevel()
    {
        foreach (Brick brick in bricksDictionary.Values)
        {
            Destroy(brick);   
        }
        
        bricksDictionary.Clear();

        yield return new WaitForSeconds(2);
        
        bricksDictionary = GridManager.Instance.SpawnLevel();
        InitializeBricks();
    }
    
    private void BrickExplosion(object sender, Vector3 source)
    {
        //Nudge the spring component of all bricks.
        foreach (Brick brick in activeBricks)
        {
            Vector3 direction = brick.transform.position - source;
            Vector3 pushForce = direction.normalized * 1.15f;
            brick.Push(pushForce.x, pushForce.y);
        }
    }

    private void RemoveBrick(object sender, Brick brick)
    {
        if (activeBricks.Contains(brick))
        {
            activeBricks.Remove(brick);
            
            if (activeBricks.Count <= 0)
            {
                StartCoroutine(SpawnLevel());
            }
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