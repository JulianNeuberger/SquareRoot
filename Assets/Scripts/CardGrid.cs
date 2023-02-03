using UnityEngine;

public class CardGrid : MonoBehaviour
{
    [SerializeField] private float cellSize = 1f;
    [SerializeField] private int widthCells = 50;
    [SerializeField] private int heightCells = 50;

    [SerializeField] private GridCell cellPrefab;
    
    [SerializeField] private int earthLevel = 25;

    [SerializeField] private TerrainType airTerrain;
    [SerializeField] private TerrainType earthTerrain;

    private GridCell[,] _grid;

    protected void Awake()
    {
        PopulateGrid();
    }

    private void PopulateGrid()
    {
        _grid = new GridCell[widthCells, heightCells];

        for (var x = 0; x < widthCells; ++x)
        {
            for (var y = 0; y < heightCells; ++y)
            {
                var terrainType = y < earthLevel ? earthTerrain : airTerrain;
                var cell = Instantiate(cellPrefab, transform);
                cell.SetTerrain(terrainType);
                cell.transform.localPosition = new Vector3(x * cellSize, y * cellSize, 0f);
                
                _grid[x, y] = cell;
            }
        }
    }

    private void OnDrawGizmos()
    {
        for (var x = 0; x < widthCells; ++x)
        {
            for (var y = 0; y < heightCells; ++y)
            {
                var center = transform.position + new Vector3(x * cellSize, y * cellSize, 0f);
                var size = new Vector3(cellSize, cellSize, 0f);
                Gizmos.DrawWireCube(center, size);
            }
        }
        
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + new Vector3(-cellSize / 2f, earthLevel * cellSize - cellSize / 2f), 
            transform.position + new Vector3(widthCells - cellSize / 2f, earthLevel * cellSize - cellSize / 2f));
    }
}