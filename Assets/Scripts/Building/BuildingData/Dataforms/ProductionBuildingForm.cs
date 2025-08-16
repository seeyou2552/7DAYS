using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ProductionBuilding", menuName = "BuildingData/Production")]
public class ProductionBuildingForm : BaseBuildingForm<ProductionBuildingData>
{
    public override void CreateForm()
    {
        base.CreateForm();
        foreach (var data in dataList)
        {
            DataDic[(int)data.ID] = data;
        }
    }
}


[Serializable]
public class ProductionBuildingData : CommonBuildingData
{
    public ProductionBuildingDataByLevel[] dataByLevel;
}

// 레벨 별 생산 정보
[Serializable]
public class ProductionBuildingDataByLevel : BasicBuildingDataByLevel
{
    [Header("생산 기능 정보")]
    [Tooltip("생산에 필요한 아이템")]
    public ItemData resourceForProduct;
    [Tooltip("생산품")]
    public ItemData product;
    [Tooltip("생산 시간")]
    public float productionTime;
    [Tooltip("한번에 생산하는 양")]
    public int amount;
    [Tooltip("생산 적재 한계치")]
    public int capacity;
}