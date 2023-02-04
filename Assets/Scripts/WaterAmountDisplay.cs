using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class WaterAmountDisplay : MonoBehaviour
{
    private int waterAmount = 0;
    private int waterIncome = 0;
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
        tmp.text = $"Water: {waterAmount} + {waterIncome} - {waterUpkeep}";
    }

    public void UpdateWaterAmount(int amount)
    {
        waterAmount = amount;
    }

    public void UpdateWaterIncome(int income)
    {
        waterIncome = income;
    }

    public void UpdateWaterUpkeep(int upkeep)
    {
        waterUpkeep = upkeep;
    }
}
