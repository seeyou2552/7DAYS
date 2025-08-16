using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

[Serializable]
public class ItemInfo // 리스트에 추가할 아이템 정보
{
    public ItemData data;
    public int count;

    public ItemInfo(ItemData data, int count)
    {
        this.data = data;
        this.count = count;
    }
}
[Serializable]
public class ItemDictionaryEntry
{
    public FloatType ItemType; // 딕셔너리의 키
    public ItemInfo ItemData; // 딕셔너리의 값
}

public enum FloatType
{
    Item_ScrapIron,
    Item_Circuit,
    Item_Fuel,
    Wave,
}


public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;
    // 기초 자원
    [SerializeField]
    private List<ItemDictionaryEntry> itemEntries = new List<ItemDictionaryEntry>();
    public Dictionary<string, ItemInfo> itemList = new Dictionary<string, ItemInfo>();
    public event Action<int> OnScrapChanged;
    public event Action<int> OnCircuitChanged;
    public event Action<int> OnFuelChanged;
    [SerializeField] private float itemCoolTime = 2.0f;

    // 인벤토리 아이템
    public ItemInfo[] itemSlots;
    [SerializeField] private int slotCount = 20;
    private bool isOpenInventory = false;
    private UI_Inventory uiInventory;

    // 퀵슬롯
    public Dictionary<ItemInfo, int> quickSlots; // 해당 아이템이 아이템슬롯 몇번과 연결되어있는가
    public ItemInfo[] quickSlotsIndex; // 해당 아이템이 퀵슬롯 몇번과 연결되어 있는가.
    public UI_QuickSlotManager quickSlotManager;
    public Player player;
    public event Action<int> OnHealthChanged;
     public WeaponDataSO[] weaponDataSO;

    private void Awake()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        // 싱글톤
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
            Destroy(this.gameObject);
        quickSlotsIndex = new ItemInfo[4];
        quickSlots = new Dictionary<ItemInfo, int>();
        itemSlots = new ItemInfo[slotCount];
        itemList.Clear();

        foreach (var entry in itemEntries)
        {
            if (!itemList.ContainsKey(entry.ItemType.ToString()))
            {
                itemList.Add(entry.ItemType.ToString(), entry.ItemData);
            }
        }
    }

    private void Start()
    {
        player.PlayerEvents.onSelectSlot += OnQuick;



        Invoke("Cheat", 3);
    }
    void Cheat()
    {
        foreach (var value in itemList)
        {
            AddResource(value.Value.data, 1000);
        }
    }

    // 퀵슬롯 설정
    public void SetQuickSlot(int slotIndex,int itemIndex, ItemInfo info)
    {
        // 등록되어있지 않다면
        if (!quickSlots.ContainsKey(info))
        {
            if (quickSlotsIndex[slotIndex] == null)
            {
                quickSlotsIndex[slotIndex] = info;
                quickSlotManager.SetItemSlot(slotIndex, info, itemCoolTime);
                quickSlots.Add(info, itemIndex);
            }
            else
            {
                var temp = quickSlotsIndex[slotIndex];
                // 해당 자리를 일단 지우기
                quickSlots.Remove(temp);


                quickSlotsIndex[slotIndex] = info;
                quickSlotManager.SetItemSlot(slotIndex, info, itemCoolTime);
                quickSlots.Add(info, itemIndex);
            }

        }
        else
        {
         
            // 만일 해당 슬롯이 비어있다면
            if (quickSlotsIndex[slotIndex] == null)
            {
                int tempInt = 0;
                for(int i = 0; i < quickSlotsIndex.Length; i++)
                {
                    if (quickSlotsIndex[i] == info)
                    {
                        tempInt = i;
                    }
                }

                quickSlotsIndex[tempInt] = null;
                quickSlotManager.ClearItemSlot(tempInt);
                quickSlots.Remove(info);

                quickSlotsIndex[slotIndex] = info;
                quickSlotManager.SetItemSlot(slotIndex, info, itemCoolTime);
                quickSlots.Add(info, itemIndex);

            }
            else
            {
                int tempInt = 0;
                for (int i = 0; i < quickSlotsIndex.Length; i++)
                {
                    if (quickSlotsIndex[i] == info)
                    {
                        tempInt = i;
                    }
                }
                var temp = quickSlotsIndex[slotIndex];

                quickSlotsIndex[tempInt] = temp;
                quickSlotManager.SetItemSlot(tempInt, temp, itemCoolTime);

                quickSlotsIndex[slotIndex] = info;
                quickSlotManager.SetItemSlot(slotIndex, info, itemCoolTime);
            }
        }

    }


    // 기초자원 관련
    public void AddResource(ItemData itemData, int amount)
    {
        if (itemList.ContainsKey(itemData.name))
        {
            // 이미 딕셔너리에 있는 아이템이면 수량만 증가
            itemList[itemData.name].count += amount;
        }

        UpdateResourceInovoke(itemData.name);
    }
    public bool DeductResource(ItemData itemData, int amount)
    {
        // 수량 부족
        if (itemList[itemData.name].count < amount)
        {
            return false;
        }

        // 수량 차감
        itemList[itemData.name].count -= amount;
        UpdateResourceInovoke(itemData.name);
        return true; // 차감 성공
    }

    // 해당 자원의 갯수가 충분한지 체크
    public bool HasResource(ItemData itemData, int amount)
    {
        return itemList[itemData.name].count >= amount;
    }

   private void UpdateResourceInovoke(string name)
    {
        switch (name)
        {

            case "Item_ScrapIron":
                OnScrapChanged.Invoke(itemList[name].count);
                break;
            case "Item_Circuit":
                OnCircuitChanged.Invoke(itemList[name].count);
                break;
            case "Item_Fuel":
                OnFuelChanged.Invoke(itemList[name].count);
                break;
        }
    }

    // 이건 인벤토리에 추가되는 녀석들, 자원은 별도로 AddResource 함수를 만들어야함.
    public void AddItem(ItemData item, int amount)
    {
        int leftAmount = amount;

        // 해당 자원이 리소스면 바로 리소스에 더하고 종료
        if (itemList.ContainsKey(item.name))
        {
            AddResource(item, amount);
            return;
        }

        // 기존 스택에 채우기 시도
        for (int i = 0; i < slotCount; i++)
        {
            if(itemSlots[i] == null)
                continue;
            if (itemSlots[i].data == null)
                continue;
            // 해당 슬롯이 null이 아니고, 같은 아이템이며, 스택에 여유가 있을 때
            if ( itemSlots[i].data.name == item.name && itemSlots[i].count < item.maxStackAmount)
            {
                int canFill = item.maxStackAmount - itemSlots[i].count; // 현재 슬롯에 채울 수 있는 최대량
                int fillAmount = Math.Min(leftAmount, canFill);         

                itemSlots[i].count += fillAmount;
                leftAmount -= fillAmount;
                if (uiInventory != null)
                    uiInventory.UpdateSlotData(i);
                AddUpdater(item);
                if (leftAmount == 0) // 모든 아이템을 다 넣었으면 종료
                {
               
                    return;
                }
            }
        }
        // 남은 아이템이 있다면 빈 슬롯에 새로 추가
        while (leftAmount > 0)
        {
            int amountToPlace = Math.Min(leftAmount, item.maxStackAmount);
            int emptySlotIndex = FindEmptySlot();

            if (emptySlotIndex != -1) // 빈 슬롯을 찾았다면
            {
                itemSlots[emptySlotIndex] = new ItemInfo(item, amountToPlace);
                leftAmount -= amountToPlace;
                if (uiInventory != null)
                    uiInventory.UpdateSlotData(emptySlotIndex);
            }
            else // 빈 슬롯이 없다면
            {
                Console.WriteLine($"인벤토리가 가득 찼습니다. {item.name} {leftAmount}개가 드롭됩니다.");
                return; // 더 이상 아이템을 넣을 수 없으므로 종료
            }
        }
    }

    // 빈 슬롯의 인덱스를 찾아 반환하는 헬퍼 메서드
    private int FindEmptySlot()
    {
        for (int i = 0; i < slotCount; i++)
        {
            if (itemSlots[i] == null ||itemSlots[i].data == null)
            {
                return i;
            }
        }
        return -1; // 빈 슬롯이 없는 경우
    }

    public bool DeductItemBySlot(int slotIndex, int amount)
    {
        // 아이템 차감
        itemSlots[slotIndex].count -= amount;
        if (uiInventory != null)
            uiInventory.UpdateSlotData(slotIndex);
        // 스택이 0이 되면 슬롯을 비움
        if (itemSlots[slotIndex].count <= 0)
        {
            itemSlots[slotIndex].data = null;
            itemSlots[slotIndex].count = 0;
        }
        return true;
    }

    public bool DeductItem(ItemData itemData, int amount)
    {
        int remainingAmount = amount;

        // 차감할 수 있는 아이템 총 수량 확인 
        if (!HasEnoughItem(itemData, amount))
        {
            Console.WriteLine($"아이템 {itemData.name} {amount}개를 차감하기에 수량이 부족합니다.");
            return false;
        }

        // 인벤토리를 순회하며 해당 아이템을 찾아서 차감
        for (int i = 0; i < slotCount; i++)
        {
            if (itemSlots[i] != null && itemSlots[i].data.name == itemData.name && remainingAmount > 0)
            {
                int currentStackCount = itemSlots[i].count;
                int deductFromThisStack = Math.Min(remainingAmount, currentStackCount); // 현재 스택에서 차감할 양

                itemSlots[i].count -= deductFromThisStack;
                remainingAmount -= deductFromThisStack;

                // 스택이 0이 되면 슬롯을 비움 (완전 삭제)
                if (itemSlots[i].count <= 0)
                {
                    itemSlots[i].data = null;
                    itemSlots[i].count = 0;
                }
                if (uiInventory != null)
                    uiInventory.UpdateSlotData(i);
                if (remainingAmount == 0)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool DeductItem(ItemData[] itemsToDeduct, int[] amountsToDeduct)
    {
        if (itemsToDeduct == null || amountsToDeduct == null || itemsToDeduct.Length != amountsToDeduct.Length)
        {
            Console.WriteLine("DeductMultipleItems: 입력값이 유효하지 않습니다.");
            return false;
        }

        // 먼저 모든 아이템의 수량이 충분한지 확인
        for (int i = 0; i < itemsToDeduct.Length; i++)
        {
            if (!HasEnoughItem(itemsToDeduct[i], amountsToDeduct[i]))
            {
                Console.WriteLine($"요청한 {itemsToDeduct[i].name} {amountsToDeduct[i]}개가 인벤토리에 부족합니다. 전체 차감 취소.");
                return false; 
            }
        }

        // 모든 수량이 충분하면 실제 차감 진행
        for (int i = 0; i < itemsToDeduct.Length; i++)
        {
            // 이전에 구현한 단일 아이템 차감 메서드를 재사용
            if (!DeductItem(itemsToDeduct[i], amountsToDeduct[i]))
            {
                Console.WriteLine($"DeductMultipleItems: {itemsToDeduct[i].name} 차감 중 예상치 못한 오류 발생.");
                return false;
            }
        }
        return true; // 모든 아이템 성공적으로 차감
    }

    public bool HasEnoughItem(ItemData itemData, int requiredAmount)
    {
        int totalFoundAmount = 0;
        foreach (var slot in itemSlots)
        {
            
            if (slot != null && slot.data!=null&&slot.data.name == itemData.name)
            {
                totalFoundAmount += slot.count;
            }
        }
        return totalFoundAmount >= requiredAmount;
    }

    // 소모품 아이템 사용
    public void OnUseItem(int index,ItemData data, int amount = 1)
    {
        UpdateQuickSlotDetail(data);
        DeductItemBySlot(index, amount);
        if (uiInventory != null)
            uiInventory.UpdateSlotData(index);
 
        foreach (var value in data.consumables)
        {
            player.ConsumeItem(value);
        }
    }
    // 퀵슬롯으로 아이템 사용
    public void OnQuickUseItem(int quickIndex, int slotindex)
    {
        ItemInfo itemInfo = itemSlots[quickIndex];
        OnUseItem(quickIndex, itemInfo.data, 1);
       // DeductItemBySlot(quickIndex, 1);
        if (uiInventory != null)
            uiInventory.UpdateSlotData(quickIndex);

        quickSlotManager.UpdateStack(slotindex, itemInfo);
        // 0개가 되었으면 퀵슬롯 해체.
        if (itemInfo.count == 0)
        {
            quickSlots.Remove(itemInfo);
            quickSlotsIndex[slotindex] = null;
            quickSlotManager.ClearItemSlot(slotindex);
        }
    }
    public void AddUpdater(ItemData data)
    {
        int tempInt = -1;
        for (int i = 0; i < quickSlotsIndex.Length; i++)
        {
            if (quickSlotsIndex[i] == null)
                continue;

            if (quickSlotsIndex[i].data == data)
            {
                tempInt = i;
            }
        }
        if (tempInt == -1)
            return;

        ItemInfo tempInfo = new ItemInfo(quickSlotsIndex[tempInt].data, quickSlotsIndex[tempInt].count);
        quickSlotManager.UpdateStack(tempInt, tempInfo);

    }
    public void UpdateQuickSlotDetail(ItemData data)
    {
        int tempInt = -1;
        for (int i = 0; i < quickSlotsIndex.Length; i++)
        {
            if (quickSlotsIndex[i] == null)
                continue;

            if (quickSlotsIndex[i].data == data)
            {
                tempInt = i;
            }
        }
        if (tempInt == -1)
            return;

        ItemInfo tempInfo = new ItemInfo(quickSlotsIndex[tempInt].data, quickSlotsIndex[tempInt].count-1);
        quickSlotManager.UpdateStack(tempInt, tempInfo);
        // 0개가 되었으면 퀵슬롯 해체.
        if (tempInfo.count == 0)
        {
            quickSlots.Remove(tempInfo);
            quickSlotsIndex[tempInt] = null;
            quickSlotManager.ClearItemSlot(tempInt);
        }
    }


    public void OnInventory()
    {
        if (isOpenInventory)
        {
            isOpenInventory = false;
            UIManager.instance.ClosePopupUI();
            uiInventory = null;
        }
        else
        {
            isOpenInventory = true;
            var go =  UIManager.instance.ShowPopupUI("UI_Inventory");
            var ui = go.GetComponent<UI_Inventory>();
            uiInventory = go.GetComponent<UI_Inventory>();
            ui.inventoryManager = this;
        }
    }
    public void OnQuick(int index)
    {
        if (quickSlotsIndex[index] != null && uiInventory == null)
        {
            int num = quickSlots[quickSlotsIndex[index]];
            bool isAble;
            if (quickSlotsIndex[index].count != 1)
                isAble = quickSlotManager.CheckQuick(index);
            else
                isAble = quickSlotManager.CheckQuick(index, true);

            if (isAble == true)
                OnQuickUseItem(num, index);
        }
    }

    public bool CheckContainItem(string name)
    {
        if (itemList.ContainsKey(name))
            return true;
        else
            return false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            OnInventory();
        }
    }
}
