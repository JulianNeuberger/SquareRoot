using System.Collections.Generic;
using UnityEngine;

public class CardView : MonoBehaviour
{
    [SerializeField] private Card card;
    [SerializeField] private SpriteRenderer mainSprite;

    private int _rotation; // 0 is unrotated, each increment of 1 is 90° clockwise

    private bool[] hasOccupiedSocketAtWorldSideId = new bool[4]; // stored in world coordinates

    public Card Card => card;

    public Card GetCard()
    {
        return card;
    }

    public bool HasOpenSocketAtWorldSideId(int worldSideId)
    {
        if(card.HasSocketAtWorldSideId(worldSideId, _rotation) && !hasOccupiedSocketAtWorldSideId[worldSideId])
        {
            return true;
        }
        return false;
    }
}
