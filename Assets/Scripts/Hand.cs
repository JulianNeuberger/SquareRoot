using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class Hand : MonoBehaviour
{
    [SerializeField] private HandCard handCardPrefab;
    [SerializeField] private float pixelsPerUnit = 32f;
    [SerializeField] private float cardSize = 1f;
    [SerializeField] private float availableSpace = 30f;
    [SerializeField] private float height = 3f;
    [SerializeField] private float maxGap = 1f;

    [SerializeField] private Vector3 draggingOffset = new(1f, -1.5f, 0f);

    [SerializeField] private CardGrid grid;
    
    public List<Card> cards = new();

    private readonly List<HandCard> _currentCards = new();
    private HandCard _draggedCard;
    private int _draggedCardRotation;

    private Camera _camera;

    protected void Start()
    {
        _camera = transform.parent.GetComponent<Camera>();
        // we could also set the camera somehow, to allow this component to be a child of anything other than a cam
        // but then we would have to update its position in each update accordingly!
        if (_camera == null) throw new ArgumentException("Hand has to be a child of the camera for now.");
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
        handCard.SetSortingOrder(transform.childCount - 1);
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
            pos += new Vector3(cardSize / 2f, cardSize / 2f, 0f);
            pos += new Vector3(margin, 0f, 0f);
            // account for gaps, after the first card
            if (i > 0) pos += new Vector3(i * gap, 0f, 0f);
            
            _currentCards[i].transform.localPosition = pos;
        }
    }

    private void HandleCardDragStarted()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        var cardStartedDragging = GetDraggedHandCard();
        if (cardStartedDragging == null) return;

        _draggedCard = cardStartedDragging;
        _draggedCard.Collider2D.enabled = false;
    }

    private void HandleCardDragOngoing()
    {
        if (_draggedCard == null) return;

        var worldPos = _camera.ScreenToWorldPoint(Input.mousePosition);
        worldPos.z = 0f;
        _draggedCard.transform.position = worldPos + draggingOffset;

        var gridPos = grid.WorldCoordinatesToGridPosition(worldPos);
        var canPlace = grid.CanPlaceCard(gridPos, _draggedCard.Card, _draggedCardRotation);
        grid.HighlightCell(gridPos, canPlace ? Color.green : Color.red);
    }

    private void HandleCardDragRotate()
    {
        if (_draggedCard == null) return;

        if (!Input.GetMouseButtonDown(1)) return;

        _draggedCardRotation = (_draggedCardRotation + 1) % 4;
        _draggedCard.RotateSprite(new Vector3(0, 0, -90));
    }

    private void HandleCardDragEnded()
    {
        if (!Input.GetMouseButtonUp(0)) return;
        if (!_draggedCard) return;
        
        var worldPos = _camera.ScreenToWorldPoint(Input.mousePosition);
        var gridPos = grid.WorldCoordinatesToGridPosition(worldPos);

        if (grid.CanPlaceCard(gridPos, _draggedCard.Card, _draggedCardRotation))
        {
            var success = grid.TryPlaceCard(gridPos, _draggedCard.Card, _draggedCardRotation);
            if(success) RemoveCard(_draggedCard);
        }

        _draggedCard.Collider2D.enabled = true;
        _draggedCard = null;
        _draggedCardRotation = 0;

        grid.ResetHighlight();
        
        LayoutCards();
    }
    
    private HandCard GetDraggedHandCard()
    {
        var ray = _camera.ScreenPointToRay(Input.mousePosition);
        var hit = Physics2D.Raycast(ray.origin, ray.direction, 10f);
        if (!hit) return null;
        
        var target = hit.collider.gameObject;
        return target.GetComponent<HandCard>();
    }
    
    // GIZMOS
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(transform.position + new Vector3(availableSpace / 2f, height / 2f, 0f),
            new Vector3(availableSpace, height, 0f));
    }
}

[CustomEditor(typeof(Hand))]
public class HandEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Deal random card"))
        {
            var hand = (Hand) target;
            var card = hand.cards[Random.Range(0, hand.cards.Count)];
            hand.DealCard(card);
        }
    }
}