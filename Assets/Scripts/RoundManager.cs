using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    [SerializeField] private AudioManager audioManager;
    [SerializeField] private GameObject hud;
    [SerializeField] private GameObject title;
    [SerializeField] private GameObject tutorial;


    private void Start()
    {
        endRoundButton.onClick.AddListener(() =>
        {
            Debug.Log("User clicked end round button, calling NextRound");
            NextRound();
        });
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log("User pressed enter, calling NextRound");
            NextRound();
        }
    }

    public void StartGame()
    {
        DrawCards();
        grid.PlaceStarter(grid.GridSize.x / 2);
        hud.SetActive(true);
        title.SetActive(false);
        resourceManager.UpdateAmountsDisplay();
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OpenMenu()
    {
        tutorial.SetActive(true);
        hud.SetActive(false);
    }

    public void CloseMenu()
    {
        tutorial.SetActive(false);
        hud.SetActive(true);
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
        grid.SpawnNewResources();
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