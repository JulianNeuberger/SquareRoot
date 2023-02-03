using UnityEngine;

public class CardView : MonoBehaviour
{
    [SerializeField] private Card card;
    [SerializeField] private SpriteRenderer mainSprite;

    public Card Card => card;
}
