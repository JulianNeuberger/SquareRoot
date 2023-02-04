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
        activeCardSprite.sprite = card.Card.sprite;
    }
}
