using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class WaterDisplay : MonoBehaviour
{
    private int waterAmount = 0;
    private int waterIncome = 0;
    private int waterUpkeep = 0;
    private bool waterShortageStatus = false;
    public TextMeshProUGUI waterAmountTmp;
    public TextMeshProUGUI waterIncomeTmp;
    public TextMeshProUGUI waterUpkeepTmp;
    public TextMeshProUGUI waterMessageTmp;

    // Update is called once per frame
    void Update()
    {
        waterAmountTmp.text = $"{waterAmount}";
        waterIncomeTmp.text = $"{waterIncome}";
        waterUpkeepTmp.text = $"{waterUpkeep}";

        if (waterShortageStatus)
        {
            waterMessageTmp.text = "WATER SHORTAGE!";
        }
        else
        {
            waterMessageTmp.text = "";
        }
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

    public void UpdateWaterShortageStatus(bool shortageStatus)
    {
        waterShortageStatus = shortageStatus;
    }
}
