using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandCard : MonoBehaviour
{
    [SerializeField] private Card card;
    [SerializeField] private Image mainSprite;

    [SerializeField] private RectTransform upkeepIcon;
    [SerializeField] private RectTransform incomeIcon;
    [SerializeField] private RectTransform tradeIcon;

    [SerializeField] private RectTransform sugarIcon;
    [SerializeField] private RectTransform waterIcon;
    [SerializeField] private RectTransform nitrateIcon;

    [SerializeField] private RectTransform costPanel;
    [SerializeField] private RectTransform effectsPanel;
    [SerializeField] private RectTransform effectContainer;

    [SerializeField] private List<Card> tradableCards = new();

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

        AddResourceIcon(nitrateIcon, card.nitrateCost, costPanel);
        AddResourceIcon(waterIcon, card.waterCost, costPanel);
        AddResourceIcon(sugarIcon, card.sugarCost, costPanel);

        if (tradableCards.Contains(card)) AddTradeIcon();
        if (card.gatherMultiplierOnResource > 0) AddIncomePanel();
        if (card.nitrateUpkeep > 0 || card.sugarUpkeep > 0 || card.waterUpkeep > 0) AddUpkeepPanel();
    }

    private void AddResourceIcon(RectTransform icon, int amount, RectTransform container)
    {
        for (var i = 0; i < amount; ++i)
        {
            Instantiate(icon, container);
        }
    }

    private void AddIncomePanel()
    {
        var container = Instantiate(effectContainer, effectsPanel);
        for (var i = 0; i < card.gatherMultiplierOnResource; i++)
        {
            Instantiate(incomeIcon, container);
        }
    }

    private void AddUpkeepPanel()
    {
        var container = Instantiate(effectContainer, effectsPanel);
        Instantiate(upkeepIcon, container);
        AddResourceIcon(nitrateIcon, (int) card.nitrateUpkeep, container);
        AddResourceIcon(waterIcon, (int) card.waterUpkeep, container);
        AddResourceIcon(sugarIcon, (int) card.sugarUpkeep, container);
    }

    private void AddTradeIcon()
    {
        var container = Instantiate(effectContainer, effectsPanel);
        Instantiate(tradeIcon, container);
    }

    private void ResetColor()
    {
        mainSprite.color = _originalSpriteColor;
        _isCurrentlyRed = false;
    }
}