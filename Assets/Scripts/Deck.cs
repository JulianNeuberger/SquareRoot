using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Deck : MonoBehaviour
{
    [SerializeField] private List<DeckEntry> contents = new();

    public void PlaceCard(Card card)
    {
        foreach (var entry in contents)
        {
            if (entry.card != card) continue;
            
            entry.amount += 1;
            return;
        }

        var newEntry = new DeckEntry();
        newEntry.amount = 1;
        newEntry.card = card;
        contents.Add(newEntry);
    }
    
    public Card DrawCard()
    {
        var totalCards = contents.Select(e => e.amount).Sum();
        var threshold = 0f;
        var randomValue = Random.value;
        
        foreach (var entry in contents)
        {
            threshold += entry.amount / (float) totalCards;
            
            if (!(randomValue < threshold)) continue;
            
            entry.amount -= 1;
            return entry.card;
        }

        throw new ArgumentException("Should not happen, something is wrong with random calculation.");
    }
}

[Serializable]
public class DeckEntry
{
    public Card card;
    public int amount;
}
