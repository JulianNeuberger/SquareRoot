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

    public List<Card> cards = new();

    private readonly List<HandCard> _currentCards = new();
    private HandCard _draggedCard;

    private Camera _camera;

    protected void Start()
    {
        _camera = transform.parent.GetComponent<Camera>();
        // we could also set the camera somehow, to allow this component to be a child of anything other than a cam
        // but then we would have to update its position in each update accordingly!
        if (_camera == null) throw new ArgumentException("Hand has to be a child of the camera for now.");
    }
    
    protected void Update()
    {
        HandleCardDragStarted();
        HandleCardDragOngoing();
        HandleCardDragEnded();
    }

    public void DealCard(Card card)
    {
        var handCard = Instantiate(handCardPrefab, transform);
        handCard.transform.position = Vector3.zero;
        handCard.Card = card;
        _currentCards.Add(handCard);

        LayoutCards();
    }

    public void PlayCard(HandCard card)
    {
        if (!_currentCards.Contains(card)) return;
        _currentCards.Remove(card);

        LayoutCards();
    }

    private void LayoutCards()
    {
        for (var i = 0; i < _currentCards.Count; ++i)
        {
            _currentCards[i].transform.localPosition =
                new Vector3(i * cardSize + cardSize / 2f + .5f, cardSize / 2f, 0f);
        }
    }

    private void HandleCardDragStarted()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        var cardStartedDragging = GetDraggedHandCard();
        if (cardStartedDragging == null) return;

        Debug.Log("Drag started");
        _draggedCard = cardStartedDragging;
    }

    private void HandleCardDragOngoing()
    {
        if (_draggedCard == null) return;

        Debug.Log("Drag ongoing");
        
        var worldPos = _camera.ScreenToWorldPoint(Input.mousePosition);
        worldPos.z = 0f;
        _draggedCard.transform.position = worldPos;
        // TODO: animate if card cant be dropped here
    }

    private void HandleCardDragEnded()
    {
        if (!Input.GetMouseButtonUp(0)) return;
        
        // TODO: get grid cell for current pointer world pos
        // TODO: check if grid allows card here
        Debug.Log("Dropped");
        _draggedCard = null;
        
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