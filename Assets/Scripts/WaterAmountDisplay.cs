using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class WaterAmountDisplay : MonoBehaviour
{
    private int waterAmount = 0;
    private int waterUpkeep = 0;
    private TextMeshProUGUI tmp;

    // Start is called before the first frame update
    void Start()
    {
        tmp = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if(waterUpkeep >= 0)
        {
            tmp.text = $"Water: {waterAmount} + {waterUpkeep}";
        }
        else
        {
            tmp.text = $"Water: {waterAmount} - {Math.Abs(waterUpkeep)}";
        }

    }

    public void UpdateWaterAmount(int amount)
    {
        waterAmount = amount;
    }

    public void UpdateWaterUpkeep(int upkeep)
    {
        waterUpkeep = upkeep;
    }
}
