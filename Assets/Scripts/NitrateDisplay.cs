using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class NitrateDisplay : MonoBehaviour
{
    private int nitrateAmount = 0;
    private int nitrateIncome = 0;
    private int nitrateUpkeep = 0;
    private bool nitrateShortageStatus = false;
    public TextMeshProUGUI nitrateAmountTmp;
    public TextMeshProUGUI nitrateIncomeTmp;
    public TextMeshProUGUI nitrateUpkeepTmp;
    public TextMeshProUGUI nitrateMessageTmp;

    // Update is called once per frame
    void Update()
    {
        nitrateAmountTmp.text = $"{nitrateAmount}";
        nitrateIncomeTmp.text = $"{nitrateIncome}";
        nitrateUpkeepTmp.text = $"{nitrateUpkeep}";

        if (nitrateShortageStatus)
        {
            nitrateMessageTmp.text = "NITRATE SHORTAGE!";
        }
        else
        {
            nitrateMessageTmp.text = "";
        }
    }

    public void UpdateNitrateAmount(int amount)
    {
        nitrateAmount = amount;
    }

    public void UpdateNitrateIncome(int income)
    {
        nitrateIncome = income;
    }

    public void UpdateNitrateUpkeep(int upkeep)
    {
        nitrateUpkeep = upkeep;
    }

    public void UpdateNitrateShortageStatus(bool shortageStatus)
    {
        nitrateShortageStatus = shortageStatus;
    }
}
