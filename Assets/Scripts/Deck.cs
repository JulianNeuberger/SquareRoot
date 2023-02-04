using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Deck : MonoBehaviour
{
    [SerializeField] private List<DeckEntry> contents = new();

    public List<DeckEntry> Cards => contents;
    
    public int CardsRemaining()
    {
        return contents.Select(e => e.amount).Sum();
    }

    public void Clear()
    {
        contents.Clear();
    }
    
    public void AddManyCards(List<DeckEntry> cards)
    {
        foreach (var entry in cards)
        {
            AddCard(entry.card, entry.amount);
        }
    }
    
    public void PlaceCard(Card card)
    {
        AddCard(card, 1);
    }
    
    public Card DrawCard()
    {
        var totalCards = CardsRemaining();
        if (totalCards <= 0)
        {
            throw new ArgumentException("Can't draw from a deck with no cards");
        }
        
        var threshold = 0f;
        var randomValue = Random.value;
        
        foreach (var entry in contents)
        {
            threshold += entry.amount / (float) totalCards;
            
            if (!(randomValue < threshold)) continue;
            
            entry.amount -= 1;
            return entry.card;
        }

        throw new ArgumentException($"Should not happen, something is wrong with random calculation, could not draw a card with threshold {threshold}.");
    }

    private void AddCard(Card card, int amount)
    {
        foreach (var entry in contents)
        {
            if (entry.card != card) continue;
            
            entry.amount += amount;
            return;
        }

        var newEntry = new DeckEntry();
        newEntry.amount = amount;
        newEntry.card = card;
        contents.Add(newEntry);
    }
}

[Serializable]
public class DeckEntry
{
    public Card card;
    public int amount;
}
