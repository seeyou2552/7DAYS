using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_Inventory : UI_Popup
{
    public InventoryManager inventoryManager;

    [SerializeField] private int slotCount;

    private GameObject ItemSlotLayout;
    private List<UI_ItemSlot> slots = new List<UI_ItemSlot>();

    private ItemInfo curItemInfo;
    private ItemData curItemData;
    private int curSlotIndex =-1;
    private int quickSlotCount = 4;

    private Image icon;
    private TextMeshProUGUI nameTxt;
    private TextMeshProUGUI descriptionTxt;
    private Button useBtn;
    private Button closeBtn;
    private Button quickBtn;
    private Button deductBtn;
    private Button deductAllBtn;
    private Button[] setQuickBtn;
    private Button closeQuickBtn;
    private Transform setSlots;

    //색상
    private Color _alpha0 = new Color(255, 255, 255, 0);
    private Color _alpha255 = new Color(255, 255, 255, 255);

    enum Grid
    {
        ItemSlotLayout,
    }
    enum TMPs
    {
        Detail_Name,
        Detail_Description,
    }
    enum Buttons
    {
        CloseBtn,
        UseBtn,
        QuickBtn,
        DeductBtn,
        DeductAllBtn,
        SetQuickBtn_1,
        SetQuickBtn_2,
        SetQuickBtn_3,
        SetQuickBtn_4,
        CloseQuick,
    }
    enum Images
    {
        Detail_Icon,
    }
    enum Transforms
    {
        SetSlots,
    }

    private void Awake()
    {
        inventoryManager = InventoryManager.instance;
    }
    public override void Init()
    {

    }
    private void Start()
    {
        // 초기화
        //base.Init();

        setQuickBtn = new Button[quickSlotCount];

        Bind<GridLayoutGroup>(typeof(Grid));
        Bind<TextMeshProUGUI>(typeof(TMPs));
        Bind<Button>(typeof(Buttons));
        Bind<Image>(typeof(Images));
        Bind<Transform>(typeof(Transforms));

        ItemSlotLayout = Get<GridLayoutGroup>((int)Grid.ItemSlotLayout).gameObject;
        icon = Get<Image>((int)Images.Detail_Icon);
        nameTxt = Get<TextMeshProUGUI>((int)TMPs.Detail_Name);
        descriptionTxt = Get<TextMeshProUGUI>((int)TMPs.Detail_Description);
        useBtn = Get<Button>((int)Buttons.UseBtn);
        closeBtn = Get<Button>((int)Buttons.CloseBtn);
        quickBtn = Get<Button>((int)Buttons.QuickBtn);
        deductBtn = Get<Button>((int)Buttons.DeductBtn);
        deductAllBtn = Get<Button>((int)Buttons.DeductAllBtn);
        quickBtn = Get<Button>((int)Buttons.QuickBtn);
        setSlots = Get<Transform>((int)Transforms.SetSlots);
        setQuickBtn[0] = Get<Button>((int)Buttons.SetQuickBtn_1);
        setQuickBtn[1] = Get<Button>((int)Buttons.SetQuickBtn_2);
        setQuickBtn[2] = Get<Button>((int)Buttons.SetQuickBtn_3);
        setQuickBtn[3] = Get<Button>((int)Buttons.SetQuickBtn_4);
        closeQuickBtn = Get<Button>((int)Buttons.CloseQuick);
        CloseInventoruDetail();
        SetSlot();

        // 버튼 함수 연결
        useBtn.onClick.AddListener(OnConsumItem);
        deductBtn.onClick.AddListener(OnDeductItem);
        deductAllBtn.onClick.AddListener(OnDeductAllItem);
        quickBtn.onClick.AddListener(OnSetQuickSlot);
        closeBtn.onClick.AddListener(inventoryManager.OnInventory);

        for (int i = 0; i < quickSlotCount; i++)
        {
            int slotIndex = i;
            setQuickBtn[i].onClick.AddListener(() => OnSetSlot(slotIndex));
        }
        closeQuickBtn.onClick.AddListener(OnCloseQuickPanel);

    }
    private void SetSlot()
    {
        GameObject prefab = Resources.Load<GameObject>($"Prefabs/UI/Popup/UI_ItemSlot");
        // 슬롯생성
        for (int i = 0; i < slotCount; i++)
        {
            var go = Instantiate(prefab);
            slots.Add(go.GetComponent<UI_ItemSlot>());
            slots[i].inventory = this;
            slots[i].index = i;
            slots[i].ResetSlot();
            go.transform.SetParent(ItemSlotLayout.transform, false);
        }

        // 인벤토리에서 슬롯 반영
        //var curSlots = inventoryManager.itemSlots;
        for (int i = 0; i < slotCount; i++)
        {
            UpdateSlotData(i);
        }

    }

    public void UpdateSlotData(int index)
    {
        if(inventoryManager.itemSlots[index] == null)
        {
            slots[index].ResetSlot();
            return;
        }

        ItemInfo curSlots = inventoryManager.itemSlots[index];


        if (curSlots != null&& curSlots.data != null)
        {
            slots[index].Item = curSlots.data;
            slots[index].Stack = curSlots.count;
            slots[index].UpdateIcon(curSlots.data.icon);
            slots[index].UpdateTMP();
        }
        else
        {
            slots[index].ResetSlot();
        }
    }

    // 버튼 함수 할당
    private void OnConsumItem()
    {
        setSlots.gameObject.SetActive(false);
        if (curItemData != null)
        {
            inventoryManager.OnUseItem(curSlotIndex, slots[curSlotIndex].Item);
            UpdateSlotData(curSlotIndex);
        }
    }
    private void OnDeductItem()
    {
        setSlots.gameObject.SetActive(false);
        if (curItemData != null)
        {
            inventoryManager.DeductItemBySlot(curSlotIndex,1);
            UpdateSlotData(curSlotIndex);
        }
    }
    private void OnDeductAllItem()
    {
        setSlots.gameObject.SetActive(false);
        if (curItemData != null)
        {
            inventoryManager.DeductItemBySlot(curSlotIndex, curItemInfo.count);
            UpdateSlotData(curSlotIndex);
        }
    }
    // 퀵슬롯 버튼
    private void OnSetQuickSlot()
    {
        setSlots.gameObject.SetActive(true);
    }
    private void OnSetSlot(int index)
    {
        //슬롯인덱스, 아이템위치 인덱스, 아이템 정보
        inventoryManager.SetQuickSlot(index, curSlotIndex, curItemInfo);
        OnCloseQuickPanel();
    }

    private void OnCloseQuickPanel()
    {
        setSlots.gameObject.SetActive(false);
    }


    public void ShowDetail(int index)
    {
        CloseInventoruDetail();
        OpenInventoruDetail();
        var slot = slots[index];
        curSlotIndex = index;
        curItemInfo = inventoryManager.itemSlots[curSlotIndex];
        curItemData = slot.Item;
        icon.sprite = curItemData.icon;
        nameTxt.text = curItemData.resourceName;
        descriptionTxt.text = curItemData.description;

        switch (curItemData.type)
        {
            case ItemType.Consumable:
                useBtn.gameObject.SetActive(true);
                quickBtn.gameObject.SetActive(true);
                break;
        }
    }

    void CloseInventoruDetail()
    {
        curItemData = null;
        curItemInfo = null;
        curSlotIndex = -1;
        icon.color = _alpha0;
        nameTxt.gameObject.SetActive(false);
        descriptionTxt.gameObject.SetActive(false);
        useBtn.gameObject.SetActive(false);
        quickBtn.gameObject.SetActive(false);
        deductBtn.gameObject.SetActive(false);
        deductAllBtn.gameObject.SetActive(false);
        setSlots.gameObject.SetActive(false);
    }
    void OpenInventoruDetail()
    {
        icon.color = _alpha255;
        nameTxt.gameObject.SetActive(true);
        descriptionTxt.gameObject.SetActive(true);
        deductBtn.gameObject.SetActive(true);
        deductAllBtn.gameObject.SetActive(true);
    }

}
