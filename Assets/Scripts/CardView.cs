using System.Collections.Generic;
using UnityEngine;

public class CardView : MonoBehaviour
{
    [SerializeField] private Card card;
    [SerializeField] private SpriteRenderer mainSprite;
    private int _occupiedSockets = 0;
    
    public Card Card => card;

    public Card GetCard()
    {
        return card;
    }

    public bool HasOpenSockets()
    {
        if (_occupiedSockets < card.numberOfSockets)
        {
            return true;
        }
        return false;
    }
}
