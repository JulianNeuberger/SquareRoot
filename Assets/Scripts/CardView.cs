using System.Collections.Generic;
using UnityEngine;

public class CardView : MonoBehaviour
{
    private Card _card;
    private int _occupiedSockets = 0;

    public Card GetCard()
    {
        return _card;
    }

    public bool HasOpenSockets()
    {
        if (_occupiedSockets < _card.numberOfSockets)
        {
            return true;
        }
        return false;
    }

    public Dictionary<string, int> GatherResources()
    {
        var gatheredResources = new Dictionary<string, int>();
        



    }
}
