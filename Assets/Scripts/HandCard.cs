using System;
using UnityEngine;
using UnityEngine.UI;

public class HandCard : MonoBehaviour
{
    [SerializeField] private Card card;
    [SerializeField] private Image mainSprite;

    [HideInInspector] public RectTransform rectTransform;

    private bool _isCurrentlyRed = false;
    private Color _originalSpriteColor;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public Card Card
    {
        get => card;
        set => SetCard(value);
    }

    public void FlashRed()
    {
        _originalSpriteColor = mainSprite.color;
        mainSprite.color = new Color(1, 0.2f, 0.2f);
        _isCurrentlyRed = true;
        Invoke(nameof(ResetColor), 0.2f);
    }

    private void SetCard(Card newCard)
    {
        this.card = newCard;
        mainSprite.sprite = card.sprite;
    }

    private void ResetColor()
    {
        mainSprite.color = _originalSpriteColor;
        _isCurrentlyRed = false;
    }
}