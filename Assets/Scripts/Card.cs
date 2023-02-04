using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = "Card")]
public class Card : ScriptableObject
{
    public Sprite sprite;
    
    public int numberOfSockets;
    public Card[] attachableCardTypes;
    public float gatherMultiplierOnResource;
    public float gatherMultiplierNextToResource;


    public bool CanAttachCardType(Card cardType)
    {
        if (attachableCardTypes.Contains(cardType))
        {
            return true;
        }
        return false;
    }
}
