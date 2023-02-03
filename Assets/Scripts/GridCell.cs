using UnityEngine;

public class GridCell : MonoBehaviour
{
    private CardView _activeCardView;
    private TerrainType _terrain;

    [SerializeField] private SpriteRenderer backgroundSprite;
    [SerializeField] private SpriteRenderer activeCardSprite;

    public void SetTerrain(TerrainType type)
    {
        _terrain = type;
        backgroundSprite.sprite = _terrain.Sprite;
    }
}
