using System;
using System.Collections.Generic;
using UnityEngine;

public class DroneManagerOffice : BaseBuilding
{
    public WorkerOfficeData data { get; private set; }

    private Dictionary<ItemData, int> resourceStorage;
    private DroneHandler droneHandler;
    [SerializeField] private DroneUI droneUI;

    protected override void Init()
    {
        level = 0;

        if (TryGetComponent(out droneHandler))
        {
            droneHandler.Init(this);
        }

        droneUI.Init(this);

        // 드론사무소도 처음에 지어져 있기에 false
        isConstructing = false;
        // 데이터 받아오기
        data = FormManager.Instance.GetForm<WorkerOfficeForm>().GetDataByID((int)buildingIndex);
        // 최대 레벨
        levelMax = data.dataByLevel.Length - 1;
        // 건설 필요 시간 써주기
        requireTime = data.dataByLevel[1].time;
        SetBuildingStatus();

        resourceStorage = new Dictionary<ItemData, int>();

        DroneManager.DroneBuildings.Add(transform);

    }

    protected override void SetBuildingStatus()
    {
        // 레벨업으로 인한 최대 HP 증가
        hpMax = hpCurrent = data.dataByLevel[level].hpMax;

        for (int i = 0; i < data.dataByLevel[level].workerCount; i++)
        {
            GenerateWorker();
            droneUI.AddDroneSlot();
        }
    }

    // 건물마다 고유로 가지는 값들 반환
    public override BuildingStatus GetIndividualBuildingInfo() => new WorkerOfficeStatus(level, levelMax, hpMax);

    public override void ResourceConsumption(int nextLevel)
    {
        // 건설/업그레이드에 필요한 자원이 충분치 않다면 종료
        if (!ResourceCheck(nextLevel))
            return;

        ResourceRequire[] resourcesRequire = data.dataByLevel[nextLevel].resources;
        foreach (ResourceRequire resourceRequire in resourcesRequire)
        {
            InventoryManager.instance.DeductResource(resourceRequire.resourceSort, resourceRequire.amount);
        }
        
        // 건설 필요 시간 써주기
        requireTime = data.dataByLevel[nextLevel].time;
        // 건설 상태
        isConstructing = true;
    }

    // 해당 레벨로의 건설/업그레이드에 필요한 자원이 충분한지 여부
    public override bool ResourceCheck(int nextLevel)
    {
        ResourceRequire[] resourcesRequire = data.dataByLevel[nextLevel].resources;
        foreach (ResourceRequire resourceRequire in resourcesRequire)
        {
            if (!InventoryManager.instance.HasResource(resourceRequire.resourceSort, resourceRequire.amount))
                return false;
        }
        return true;
    }

    public void SaveResouce(Dictionary<ItemData, int> gatherResources)
    {
        foreach (var resource in gatherResources.Keys)
        {
            if (resourceStorage.ContainsKey(resource))
            {
                resourceStorage[resource] += gatherResources[resource];
            }
            else
            {
                resourceStorage.Add(resource, gatherResources[resource]);
            }
        }

        GetAllResources();
    }

    public void GetAllResources()
    {
        InventoryManager inventory = InventoryManager.instance;

        foreach (var resource in resourceStorage.Keys)
        {
            inventory.AddItem(resource, resourceStorage[resource]);
        }

        resourceStorage.Clear();
    }

    public void GenerateWorker()
    {
        droneHandler.GenerateDrone(data.WorkerPrefab);
    }

    public void OnDroneUI()
    {
        droneUI.gameObject.SetActive(true);    
    }

    public void SelectAndCommand(int idx, DroneMode mode)
    {
        droneHandler.ChangeDroneMode(idx, mode);
    }
}

public class WorkerOfficeStatus : BuildingStatus
{
    public WorkerOfficeStatus(int level, int levelMax, float hpCurrent) : base(level, levelMax, hpCurrent)
    {
        
    }
}