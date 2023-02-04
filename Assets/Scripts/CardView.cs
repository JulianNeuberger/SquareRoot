using System;
using System.Collections.Generic;
using UnityEngine;

public class CardView : MonoBehaviour
{
    public Action<CardView> OnDeath;
    
    [SerializeField] private Card _card;
    [SerializeField] private SpriteRenderer _mainSprite;

    private int _rotation; // 0 is unrotated, each increment of 1 is 90° clockwise

    private bool[] _hasOccupiedSocketAtWorldSideId = new bool[4]; // stored in world coordinates

    private int _currentHealth;

    public void SetCard(Card card)
    {
        _card = card;
        _mainSprite.sprite = card.sprite;
        _currentHealth = card.initialHealth;
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
        //Debug.Log($"HasOpenSocketAtWorldSideId: Rotation of CardView is {_rotation}");
        if(_card.HasSocketAtWorldSideId(worldSideId, _rotation) && !_hasOccupiedSocketAtWorldSideId[worldSideId])
        {
            return true;
        }

        return false;
    }

    public void DecreaseHealth()
    {
        _currentHealth--;

        _mainSprite.sprite = _card.unhealthySprite;

        if(_currentHealth == 0)
        {
            OnDeath?.Invoke(this);
        }
    }

    public int GetCurrentHealth()
    {
        return _currentHealth;
    }
}