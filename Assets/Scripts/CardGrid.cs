using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CardGrid : MonoBehaviour
{
    [SerializeField] private float cellSize = 1f;
    [SerializeField] private int widthCells = 50;
    [SerializeField] private int heightCells = 50;

    [SerializeField] private float nitrateChance = .1f;
    [SerializeField] private float waterChance = .2f;

    [SerializeField] private GridCell cellPrefab;
    [SerializeField] private CardView cardViewPrefab;
    [SerializeField] private ResourceManager resourceManager;

    [SerializeField] private Camera _camera;
    [SerializeField] private AudioManager _audioManager;

    [SerializeField] private int earthLevel = 25;

    [SerializeField] private TerrainType airTerrain;
    [SerializeField] private TerrainType earthTerrain;
    [SerializeField] private TerrainType nitrateTerrain;
    [SerializeField] private TerrainType waterTerrain;

    [SerializeField] private Card straightRoot;
    [SerializeField] private Card sapling;
    [SerializeField] private Card leaf;

    [SerializeField] private GameObject leafExchangeButtonContainer;
    [SerializeField] private Button leafExchangeButton;
    private CardView _leafSelectedForExchange;

    private GridCell[,] _grid;
    private List<Vector2Int> _gridPositionsWithActiveCardView = new List<Vector2Int>();
    private GridCell _highlightedCell;

    private GraphManager<CardView> _graph;

    public Vector2Int GridSize => new Vector2Int(widthCells, heightCells);
    
    #region UnityCallbacks

    protected void Awake()
    {
        PopulateGrid();
    }

    protected void Start()
    {
        leafExchangeButton.onClick.AddListener(() =>
        {
            Debug.Log("User clicked on leaf exchange button, exchanging resources");
            var success = resourceManager.ExchangeResourcesAtLeaf();
            if(success)
            {
                _audioManager.PlayExchangeAtLeafSound();
                _leafSelectedForExchange.Use();
            }
            leafExchangeButtonContainer.SetActive(false);
        });
    }

    protected void Update()
    {
        if (Input.GetKeyUp(KeyCode.Delete))
        {
            var cardView = GetCardViewUnderMouse();
            if (cardView == null) return;

            var gridPos = WorldCoordinatesToGridPosition(cardView.transform.position);
            DeleteCardAt(gridPos);
        }


        HandleLeafClicked();
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

        if(_graph != null)
        {
            Gizmos.color = Color.yellow;
            foreach (var node in _graph.Nodes)
            {
                foreach (var neighbour in node.neighbors)
                {
                    Gizmos.DrawLine(node.Value.transform.position, neighbour.Value.transform.position);
                }
            }
        }
    }

    #endregion

    #region PublicInterface

    public List<Vector2Int> GetAllGridPositionsWithActiveCardViews()
    {
        return _gridPositionsWithActiveCardView;
    }

    public void DeleteCardAt(Vector2Int gridPos)
    {
        var gridCell = GetGridCell(gridPos);
        if (gridCell == null) return;
        if (gridCell.GetActiveCardView() == null) return;

        _gridPositionsWithActiveCardView.Remove(gridCell.GetGridPosition());
        _graph.DeleteNode(gridCell.GetActiveCardView());
        
        Destroy(gridCell.GetActiveCardView().gameObject);
        gridCell.SetCardView(null);

        var toDelete = _graph.GetNodesNotConnectedToRoot();
        Debug.Log($"Removing {toDelete.Count} nodes");
        foreach (var node in toDelete)
        {
            _graph.DeleteNode(node.Value);
            
            var cell = GetGridCell(WorldCoordinatesToGridPosition(node.Value.transform.position));
            if (cell == null) continue;
            _gridPositionsWithActiveCardView.Remove(cell.GetGridPosition());
            if (cell.GetActiveCardView() == null) continue;
            
            Destroy(node.Value.gameObject);
            cell.SetCardView(null);
        }
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
        //Debug.Log($"WorldCoordinatesToGridPosition: worldPos {worldPos}");
        worldPos -= transform.position;
        worldPos += new Vector3(cellSize / 2f, cellSize / 2f);
        var gridPos = new Vector2Int((int) (worldPos.x / cellSize), (int) (worldPos.y / cellSize));

        //Debug.Log($"WorldCoordinatesToGridPosition: worldPos {worldPos} -> gridPos {gridPos}");
        return gridPos;
    }


    public Vector3 GridPositionToWorldCoordinates(Vector2Int gridPos)
    {
        var worldPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        worldPos += new Vector3(gridPos.x * cellSize, gridPos.y * cellSize);
        return worldPos;
    }

    public void HighlightCell(Vector2Int cellPos, Color color)
    {
        var cell = GetGridCell(cellPos);

        if (cell != _highlightedCell)
        {
            if(_highlightedCell != null) _highlightedCell.SetHighlighted(false, Color.white);
        }
        
        _highlightedCell = cell;
        if (cell == null) return;
        
        _highlightedCell.SetHighlighted(true, color);        
    }

    public void ResetHighlight()
    {
        if (_highlightedCell == null) return;
        _highlightedCell.SetHighlighted(false, Color.white);
        _highlightedCell = null;
    }

    public bool CanPlaceCard(Vector2Int gridPos, Card card, int rotation)
    {
        //Debug.Log($"Trying to place card {card.name} with rotation {rotation} at pos {gridPos}");

        if (!resourceManager.CanPayForCard(card))
        {
            // TODO: Better move this to start of dragging
            Debug.Log($"Not enough resources to pay for this card");
            return false;
        }

        if (GetGridCell(gridPos).GetActiveCardView() != null)
        {
            Debug.Log($"Can not place card here, there is already a {GetGridCell(gridPos).GetActiveCardView().name} here!");
            return false;
        }

        if(!card.CanPlaceOnTerrain(GetGridCell(gridPos).GetTerrain()))
        {
            Debug.Log($"Can not place card here, card {card.name} can not be placed on terrain {GetGridCell(gridPos).GetTerrain().name}.");
            return false;
        }

        bool connectingSocketAvailable = false;

        //card to place has socket at top side -> if there is a neighbor card above that one needs to have open socket at its bottom
        var neighborGridCell = GetNeighborAbove(gridPos);
        if (card.HasSocketAtWorldSideId(0, rotation))
        {
            //Debug.Log($"Card has socket at top side.");
            if (neighborGridCell != null && neighborGridCell.GetActiveCardView() != null)
            {
                //Debug.Log($"Found a neighbor above.");
                if (neighborGridCell.GetActiveCardView().HasOpenSocketAtWorldSideId(2) &&
                    neighborGridCell.GetActiveCardView().GetCard().CanAttachCardType(card))
                {
                    Debug.Log($"Found connecting socket available at neighbor above.");
                    connectingSocketAvailable = true;
                }
                else
                {
                    Debug.Log($"Can not place card here, card to place has socket at top, but neighbor above has no attachable open socket at bottom.");
                    return false;
                }
            }
        }
        //card to place has no socket at top side -> if there is a neighbor card above that one must not have an open socket at its bottom
        else
        {
            //Debug.Log($"Card has no socket at top side.");
            if (neighborGridCell != null && neighborGridCell.GetActiveCardView() != null)
            {
                //Debug.Log($"Found a neighbor above.");
                if (neighborGridCell.GetActiveCardView().HasOpenSocketAtWorldSideId(2))
                {
                    Debug.Log(
                        $"Can not place card here, card to place has no socket at top, but neighbor above has an open socket at bottom.");
                    return false;
                }
            }
        }

        //card to place has socket at right side -> if there is a neighbor card right that one needs to have open socket at its left
        neighborGridCell = GetNeighborRight(gridPos);
        if (card.HasSocketAtWorldSideId(1, rotation))
        {
            //Debug.Log($"Card has socket at right side.");
            if (neighborGridCell != null && neighborGridCell.GetActiveCardView() != null)
            {
                //Debug.Log($"Found a neighbor right.");
                if (neighborGridCell.GetActiveCardView().HasOpenSocketAtWorldSideId(3) &&
                    neighborGridCell.GetActiveCardView().GetCard().CanAttachCardType(card))
                {
                    Debug.Log($"Found connecting socket available at neighbor right.");
                    connectingSocketAvailable = true;
                }
                else
                {
                    Debug.Log($"Can not place card here, card to place has socket at right, but neighbor right has no attachable open socket at left.");
                    return false;
                }
            }
        }
        //card to place has no socket at right side -> if there is a neighbor card right that one must not have an open socket at its left
        else
        {
            //Debug.Log($"Card has no socket at right side.");
            if (neighborGridCell != null && neighborGridCell.GetActiveCardView() != null)
            {
                //Debug.Log($"Found a neighbor right.");
                if (neighborGridCell.GetActiveCardView().HasOpenSocketAtWorldSideId(3))
                {
                    Debug.Log(
                        $"Can not place card here, card to place has no socket at right, but neighbor right has an open socket at left.");
                    return false;
                }
            }
        }

        neighborGridCell = GetNeighborBelow(gridPos);
        //card to place has socket at bottom side -> if there is a neighbor card below that one needs to have open socket at its top
        if (card.HasSocketAtWorldSideId(2, rotation))
        {
            //Debug.Log($"Card has socket at bottom side.");
            if (neighborGridCell != null && neighborGridCell.GetActiveCardView() != null)
            {
                //Debug.Log($"Found a neighbor below.");
                if (neighborGridCell.GetActiveCardView().HasOpenSocketAtWorldSideId(0) &&
                    neighborGridCell.GetActiveCardView().GetCard().CanAttachCardType(card))
                {
                    Debug.Log($"Found connecting socket available at neighbor below.");
                    connectingSocketAvailable = true;
                }
                else
                {
                    Debug.Log($"Can not place card here, card to place has socket at bottom, but neighbor below has no attachable open socket at top.");
                    return false;
                }
            }
        }
        //card to place has no socket at bottom side -> if there is a neighbor card below that one must not have an open socket at its top
        else
        {
            //Debug.Log($"Card has no socket at bottom side.");
            if (neighborGridCell != null && neighborGridCell.GetActiveCardView() != null)
            {
                //Debug.Log($"Found a neighbor below.");
                if (neighborGridCell.GetActiveCardView().HasOpenSocketAtWorldSideId(0))
                {
                    Debug.Log(
                        $"Can not place card here, card to place has no socket at bottom, but neighbor below has an open socket at top.");
                    return false;
                }
            }
        }

        //card to place has socket at left side -> if there is a neighbor card left that one needs to have open socket at its right
        neighborGridCell = GetNeighborLeft(gridPos);
        if (card.HasSocketAtWorldSideId(3, rotation))
        {
            //Debug.Log($"Card has socket at left side.");
            if (neighborGridCell != null && neighborGridCell.GetActiveCardView() != null)
            {
                //Debug.Log($"Found a neighbor left.");
                if (neighborGridCell.GetActiveCardView().HasOpenSocketAtWorldSideId(1) &&
                    neighborGridCell.GetActiveCardView().GetCard().CanAttachCardType(card))
                {
                    Debug.Log($"Found connecting socket available at neighbor left.");
                    connectingSocketAvailable = true;
                }
                else
                {
                    Debug.Log($"Can not place card here, card to place has socket at left, but neighbor left has no attachable open socket at right.");
                    return false;
                }
            }
        }
        //card to place has no socket at left side -> if there is a neighbor card left that one must not have an open socket at its right
        else
        {
            //Debug.Log($"Card has no socket at left side.");
            if (neighborGridCell != null && neighborGridCell.GetActiveCardView() != null)
            {
                //Debug.Log($"Found a neighbor left.");
                if (neighborGridCell.GetActiveCardView().HasOpenSocketAtWorldSideId(1))
                {
                    Debug.Log(
                        $"Can not place card here, card to place has no socket at left, but neighbor left has an open socket at right.");
                    return false;
                }
            }
        }
        
        if (!connectingSocketAvailable)
        {
            Debug.Log($"Can not place card here, did not find any available connecting sockets.");
            return false;
        }
        
        Debug.Log($"Can place card! All checks passed and we found connecting sockets available.");
        return true;
    }


    /// <summary>
    /// tries to place a card at given grid position. Returns true if successful, false otherwise.
    /// If force is set, we dont check if the card can be placed, useful to spawn the starter plant.
    /// </summary>
    public bool TryPlaceCard(Vector2Int gridPos, Card card, int rotation, bool force = false)
    {
        Debug.Log($"Try placing card at gridPos {gridPos}");
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

        var cardView = Instantiate(cardViewPrefab, transform);
        cardView.transform.position = GridPositionToWorldCoordinates(gridPos);
        cardView.SetRotation(rotation);
        cardView.SetCard(card);
        cardView.OnDeath = _ => DeleteCardAt(gridPos);
        _grid[gridPos.x, gridPos.y].SetCardView(cardView);
        if (!_gridPositionsWithActiveCardView.Contains(gridPos))
        {
            _gridPositionsWithActiveCardView.Add(gridPos);
        }

        if (_graph == null)
        {
            // assume the first plant part is the root
            _graph = new GraphManager<CardView>(cardView);
        }
        else
        {
            // add this plant to the connected plant parts
            _graph.AddNode(cardView);

            foreach(var neighbour in GetNeighbors(gridPos))
            {
                if (neighbour.GetActiveCardView() == null) continue;
                _graph.Connect(cardView, neighbour.GetActiveCardView());
            }
        }

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
        Debug.Log($"GetNeighbors returning {neighbors.Count} neighbors");

        return neighbors;
    }

    public void PlaceStarter(int gridPosX)
    {
        TryPlaceCard(new Vector2Int(gridPosX, earthLevel), sapling, rotation: 0, force: true);
        TryPlaceCard(new Vector2Int(gridPosX, earthLevel - 1), straightRoot, rotation: 0, force: true);
        TryPlaceCard(new Vector2Int(gridPosX, earthLevel + 1), leaf, rotation: 1, force: true);
        resourceManager.UpdateUpkeep();
        resourceManager.UpdateResourceIncome();
    }

    #endregion

    #region PrivateMethods

    private void HandleLeafClicked()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var cardView = GetCardViewUnderMouse();
            if (cardView == null || cardView.GetCard() != leaf || !cardView.CanBeUsed())
            {
                leafExchangeButtonContainer.SetActive(false);
                return;
            }

            _audioManager.PlayClickOnUsableLeafSound();
            _leafSelectedForExchange = cardView;

            var gridPos = WorldCoordinatesToGridPosition(cardView.transform.position);
            var screenPos = _camera.WorldToScreenPoint(cardView.transform.position);

            Debug.Log($"Leaf clicked at position {gridPos}. Setting button to world position of cardView {cardView.transform.position} which is screen point {screenPos}");

            leafExchangeButtonContainer.transform.position = screenPos;
            leafExchangeButtonContainer.SetActive(true);
        }
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
                cell.StoreGridPosition(new Vector2Int(x, y));
                cell.SetTerrain(terrainType);
                cell.transform.localPosition = new Vector3(x * cellSize, y * cellSize, 0f);

                _grid[x, y] = cell;
            }
        }

        for (var x = 0; x < widthCells; ++x)
        {
            for (var y = 0; y < heightCells; ++y)
            {
                if (_grid[x, y].GetTerrain() != earthTerrain) continue;
                
                var shouldSpawnNitrate = Random.value < nitrateChance;
                if (shouldSpawnNitrate)
                {
                    _grid[x, y].SetTerrain(nitrateTerrain);
                    continue;
                }

                var shouldSpawnWater = Random.value < waterChance;
                if (shouldSpawnWater)
                {
                    _grid[x, y].SetTerrain(waterTerrain);
                    continue;
                }
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

    private CardView GetCardViewUnderMouse()
    {
        var ray = _camera.ScreenPointToRay(Input.mousePosition);
        var hit = Physics2D.Raycast(ray.origin, ray.direction, 10f);
        if (!hit) return null;
        
        var target = hit.collider.gameObject;
        return target.GetComponent<CardView>();
    }

    #endregion
}

#if UNITY_EDITOR
[CustomEditor(typeof(CardGrid))]
class CardGridEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var grid = (CardGrid) target;

        if (GUILayout.Button("Place Starter at 0,0"))
        {
            grid.PlaceStarter(grid.WorldCoordinatesToGridPosition(new Vector3(0, 0, 0)).x);
        }
    }
}
#endif