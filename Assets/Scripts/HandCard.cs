using UnityEngine;

public class HandCard : MonoBehaviour
{
    [SerializeField] private Card card;
    [SerializeField] private SpriteRenderer mainSprite;
    [SerializeField] private SpriteRenderer borderSprite;

    public Card Card {
        get => card;
        set => SetCard(value);
    }

    public void SetSortingOrder(int index)
    {
        mainSprite.sortingOrder = index;
        borderSprite.sortingOrder = index;
    }
    
    private void SetCard(Card newCard)
    {
        this.card = newCard;
        mainSprite.sprite = card.sprite;
    }
}
