using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class GridCell : MonoBehaviour
{
    private CardView _activeCardView;
    private TerrainType _terrain;
    private Vector2Int _gridPosition;
    private float _visibility;
    private int _terrainResourceAmount;
    
    public float VisibilityCounter
    {
        get => _visibility;
        set
        {
            _visibility = value;
            UpdateVisibility();
        }
    }

    [SerializeField] private SpriteRenderer backgroundSprite;
    [SerializeField] private TerrainType air;
    
    private void Awake()
    {
        UpdateVisibility();
    }

    public void SetTerrain(TerrainType type, int initialTerrainResourceAmount)
    {
        _terrain = type;
        _terrainResourceAmount = initialTerrainResourceAmount;

        var totalWeights = _terrain.SpriteTable.Select(e => e.weight).Sum();

        var randomValue = Random.value;

        Sprite useSprite = null;
        var threshold = 0f;
        foreach (var spriteEntry in _terrain.SpriteTable)
        {
            threshold += spriteEntry.weight / totalWeights;
            if (randomValue < threshold)
            {
                useSprite = spriteEntry.sprite;
                break;
            }
        }

        if (useSprite == null) throw new ArgumentException("Should not happen");
        
        backgroundSprite.sprite = useSprite;
        
        UpdateVisibility();
    }

    public TerrainType GetTerrain()
    {
        return _terrain;
    }

    public CardView GetActiveCardView()
    {
        return _activeCardView;
    }

    public void SetCardView(CardView card)
    {
        _activeCardView = card;
    }

    public void StoreGridPosition(Vector2Int gridPos)
    {
        _gridPosition = gridPos;
    }

    public Vector2Int GetGridPosition()
    {
        return _gridPosition;
    }

    //returns true if resource reservoir now depleted
    public bool reduceResourceReservoir()
    {
        if(_terrainResourceAmount > 0)
        {
            _terrainResourceAmount--;
            if(_terrainResourceAmount == 0)
            {
                return true;
            }
        }
        return false;
    }

    private void UpdateVisibility()
    {
        if (_terrain == air)
        {
            backgroundSprite.color = Color.white;
            return;
        }
        
        var color = Color.white;
        if (_visibility < 1f)
        {
            color = Color.Lerp(Color.black, Color.white, Mathf.Clamp01(_visibility));
        }

        backgroundSprite.color = color;
    }
}
