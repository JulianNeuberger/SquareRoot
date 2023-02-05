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

    [SerializeField] private SpriteRenderer ghost;
    
    [SerializeField] private Vector3 draggingOffset = new(1f, -1.5f, 0f);

    [SerializeField] private CardGrid grid;
    [SerializeField] private ResourceManager resourceManager;
    
    [SerializeField] private Camera camera;

    [SerializeField] private RectTransform drawer;
    [SerializeField] private float drawerHideSeconds = .5f;
    [SerializeField] private float drawerHideDistance = -30f;

    private float _drawerHideStarted = 0f;
    private Vector2 _drawerStartPos;
    private Vector2 _drawerTargetPos;
    
    public Deck redrawPile;
    public List<HandCard> Current => _currentCards;
    
    private readonly List<HandCard> _currentCards = new();
    private HandCard _draggedCard;
    private int _draggedCardRotation;

    protected void Start()
    {
        if (grid == null) throw new ArgumentException("CardGrid not set!");

        _drawerStartPos = drawer.position;
    }
    
    protected void Update()
    {
        HandleCardDragStarted();
        HandleCardDragOngoing();
        HandleCardDragRotate();
        HandleCardDragEnded();
        
        HandleDrawerHide();
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
    }
    
    public void RemoveCard(HandCard card)
    {
        if (!_currentCards.Contains(card)) return;
        _currentCards.Remove(card);

        Destroy(card.gameObject);
    }

    private void HandleDrawerHide()
    {
        var t = (Time.time - _drawerHideStarted) / drawerHideSeconds;
        if (t >= 1f)
        {
            drawer.position = _drawerTargetPos;
            return;
        }
        drawer.position = Vector3.Lerp(drawer.position, _drawerTargetPos, t);
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

        _drawerHideStarted = Time.time;
        _drawerTargetPos = _drawerStartPos + new Vector2(0f, drawerHideDistance);
    }

    private void HandleCardDragOngoing()
    {
        if (_draggedCard == null) return;

        var worldPos = camera.ScreenToWorldPoint(Input.mousePosition);
        worldPos.z = 0f;
        
        ghost.transform.position = worldPos;
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
        
        _drawerHideStarted = Time.time;
        _drawerTargetPos = _drawerStartPos;

        grid.ResetHighlight();
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