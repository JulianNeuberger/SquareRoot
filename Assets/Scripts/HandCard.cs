using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class HandCard : MonoBehaviour
{
    [SerializeField] private Card card;
    [SerializeField] private SpriteRenderer mainSprite;
    [SerializeField] private SpriteRenderer borderSprite;

    private Collider2D _collider2D;

    public Collider2D Collider2D => _collider2D;

    protected void Awake()
    {
        _collider2D = GetComponent<Collider2D>();
    }

    public Card Card
    {
        get => card;
        set => SetCard(value);
    }

    public void SetSortingOrder(int index)
    {
        mainSprite.sortingOrder = index;
        borderSprite.sortingOrder = index;
    }

    public void RotateSprite(Vector3 eulers)
    {
        mainSprite.transform.Rotate(eulers);
    }

    private void SetCard(Card newCard)
    {
        this.card = newCard;
        mainSprite.sprite = card.sprite;
    }
}