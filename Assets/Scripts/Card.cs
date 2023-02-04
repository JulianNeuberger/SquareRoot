using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Card")]
public class Card : ScriptableObject
{
    public Sprite sprite;
    
    // to easily configure the card in the inspector, will be put in hasSocketAtSideId on Awake
    public bool hasSocketAtCardTopSide;
    public bool hasSocketAtCardRightSide;
    public bool hasSocketAtCardBottomSide;
    public bool hasSocketAtCardLeftSide;

   
    private bool[] hasSocketAtCardSideId = new bool[4]; // stored in card coordinates: with rotation 0

    public Card[] attachableCardTypes;
    
    public float gatherMultiplierOnResource;
    public float gatherMultiplierNextToResource;


    private void Awake()
    {
        hasSocketAtCardSideId[0] = hasSocketAtCardTopSide;
        hasSocketAtCardSideId[1] = hasSocketAtCardRightSide;
        hasSocketAtCardSideId[2] = hasSocketAtCardBottomSide;
        hasSocketAtCardSideId[3] = hasSocketAtCardLeftSide;
    }


    public bool HasSocketAtWorldSideId(int worldSideId, int cardRotation)
    {
        var cardSideId = (worldSideId - cardRotation + 4) % 4;
        return hasSocketAtCardSideId[cardSideId];
    }

    public bool CanAttachCardType(Card cardType)
    {
        if (attachableCardTypes.Contains(cardType))
        {
            return true;
        }
        return false;
    }
}
