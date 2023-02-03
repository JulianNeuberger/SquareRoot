using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = "Card")]
public class Card : ScriptableObject
{
    public Sprite sprite;
    
    public string cardType;
    public int numberOfSockets;
    public string[] attachableCardTypes;


    public bool CanAttachCardType(string cardType)
    {
        if (attachableCardTypes.Contains(cardType))
        {
            return true;
        }
        return false;
    }
}
