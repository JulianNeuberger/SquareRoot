using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class NitrateAmountDisplay : MonoBehaviour
{
    private int nitrateAmount = 0;
    private int nitrateIncome = 0;
    private int nitrateUpkeep = 0;
    private bool nitrateShortageStatus = false;
    private TextMeshProUGUI tmp;

    // Start is called before the first frame update
    void Start()
    {
        tmp = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if(nitrateShortageStatus)
        {
            tmp.text = $"Nitrate: {nitrateAmount} + {nitrateIncome} - {nitrateUpkeep} | NITRATE SHORTAGE";
        }
        else
        {
            tmp.text = $"Nitrate: {nitrateAmount} + {nitrateIncome} - {nitrateUpkeep}";
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
