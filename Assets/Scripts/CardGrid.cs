using System.Collections.Generic;
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


    public List<(int, int)> GetAllGridPositionsWithActiveCardViews()
    {
        // TODO IMPLEMENT
        return new List<(int, int)>();
    }

    public GridCell GetGridCell(int x, int y)
    {
        if(x >= 0 && x < widthCells && y >= 0 && y <= heightCells) {
            return _grid[x, y];
        }
        return null;
    }


    public (int, int) WorldCoordinatesToGridPosition(float x, float y)
    {
        int gridX = (int)System.Math.Floor(x / cellSize);
        int gridY = (int)System.Math.Floor(y / cellSize);
        return (gridX, gridY);
    }


    public bool CanPlaceCard(int x, int y, Card card)
    {
        if(_grid[x, y].GetActiveCardView() != null)
        {
            // there is already an active card here
            return false;
        }

        var neighbors = GetNeighbors(x, y);

        foreach(var neighbor in neighbors)
        {
            if (neighbor.GetActiveCardView().HasOpenSockets() && neighbor.GetActiveCardView().GetCard().CanAttachCardType(card.cardType))
            {
                return true;
            }
        }

        return false;
    }

    public List<GridCell> GetNeighbors(int x, int y)
    {
        var neighbors = new List<GridCell>();

        var gridCellLeft = GetGridCell(x - 1, y);
        if(gridCellLeft != null)
        {
            neighbors.Add(gridCellLeft);
        }

        var gridCellBelow = GetGridCell(x, y - 1);
        if (gridCellBelow != null)
        {
            neighbors.Add(gridCellBelow);
        }

        var gridCellRight = GetGridCell(x + 1, y);
        if (gridCellRight != null)
        {
            neighbors.Add(gridCellRight);
        }

        var gridCellAbove = GetGridCell(x, y + 1);
        if (gridCellAbove != null)
        {
            neighbors.Add(gridCellAbove);
        }

        return neighbors;
    }



}