using UnityEngine;

public class GridCell : MonoBehaviour
{
    private Card _activeCard;
    private TerrainType _terrain;

    [SerializeField] private SpriteRenderer backgroundSprite;
    [SerializeField] private SpriteRenderer activeCardSprite;

    public void SetTerrain(TerrainType type)
    {
        _terrain = type;
        backgroundSprite.sprite = _terrain.Sprite;
    }
}
