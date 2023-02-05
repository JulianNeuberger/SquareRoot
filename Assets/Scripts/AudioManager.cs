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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
