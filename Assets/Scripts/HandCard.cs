using System;
using UnityEngine;
using UnityEngine.UI;

public class HandCard : MonoBehaviour
{
    [SerializeField] private Card card;
    [SerializeField] private Image mainSprite;

    [HideInInspector] public RectTransform rectTransform;
    
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public Card Card
    {
        get => card;
        set => SetCard(value);
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