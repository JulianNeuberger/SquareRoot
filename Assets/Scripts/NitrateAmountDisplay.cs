using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class NitrateAmountDisplay : MonoBehaviour
{
    private int nitrateAmount = 0;
    private int nitrateUpkeep = 0;
    private TextMeshProUGUI tmp;

    // Start is called before the first frame update
    void Start()
    {
        tmp = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (nitrateUpkeep >= 0)
        {
            tmp.text = $"Nitrate: {nitrateAmount} + {nitrateUpkeep}";
        }
        else
        {
            tmp.text = $"Nitrate: {nitrateAmount} - {Math.Abs(nitrateUpkeep)}";
        }
    }

    public void UpdateNitrateAmount(int amount)
    {
        nitrateAmount = amount;
    }

    public void UpdateNitrateUpkeep(int upkeep)
    {
        nitrateUpkeep = upkeep;
    }
}
