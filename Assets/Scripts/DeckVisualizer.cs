using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckVisualizer : MonoBehaviour
{
    [SerializeField] private Deck visualizedDeck;
    [SerializeField] private RectTransform card;

    private int _lastSize;
    
    private void Start()
    {
        visualizedDeck.deckChanged.AddListener(d => UpdateCards(d.CardsRemaining()));
    }

    private void UpdateCards(int amount)
    {
        if (_lastSize == amount) return;
        _lastSize = amount;

        var currentCards = transform.childCount;
        var diff = amount - currentCards;

        if (diff > 0)
        {
            for (var i = currentCards; i < amount; ++i)
            {
                var cardInstance = Instantiate(card, transform);
                var pos = card.anchoredPosition;
                pos.y = i * .75f * 2;
                cardInstance.anchoredPosition = pos;
            }
        }
        else
        {
            for (var i = currentCards - 1; i >= amount; --i)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }
        

        

    }
}
