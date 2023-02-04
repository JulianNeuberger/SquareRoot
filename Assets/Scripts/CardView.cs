using System.Collections.Generic;
using UnityEngine;

public class CardView : MonoBehaviour
{
    [SerializeField] private Card _card;
    [SerializeField] private SpriteRenderer _mainSprite;

    private int _rotation; // 0 is unrotated, each increment of 1 is 90° clockwise

    private bool[] hasOccupiedSocketAtWorldSideId = new bool[4]; // stored in world coordinates

    public void SetCard(Card card)
    {
        _card = card;
        _mainSprite.sprite = card.sprite;
    }

    public Card GetCard()
    {
        return _card;
    }

    public void SetRotation(int rotation)
    {
        _rotation = rotation;
        _mainSprite.transform.Rotate(new Vector3(0, 0, -90 * rotation));
    }

    public bool HasOpenSocketAtWorldSideId(int worldSideId)
    {
        Debug.Log($"HasOpenSocketAtWorldSideId: Rotation of CardView is {_rotation}");
        if(_card.HasSocketAtWorldSideId(worldSideId, _rotation) && !hasOccupiedSocketAtWorldSideId[worldSideId])
        {
            return true;
        }

        return false;
    }
}