using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UI_StatBar;

public class UI_FloatType : UI_Scene
{
    private TextMeshProUGUI countTxt;
    InventoryManager inventoryManager;



    [SerializeField] private FloatType type;

    enum TMP
    {
        CountTxt
    }

    public override void Init()
    {
        base.Init();
        Bind<TextMeshProUGUI>(typeof(TMP));
        inventoryManager = InventoryManager.instance;
        countTxt = Get<TextMeshProUGUI>((int)TMP.CountTxt);
        switch (type)
        {
            case FloatType.Item_ScrapIron:
                inventoryManager.OnScrapChanged += UpdateCurrent;
                break;
            case FloatType.Item_Circuit:
                inventoryManager.OnCircuitChanged += UpdateCurrent;
                break;
            case FloatType.Item_Fuel:
                inventoryManager.OnFuelChanged += UpdateCurrent;
                break;
            case FloatType.Wave:
                TestGameManager.Inst.OnChageWave += UpdateCurrent;
                countTxt.text = "0";
                break;
        }
    }
    private void OnDisable()
    {
        switch (type)
        {
            case FloatType.Item_ScrapIron:
                inventoryManager.OnScrapChanged -= UpdateCurrent;
                break;
            case FloatType.Item_Circuit:
                inventoryManager.OnCircuitChanged -= UpdateCurrent;
                break;
            case FloatType.Item_Fuel:
                inventoryManager.OnFuelChanged -= UpdateCurrent;
                break;
        }
    }

    void UpdateCurrent(int stack)
    {
        countTxt.text = stack.ToString();
    }
}
