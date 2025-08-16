using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UI_QuickSlotManager : MonoBehaviour
{
    private WeaponDataSO[] weaponDataSO;
    private PlayerEventHandler eventHandler;
    private PlayerDataSO playerData;
    private PlayerWeaponHandler weaponHandler;
    private InventoryManager inventoryManager;
    // 대쉬 이미지는 고정
    [SerializeField] private Sprite dashSprite;
    [SerializeField] private Sprite skillASprite;
    [SerializeField] private Sprite skillBSprite;
    [SerializeField] private UI_QuickSlot[] itemSlots;

    private int dashIndex = 4;
    private int buffWeaponIndex = 5;
    private int debuffWeaponIndex = 6;

    private void Start()
    {
        InventoryManager.instance.quickSlotManager = this;
        inventoryManager = InventoryManager.instance;
        playerData = inventoryManager.player.PlayerDataSO;
        eventHandler= inventoryManager.player.PlayerEvents;
        weaponDataSO = inventoryManager.weaponDataSO;

        SetSkillSlot(dashIndex, dashSprite, playerData.DashCoolDown);
        SetSkillSlot(buffWeaponIndex, skillASprite, weaponDataSO[0].cooldown);
        SetSkillSlot(debuffWeaponIndex, skillBSprite, weaponDataSO[1].cooldown);

        eventHandler.onDash += OnDash;
        eventHandler.onWeaponUsed += OnSkill;

    }

    public void SetItemSlot(int index, ItemInfo info, float cooldown = 0)
    {
        itemSlots[index].SetSlot(info, cooldown);
    }
    public void ClearItemSlot(int index)
    {
        itemSlots[index].ClearSlot();
    }

    public void SetSkillSlot(int index, Sprite sprite ,float cooldown)
    {
        itemSlots[index].SetSlot(sprite, cooldown);
    }


    public void OnSlotPressed(int index)
    {
        itemSlots[index].TriggerCooldown();
    }
    // 추후 플레이어컨트롤러와 연결해주어야함.
    public void OnDash()
    {
        itemSlots[dashIndex].TriggerCooldown();
    }
    public void OnSkill(WeaponType type)
    {
        switch (type)
        {
            case WeaponType.Buff:
                itemSlots[buffWeaponIndex].TriggerCooldown();
                break;
            case WeaponType.Debuff:
                itemSlots[debuffWeaponIndex].TriggerCooldown();
                break;
        }
    }

    public bool CheckQuick(int index, bool isEnd = false)
    {
         return itemSlots[index].TriggerCooldown(isEnd);
    }
    public void UpdateStack(int index, ItemInfo info)
    {
        itemSlots[index].UpdateStack(info);
    }


}
