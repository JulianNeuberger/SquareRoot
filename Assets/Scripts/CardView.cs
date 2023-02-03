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
}
