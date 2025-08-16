using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Refinery : UI_Popup
{
    private Button closeBtn;
    private Button inputBtn;
    private Button getBtn;

    TextMeshProUGUI UnP_Stack;
    TextMeshProUGUI P_Stack;

    public Refinery refinery;
    public ItemData p_water;

    enum Buttons
    {
        CloseBtn,
        InputBtn,
        GetBtn,
    }
    enum Stacks
    {
        UnP_Stack,
        P_Stack,
    }

    private void Start()
    {
        Bind<Button>(typeof(Buttons));
        Bind<TextMeshProUGUI>(typeof(Stacks));

        closeBtn = Get<Button>((int)Buttons.CloseBtn);
        inputBtn = Get<Button>((int)Buttons.InputBtn);
        getBtn = Get<Button>((int)Buttons.GetBtn);

        UnP_Stack = Get<TextMeshProUGUI>((int)Stacks.UnP_Stack);
        P_Stack = Get<TextMeshProUGUI>((int)Stacks.P_Stack);


        refinery.onChangeStack += UpdateStack;

        // 버튼 함수 연결
        closeBtn.onClick.AddListener(OnClose);
        inputBtn.onClick.AddListener(OnInput);
        getBtn.onClick.AddListener(OnGet);
        UpdateStack(refinery.inputAmount, refinery.productAmount);
    }
    private void OnDisable()
    {
        refinery.onChangeStack -= UpdateStack;

    }
    void UpdateStack(int input, int get)
    {
        UnP_Stack.text = input.ToString();
        P_Stack.text = get.ToString();
    }
    void OnClose()
    {
        Destroy(gameObject);
    }
    void OnInput()
    {
        var outPut = refinery.TryConsumeForProduct();

    }
    void OnGet()
    {
        if (refinery.productAmount > 0)
        {
            InventoryManager.instance.AddItem(p_water, refinery.productAmount);
            refinery.productAmount = 0;
            UpdateStack(refinery.inputAmount, refinery.productAmount);
        }
    }
}
