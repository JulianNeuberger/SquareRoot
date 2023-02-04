using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class SugarDisplay : MonoBehaviour
{
    private int sugarAmount = 0;
    private int sugarUpkeep = 0;
    private bool sugarShortageStatus = false;
    public TextMeshProUGUI sugarAmountTmp;
    public TextMeshProUGUI sugarUpkeepTmp;
    public TextMeshProUGUI sugarMessageTmp;

    // Update is called once per frame
    void Update()
    {
        sugarAmountTmp.text = $"{sugarAmount}";
        sugarUpkeepTmp.text = $"{sugarUpkeep}";

        if (sugarShortageStatus)
        {
            sugarMessageTmp.text = "SUGAR SHORTAGE!";
        }
        else
        {
            sugarMessageTmp.text = "";
        }
    }

    public void UpdateSugarAmount(int amount)
    {
        sugarAmount = amount;
    }


    public void UpdateSugarUpkeep(int upkeep)
    {
        sugarUpkeep = upkeep;
    }

    public void UpdateSugarShortageStatus(bool shortageStatus)
    {
        sugarShortageStatus = shortageStatus;
    }
}
