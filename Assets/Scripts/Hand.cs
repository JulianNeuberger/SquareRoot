using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Hand : MonoBehaviour
{
    [SerializeField] private HandCard handCardPrefab;
    [SerializeField] private float pixelsPerUnit = 32f;
    [SerializeField] private float cardSize = 1f;
    [SerializeField] private float availableSpace = 30f;
    [SerializeField] private float height = 3f;
    [SerializeField] private float maxGap = 1f;

    [SerializeField] private Image ghost;
    
    [SerializeField] private Vector3 draggingOffset = new(1f, -1.5f, 0f);

    [SerializeField] private CardGrid grid;
    [SerializeField] private ResourceManager resourceManager;
    
    [SerializeField] private Camera camera;
    
    public Deck redrawPile;
    public List<HandCard> Current => _currentCards;
    
    private readonly List<HandCard> _currentCards = new();
    private HandCard _draggedCard;
    private int _draggedCardRotation;

    protected void Start()
    {
        if (grid == null) throw new ArgumentException("CardGrid not set!");
    }
    
    protected void Update()
    {
        HandleCardDragStarted();
        HandleCardDragOngoing();
        HandleCardDragRotate();
        HandleCardDragEnded();
    }

    public void Clear()
    {
        foreach (var handCard in _currentCards)
        {
            Destroy(handCard.gameObject);
        }
        
        _currentCards.Clear();
    }
    
    public void DealCard(Card card)
    {
        var handCard = Instantiate(handCardPrefab, transform);
        handCard.transform.position = Vector3.zero;
        handCard.Card = card;
        handCard.transform.SetSiblingIndex(transform.childCount - 1);
        _currentCards.Add(handCard);

        LayoutCards();
    }
    
    public void RemoveCard(HandCard card)
    {
        if (!_currentCards.Contains(card)) return;
        _currentCards.Remove(card);

        Destroy(card.gameObject);

        LayoutCards();
    }

    private void LayoutCards()
    {
        var gap = 0f;
        if (_currentCards.Count > 1)
        {
            // more than one card on hand, there will be gaps, but how large?
            var gapSpace = availableSpace - _currentCards.Count * cardSize;
            gap = gapSpace / (_currentCards.Count - 1);
            gap = Mathf.Min(maxGap, gap);
        }

        var margin = 0f;
        // how much space do the cards themselves take up and what remains?
        var remainingSpace = availableSpace - _currentCards.Count * cardSize;
        if (_currentCards.Count > 1)
        {
            // account for gaps
            remainingSpace -= gap * (_currentCards.Count - 1);
        }
        
        // the rest is margin
        margin = remainingSpace / 2f;

        for (var i = 0; i < _currentCards.Count; ++i)
        {
            var pos = new Vector3(i * cardSize, 0f, 0f);
            pos += new Vector3(cardSize / 2f, 0, 0f);
            pos += new Vector3(margin, 0f, 0f);
            // account for gaps, after the first card
            if (i > 0) pos += new Vector3(i * gap, 0f, 0f);
            
            Debug.Log(pos);
            
            _currentCards[i].rectTransform.anchoredPosition = pos;
        }
    }

    private void HandleCardDragStarted()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        var cardStartedDragging = GetDraggedHandCard();
        Debug.Log($"cardStartedDraggin: {cardStartedDragging}.");
        if (cardStartedDragging == null) return;

        _draggedCard = cardStartedDragging;
        
        ghost.sprite = cardStartedDragging.Card.sprite;
        ghost.enabled = true;
    }

    private void HandleCardDragOngoing()
    {
        if (_draggedCard == null) return;

        ghost.transform.position = Input.mousePosition;
        
        var worldPos = camera.ScreenToWorldPoint(Input.mousePosition);
        worldPos.z = 0f;
        var gridPos = grid.WorldCoordinatesToGridPosition(worldPos);
        var canPlace = grid.CanPlaceCard(gridPos, _draggedCard.Card, _draggedCardRotation);
        grid.HighlightCell(gridPos, canPlace ? Color.green : Color.red);
    }

    private void HandleCardDragRotate()
    {
        if (_draggedCard == null) return;

        if (!Input.GetMouseButtonDown(1)) return;

        _draggedCardRotation = (_draggedCardRotation + 1) % 4;

        var rotationAngles = new Vector3(0, 0, -90);
        ghost.transform.Rotate(rotationAngles);
    }

    private void HandleCardDragEnded()
    {
        if (!Input.GetMouseButtonUp(0)) return;
        if (!_draggedCard) return;
        
        var worldPos = camera.ScreenToWorldPoint(Input.mousePosition);
        var gridPos = grid.WorldCoordinatesToGridPosition(worldPos);

        if (grid.CanPlaceCard(gridPos, _draggedCard.Card, _draggedCardRotation))
        {
            var success = grid.TryPlaceCard(gridPos, _draggedCard.Card, _draggedCardRotation);
            if (success)
            {
                RemoveCard(_draggedCard);
                resourceManager.PayNitrate(_draggedCard.Card.nitrateCost);
                resourceManager.PaySugar(_draggedCard.Card.sugarCost);
                resourceManager.PayWater(_draggedCard.Card.waterCost);
                resourceManager.UpdateUpkeep();
                resourceManager.UpdateResourceIncome();
            }
        }

        _draggedCard = null;
        _draggedCardRotation = 0;

        ghost.enabled = false;

        grid.ResetHighlight();
        
        LayoutCards();
    }
    
    private HandCard GetDraggedHandCard()
    {
        Debug.Log($"Event system: '{EventSystem.current}'");
        var pointerData = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
        var hits = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, hits);
        if (hits.Count == 0)
        {
            Debug.Log("No hits");
            return null;
        }

        var handCards = hits.Where(h => h.gameObject.GetComponent<HandCard>() != null);
        if (handCards.Count() == 0) return null;
        
        return handCards.First().gameObject.GetComponent<HandCard>();
    }
    
    // GIZMOS
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(transform.position + new Vector3(availableSpace / 2f, height / 2f, 0f),
            new Vector3(availableSpace, height, 0f));
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(Hand))]
public class HandEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Deal random card"))
        {
            var hand = (Hand) target;
            var card = hand.redrawPile.DrawCard();
            hand.DealCard(card);
        }
    }
}
#endif