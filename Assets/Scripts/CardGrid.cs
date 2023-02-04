using System;
using System.Collections.Generic;
using UnityEditor;
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

    [SerializeField] private Card straightRoot;

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


    public bool CanPlaceCard(Vector2Int gridPos, Card card, int rotation)
    {
        var activeCard = _grid[gridPos.x, gridPos.y].GetActiveCardView();
        if (activeCard != null)
        {
            Debug.Log($"Can not place card here, there is already a {activeCard.name} here!");
            // there is already an active card here
            return false;
        }

        bool connectingSocketAvailable = false;


        //card to place has socket at top side -> if there is a neighbor card above that one needs to have open socket at its bottom
        if (card.HasSocketAtWorldSideId(0, rotation))
        {
            if (GetNeighborAbove(gridPos) != null && GetNeighborAbove(gridPos).GetActiveCardView() != null)
            {
                if (GetNeighborAbove(gridPos).GetActiveCardView().HasOpenSocketAtWorldSideId(2))
                {
                    Debug.Log($"Found connecting socket available at neighbor above.");
                    connectingSocketAvailable = true;
                }
                else
                {
                    Debug.Log($"Can not place card here, card to place has socket at top, but neighbor above has no open socket at bottom.");
                    return false;
                }
            }
        }
        //card to place has no socket at top side -> if there is a neighbor card above that one must not have an open socket at its bottom
        else
        {
            if (GetNeighborAbove(gridPos) != null && GetNeighborAbove(gridPos).GetActiveCardView() != null)
            {
                if (GetNeighborAbove(gridPos).GetActiveCardView().HasOpenSocketAtWorldSideId(2))
                {
                    Debug.Log($"Can not place card here, card to place has no socket at top, but neighbor above has an open socket at bottom.");
                    return false;
                }
            }
        }

        //card to place has socket at right side -> if there is a neighbor card right that one needs to have open socket at its left
        if (card.HasSocketAtWorldSideId(1, rotation))
        {
            if (GetNeighborRight(gridPos) != null && GetNeighborRight(gridPos).GetActiveCardView() != null)
            {
                if (GetNeighborRight(gridPos).GetActiveCardView().HasOpenSocketAtWorldSideId(3))
                {
                    Debug.Log($"Found connecting socket available at neighbor right.");
                    connectingSocketAvailable = true;
                }
                else
                {
                    Debug.Log($"Can not place card here, card to place has socket at right, but neighbor right has no open socket at left.");
                    return false;
                }
            }
        }
        //card to place has no socket at right side -> if there is a neighbor card right that one must not have an open socket at its left
        else
        {
            if (GetNeighborRight(gridPos) != null && GetNeighborRight(gridPos).GetActiveCardView() != null)
            {
                if (GetNeighborRight(gridPos).GetActiveCardView().HasOpenSocketAtWorldSideId(3))
                {
                    Debug.Log($"Can not place card here, card to place has no socket at right, but neighbor right has an open socket at left.");
                    return false;
                }
            }
        }

        //card to place has socket at bottom side -> if there is a neighbor card below that one needs to have open socket at its top
        if (card.HasSocketAtWorldSideId(2, rotation))
        {
            if (GetNeighborBelow(gridPos) != null && GetNeighborBelow(gridPos).GetActiveCardView() != null)
            {
                if (GetNeighborBelow(gridPos).GetActiveCardView().HasOpenSocketAtWorldSideId(0))
                {
                    Debug.Log($"Found connecting socket available at neighbor below.");
                    connectingSocketAvailable = true;
                }
                else
                {
                    Debug.Log($"Can not place card here, card to place has socket at bottom, but neighbor below has no open socket at top.");
                    return false;
                }
            }
        }
        //card to place has no socket at bottom side -> if there is a neighbor card below that one must not have an open socket at its top
        else
        {
            if (GetNeighborBelow(gridPos) != null && GetNeighborBelow(gridPos).GetActiveCardView() != null)
            {
                if (GetNeighborBelow(gridPos).GetActiveCardView().HasOpenSocketAtWorldSideId(0))
                {
                    Debug.Log($"Can not place card here, card to place has no socket at bottom, but neighbor below has an open socket at top.");
                    return false;
                }
            }
        }

        //card to place has socket at left side -> if there is a neighbor card left that one needs to have open socket at its right
        if (card.HasSocketAtWorldSideId(3, rotation))
        {
            if (GetNeighborLeft(gridPos) != null && GetNeighborLeft(gridPos).GetActiveCardView() != null)
            {
                if (GetNeighborLeft(gridPos).GetActiveCardView().HasOpenSocketAtWorldSideId(1))
                {
                    Debug.Log($"Found connecting socket available at neighbor left.");
                    connectingSocketAvailable = true;
                }
                else
                {
                    Debug.Log($"Can not place card here, card to place has socket at left, but neighbor left has no open socket at right.");
                    return false;
                }
            }
        }
        //card to place has no socket at left side -> if there is a neighbor card left that one must not have an open socket at its right
        else
        {
            if (GetNeighborLeft(gridPos) != null && GetNeighborLeft(gridPos).GetActiveCardView() != null)
            {
                if (GetNeighborLeft(gridPos).GetActiveCardView().HasOpenSocketAtWorldSideId(1))
                {
                    Debug.Log($"Can not place card here, card to place has no socket at left, but neighbor left has an open socket at right.");
                    return false;
                }
            }
        }


        if(connectingSocketAvailable)
        {
            Debug.Log($"Can place card! All checks passed and we found connecting sockets available.");
            return true;
        }
        
        Debug.Log($"Can not place card here, did not find any available connecting sockets.");
        return false;
    }
    

    /// <summary>
    /// tries to place a card at given grid position. Returns true if successful, false otherwise.
    /// If force is set, we dont check if the card can be placed, useful to spawn the starter plant.
    /// </summary>
    public bool TryPlaceCard(Vector2Int gridPos, Card card, int rotation, bool force = false)
    {
        Debug.Log("Try placing card...");
        if (rotation < 0) throw new ArgumentException("Rotation must be between 0 and 3 (inclusive)");
        if (rotation > 3) throw new ArgumentException("Rotation must be between 0 and 3 (inclusive)");

        Debug.Log("Rotation ok");
        if (!force)
        {
            if (!CanPlaceCard(gridPos, card, rotation))
            {
                return false;
            }            
        }

        Debug.Log("Checks out, placing card!");
        var cardView = Instantiate(cardViewPrefab, transform);
        cardView.transform.position = GridPositionToWorldCoordinates(gridPos);
        cardView.Card = card;
        _grid[gridPos.x, gridPos.y].SetCardView(cardView);
        
        return true;
    }


    public List<GridCell> GetNeighbors(Vector2Int gridPos)
    {
        var neighbors = new List<GridCell>();

        if (GetNeighborAbove(gridPos) != null)
        {
            neighbors.Add(GetNeighborAbove(gridPos));
        }

        if (GetNeighborRight(gridPos) != null)
        {
            neighbors.Add(GetNeighborRight(gridPos));
        }

        if (GetNeighborBelow(gridPos) != null)
        {
            neighbors.Add(GetNeighborBelow(gridPos));
        }

        if (GetNeighborLeft(gridPos) != null)
        {
            neighbors.Add(GetNeighborLeft(gridPos));
        }

        return neighbors;
    }
    
    public void PlaceStarter(Vector3 worldPos)
    {
        var gridPos = WorldCoordinatesToGridPosition(worldPos);
        TryPlaceCard(gridPos, straightRoot, rotation: 0, force: true);
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

    private GridCell GetNeighborAbove(Vector2Int gridPos)
    {
        return GetGridCell(new Vector2Int(gridPos.x, gridPos.y + 1));
    }

    private GridCell GetNeighborRight(Vector2Int gridPos)
    {
        return GetGridCell(new Vector2Int(gridPos.x + 1, gridPos.y));
    }

    private GridCell GetNeighborBelow(Vector2Int gridPos)
    {
        return GetGridCell(new Vector2Int(gridPos.x, gridPos.y - 1));
    }

    private GridCell GetNeighborLeft(Vector2Int gridPos)
    {
        return GetGridCell(new Vector2Int(gridPos.x - 1, gridPos.y));
    }

    #endregion
}

[CustomEditor(typeof(CardGrid))]
class CardGridEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var grid = (CardGrid) target;
        
        if (GUILayout.Button("Place Starter at 0,0"))
        {
            grid.PlaceStarter(new Vector3(0, 0, 0));
        }
    }
}