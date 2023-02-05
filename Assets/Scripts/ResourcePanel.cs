using TMPro;
using UnityEngine;

public class ResourcePanel : MonoBehaviour
{
    [SerializeField] private ResourceManager resourceManager;

    [SerializeField] private RectTransform nitrateStockContainer;
    [SerializeField] private RectTransform nitrateIncomeContainer;
    [SerializeField] private RectTransform nitrateUpkeepContainer;

    [SerializeField] private RectTransform waterStockContainer;
    [SerializeField] private RectTransform waterIncomeContainer;
    [SerializeField] private RectTransform waterUpkeepContainer;

    [SerializeField] private RectTransform sugarStockContainer;
    [SerializeField] private RectTransform sugarIncomeContainer;
    [SerializeField] private RectTransform sugarUpkeepContainer;

    [SerializeField] private RectTransform sugarIcon;
    [SerializeField] private RectTransform waterIcon;
    [SerializeField] private RectTransform nitrateIcon;

    [SerializeField] private TextMeshProUGUI amountTextPrefab;

    public void Awake()
    {
        resourceManager.NitrateAmountChanged = amount => UpdateContainer(amount, nitrateIcon, nitrateStockContainer);
        resourceManager.NitrateIncomeChanged = amount => UpdateContainer(amount, nitrateIcon, nitrateIncomeContainer);
        resourceManager.NitrateUpkeepChanged = amount => UpdateContainer(amount, nitrateIcon, nitrateUpkeepContainer);
        
        resourceManager.WaterAmountChanged = amount => UpdateContainer(amount, waterIcon, waterStockContainer);
        resourceManager.WaterIncomeChanged = amount => UpdateContainer(amount, waterIcon, waterIncomeContainer);
        resourceManager.WaterUpkeepChanged = amount => UpdateContainer(amount, waterIcon, waterUpkeepContainer);
        
        resourceManager.SugarAmountChanged = amount => UpdateContainer(amount, sugarIcon, sugarStockContainer);
        resourceManager.SugarIncomeChanged = amount => UpdateContainer(amount, sugarIcon, sugarIncomeContainer);
        resourceManager.SugarUpkeepChanged = amount => UpdateContainer(amount, sugarIcon, sugarUpkeepContainer);
    }

    private void UpdateContainer(int amount, RectTransform icon, RectTransform container)
    {
        foreach (RectTransform child in container.transform)
        {
            Destroy(child.gameObject);
        }

        if (amount > 5)
        {
            Instantiate(icon, container);
            var text = Instantiate(amountTextPrefab, container);
            text.text = $"x{amount}";
        }
        else
        {
            for (var i = 0; i < amount; ++i)
            {
                Instantiate(icon, container);
            }
        }
    }
}