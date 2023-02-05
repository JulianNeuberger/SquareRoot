using System;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class RoundManager : MonoBehaviour
{
    [SerializeField] private Button endRoundButton;

    [SerializeField] private ResourceManager resourceManager;
    [SerializeField] private Hand hand;
    [SerializeField] private Deck redrawPile;
    [SerializeField] private Deck discardPile;
    [SerializeField] private CardGrid grid;

    [SerializeField] private int cardsPerTurn = 5;

    [SerializeField] private TextMeshProUGUI deckDisplay;

    [SerializeField] private AudioManager audioManager;


    private void Start()
    {
        endRoundButton.onClick.AddListener(() =>
        {
            Debug.Log("User clicked end round button, calling NextRound");
            NextRound();
        });


        StartGame();
    }

    public void StartGame()
    {
        DrawCards();
        grid.PlaceStarter(grid.GridSize.x / 2);
        deckDisplay.text = $"Deck: {redrawPile.CardsRemaining()}, Discard: {discardPile.CardsRemaining()}";
    }
    
    public void NextRound()
    {
        audioManager.PlayEndRoundSound();
        UnUseAllCardViews();
        DiscardHandCards();
        ReshuffleDeck();
        DrawCards();
        resourceManager.UpdateResourceIncome();
        resourceManager.UpdateUpkeep();
        resourceManager.ReceiveResourceIncome();
        resourceManager.PayUpkeep();

        deckDisplay.text = $"Deck: {redrawPile.CardsRemaining()}, Discard: {discardPile.CardsRemaining()}";
    }


    private void UnUseAllCardViews()
    {
        var allActiveCardViewsGridPositions = grid.GetAllGridPositionsWithActiveCardViews();
        foreach(var gridPos in allActiveCardViewsGridPositions)
        {
            grid.GetGridCell(gridPos).GetActiveCardView().UnUse();
        }
    }

    private void DiscardHandCards()
    {
        foreach (var handCard in hand.Current)
        {
            Debug.Log($"Discarding card {handCard.Card.name}");
            discardPile.PlaceCard(handCard.Card);
        }

        hand.Clear();
    }

    private void ReshuffleDeck()
    {
        if (redrawPile.CardsRemaining() >= cardsPerTurn) return;
        if (discardPile.CardsRemaining() <= 0) return;     
        
        redrawPile.AddManyCards(discardPile.Cards);
        discardPile.Clear();
    }

    private void DrawCards()
    {
        var cardsToDraw = Math.Min(cardsPerTurn, redrawPile.CardsRemaining());
        for (var i = 0; i < cardsToDraw; ++i)
        {
            hand.DealCard(redrawPile.DrawCard());
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(RoundManager))]
public class RoundManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Next Round"))
        {
            var roundManager = (RoundManager) target;
            roundManager.NextRound();
        }
    }
}
#endif