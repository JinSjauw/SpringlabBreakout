using UnityEngine;

public struct GridCell
{
    public Vector2Int gridPosition;
    public Vector3 worldPosition;
    
    public GridCell(Vector2Int gridPos, Vector3 worldPos)
    {
        gridPosition = gridPos;
        worldPosition = worldPos;
    }
}
