using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class T_PC : MonoBehaviour
{
    public static T_PC instance;
    public event Action<float,float> OnHealthChanged;
    public event Action<float,float> OnStaminaChanged;
    public InventoryManager inventoryManager;
    public ItemData[] items;
    public ItemData[] resources;


    private void Awake()
    {
        // 싱글톤
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
            Destroy(this.gameObject);
    }
    public float _maxHealth;
    public float _health;
    public float Health
    {
        get => _health;
        set
        {
            _health = value;
            OnHealthChanged?.Invoke(_maxHealth,_health);
        }
    }
    public float _maxStamina;
    public float _stamins;
    public float Stamina
    {
        get => _stamins;
        set
        {
            _stamins = value;
            OnStaminaChanged?.Invoke(_maxStamina, _stamins);
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            Stamina -= 10;
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            Stamina += 10;
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
          //  
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            // 이부분의 UI 반영은 나중에 인벤토리에 넣어주어야함.
            inventoryManager.AddItem(items[0], 1);
            inventoryManager.AddItem(items[1], 1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            // 이부분의 UI 반영은 나중에 인벤토리에 넣어주어야함.
            inventoryManager.DeductItem(items[0], 100);
            inventoryManager.DeductItem(items[1], 100);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            // 이부분의 UI 반영은 나중에 인벤토리에 넣어주어야함.
            inventoryManager.AddResource(resources[0], 100);
            inventoryManager.AddResource(resources[1], 100);
            inventoryManager.AddResource(resources[2], 100);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            // 이부분의 UI 반영은 나중에 인벤토리에 넣어주어야함.
            inventoryManager.DeductResource(resources[0], 100);
            inventoryManager.DeductResource(resources[1], 100);
            inventoryManager.DeductResource(resources[2], 100);
        }

        // 방향 입력
        float horizontal = Input.GetAxisRaw("Horizontal"); // A/D 또는 왼/오
        float vertical = Input.GetAxisRaw("Vertical");     // W/S 또는 위/아래

        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        // 움직이기 (Transform 기준)
        transform.position += direction * moveSpeed * Time.deltaTime;
    }

    public float moveSpeed = 5f;

  
}
