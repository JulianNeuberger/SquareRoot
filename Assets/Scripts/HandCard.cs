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

    private void SetCard(Card newCard)
    {
        this.card = newCard;
        mainSprite.sprite = card.sprite;
    }
}
