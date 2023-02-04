using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GridCell : MonoBehaviour
{
    private CardView _activeCardView;
    private TerrainType _terrain;
    private Vector2Int _gridPosition;

    [SerializeField] private SpriteRenderer backgroundSprite;

    public void SetTerrain(TerrainType type)
    {
        _terrain = type;
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
}
