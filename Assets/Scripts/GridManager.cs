using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private Vector2Int gridSize;
    [SerializeField] private Vector2 offset;
    [SerializeField] private Vector2 brickSize;
    
    [SerializeField] private GameObject brickPrefab;

    private GridCell[,] currentGrid; 
    
    private void Awake()
    {
        GenerateGrid();
    }

    private void Start()
    {
        PopulateGrid();
    }

    public void GenerateGrid()
    {
        currentGrid = new GridCell[gridSize.x, gridSize.y];
        
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                currentGrid[x, y] = new GridCell(new Vector2Int(x, y), transform.position + new Vector3(((gridSize.x - 1) * .5f - x) * (offset.x + brickSize.x),
                        -y * (offset.y + brickSize.y),
                        0));
            }
        }
    }

    public void PopulateGrid()
    {
        foreach (GridCell position in currentGrid)
        {
            GameObject brickObject = Instantiate(brickPrefab, transform);
            Brick brick = brickObject.GetComponent<Brick>();
        
            if (brick == null)
            {
                Destroy(brickObject);
                Debug.Log("Brick Component not found!");
                return;
            }

            Transform brickTransform = brick.transform;
            brickTransform.position = position.worldPosition;
            brickTransform.localScale = brickSize;
            
        }
    }
    
    
}
