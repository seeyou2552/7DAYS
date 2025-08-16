using System.Collections;
using UnityEngine;

public class Wall : BaseBuilding
{
    public BasicBuildingData data { get; protected set; }

    protected override void Init()
    {
        // 데이터 받아오기
        data = FormManager.Instance.GetForm<WallForm>().GetDataByID((int)buildingIndex);
        // 최대 레벨
        levelMax = data.dataByLevel.Length - 1;
        // 건설 필요 시간 써주기
        requireTime = data.dataByLevel[0].time;
    }

    protected override void SetBuildingStatus()
    {
        // 레벨업으로 인한 최대 HP 증가
        hpMax = hpCurrent = data.dataByLevel[level].hpMax;
    }

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

    [ContextMenu("TestHit")]
    public void TestHit()
    {
        hpCurrent -= 100;
    }
}


