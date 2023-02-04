using System;
using System.Collections.Generic;
using UnityEngine;

public class CardGrid : MonoBehaviour
{
    [SerializeField] private float cellSize = 1f;
    [SerializeField] private int widthCells = 50;
    [SerializeField] private int heightCells = 50;

    [SerializeField] private GridCell cellPrefab;
    [SerializeField] private CardView cardViewPrefab;

    [SerializeField] private int earthLevel = 25;

    [SerializeField] private TerrainType airTerrain;
    [SerializeField] private TerrainType earthTerrain;

    private GridCell[,] _grid;


    #region UnityCallbacks

    protected void Awake()
    {
        PopulateGrid();
    }

    protected void OnDrawGizmosSelected()
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
        var worldPos = transform.position;
        var offset = new Vector3(-cellSize / 2f, earthLevel * cellSize - cellSize / 2f);
        var start = worldPos + offset;
        var end = worldPos + offset + new Vector3(widthCells * cellSize, 0, 0);
        Gizmos.DrawLine(start, end);
    }

    #endregion

    #region PublicInterface

    public List<Vector2Int> GetAllGridPositionsWithActiveCardViews()
    {
        // TODO IMPLEMENT
        return new List<Vector2Int>();
    }

    public GridCell GetGridCell(Vector2Int gridPos)
    {
        if (gridPos.x < 0) return null;
        if (gridPos.x >= widthCells) return null;
        if (gridPos.y < 0) return null;
        if (gridPos.y > heightCells) return null;

        return _grid[gridPos.x, gridPos.y];
    }


    public Vector2Int WorldCoordinatesToGridPosition(Vector3 worldPos)
    {
        worldPos -= transform.position;
        var gridPos = new Vector2Int((int) (worldPos.x / cellSize), (int) (worldPos.y / cellSize));
        return gridPos;
    }

    public Vector3 GridPositionToWorldCoordinates(Vector2Int gridPos)
    {
        var worldPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        worldPos += new Vector3(gridPos.x * cellSize, gridPos.y * cellSize);
        return worldPos;
    }

    public bool CanPlaceCard(Vector2Int gridPos, Card card)
    {
        if (_grid[gridPos.x, gridPos.y].GetActiveCardView() != null)
        {
            // there is already an active card here
            return false;
        }

        var neighbors = GetNeighbors(gridPos);

        foreach (var neighbor in neighbors)
        {
            if (neighbor.GetActiveCardView() == null) continue;
            if (!neighbor.GetActiveCardView().HasOpenSockets()) continue;
            if (!neighbor.GetActiveCardView().GetCard().CanAttachCardType(card)) continue;

            return true;
        }

        return false;
    }

    /// <summary>
    /// tries to place a card at given grid position. Returns true if successful, false otherwise.
    /// </summary>
    public bool TryPlaceCard(Vector2Int gridPos, Card card, int rotation)
    {
        if (rotation < 0) throw new ArgumentException("Rotation must be between 0 and 3 (inclusive)");
        if (rotation > 3) throw new ArgumentException("Rotation must be between 0 and 3 (inclusive)");
        
        if (!CanPlaceCard(gridPos, card)) return false;

        var cardView = Instantiate(cardViewPrefab, transform);
        cardView.transform.position = GridPositionToWorldCoordinates(gridPos);
        _grid[gridPos.x, gridPos.y].SetCardView(cardView);
        
        return true;
    }

    public List<GridCell> GetNeighbors(Vector2Int gridPos)
    {
        var neighbors = new List<GridCell>();

        var gridCellLeft = GetGridCell(new Vector2Int(gridPos.x - 1, gridPos.y));
        if (gridCellLeft != null)
        {
            neighbors.Add(gridCellLeft);
        }

        var gridCellBelow = GetGridCell(new Vector2Int(gridPos.x, gridPos.y - 1));
        if (gridCellBelow != null)
        {
            neighbors.Add(gridCellBelow);
        }

        var gridCellRight = GetGridCell(new Vector2Int(gridPos.x + 1, gridPos.y));
        if (gridCellRight != null)
        {
            neighbors.Add(gridCellRight);
        }

        var gridCellAbove = GetGridCell(new Vector2Int(gridPos.x, gridPos.y + 1));
        if (gridCellAbove != null)
        {
            neighbors.Add(gridCellAbove);
        }

        return neighbors;
    }

    #endregion

    #region PrivateMethods

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

    #endregion
}