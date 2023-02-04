using UnityEditor;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    [SerializeField] private ResourceManager resourceManager;
    [SerializeField] private Hand hand;
    [SerializeField] private Deck redrawPile;
    [SerializeField] private Deck discardPile;

    [SerializeField] private int cardsPerTurn = 5;

    public void NextRound()
    {
        DiscardHandCards();
        DrawCards();
        GatherResources();
        PayUpkeep();
    }

    private void DiscardHandCards()
    {
        foreach (var card in hand.cards)
        {
            discardPile.PlaceCard(card);
        }

        hand.Clear();
    }

    private void DrawCards()
    {
        for (var i = 0; i < cardsPerTurn; ++i)
        {
            redrawPile.DrawCard();
        }
    }

    private void GatherResources()
    {
        resourceManager.GatherAllResources();
    }

    private void PayUpkeep()
    {
        // TODO
    }
}

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