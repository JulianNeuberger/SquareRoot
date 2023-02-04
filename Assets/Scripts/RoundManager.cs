using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundManager : MonoBehaviour
{

    private ResourceManager resourceManager;
    [SerializeField] private Hand hand;
    [SerializeField] private Deck redrawPile;
    [SerializeField] private Deck discardPile;

    private void Awake()
    {
        resourceManager = GetComponent<ResourceManager>();
    }
    
    public void StartRound()
    {
        var gatheredResources = resourceManager.GatherAllResources();

        Debug.Log($"Gathered resources: {gatheredResources}");

        // TODO Add gathered resources to spendable resource counts

        // TODO Pay upkeep from the spendable resources
        

        // TODO Check for forced actions (e.g. not enough upkeep -> destroy plant parts)

        
    }

    public void EndRound()
    {
        foreach (var card in hand.cards)
        {
            discardPile.PlaceCard(card);
        }
        
        hand.Clear();
    }
}
