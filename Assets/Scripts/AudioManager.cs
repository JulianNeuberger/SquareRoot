using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource ambientMusic;

    [SerializeField] private AudioSource cardStartDrag;
    [SerializeField] private AudioSource cardPlacement;
    [SerializeField] private AudioSource cardReturnToHand;
    [SerializeField] private AudioSource cardRotate;
    [SerializeField] private AudioSource clickOnUsableLeaf;
    [SerializeField] private AudioSource exchangeAtLeaf;
    [SerializeField] private AudioSource endRound;
    [SerializeField] private AudioSource notifyShortage;
    [SerializeField] private AudioSource leafWithering;
    [SerializeField] private AudioSource leafFalling;
    [SerializeField] private AudioSource notEnoughResources;


    public void PlayCardStartDragSound()
    {
        cardStartDrag.PlayOneShot(cardStartDrag.clip);
    }
    public void PlayCardPlacementSound()
    {
        cardPlacement.PlayOneShot(cardPlacement.clip);
    }

    public void PlayCardReturnToHandSound()
    {
        cardReturnToHand.PlayOneShot(cardReturnToHand.clip);
    }

    public void PlayCardRotateSound()
    {
        cardRotate.PlayOneShot(cardRotate.clip);
    }

    public void PlayClickOnUsableLeafSound()
    {
        clickOnUsableLeaf.PlayOneShot(clickOnUsableLeaf.clip);
    }

    public void PlayExchangeAtLeafSound()
    {
        exchangeAtLeaf.PlayOneShot(exchangeAtLeaf.clip);
    }

    public void PlayEndRoundSound()
    {
        endRound.PlayOneShot(endRound.clip);
    }

    public void PlayNotifyShortageSound()
    {
        notifyShortage.PlayOneShot(notifyShortage.clip);
    }

    public void PlayLeafWitheringSound()
    {
        leafWithering.PlayOneShot(leafWithering.clip);
    }

    public void PlayLeafFallingSound()
    {
        leafFalling.PlayOneShot(leafFalling.clip);
    }

    public void PlayNotEnoughResourcesSound()
    {
        notEnoughResources.PlayOneShot(notEnoughResources.clip);
    }
}
