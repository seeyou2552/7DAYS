using JetBrains.Annotations;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "WallOrSignalTower", menuName = "BuildingData/WallOrSignalTower")]
public class WallForm : BaseBuildingForm<BasicBuildingData>
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
public class BasicBuildingData : CommonBuildingData
{
    public BasicBuildingDataByLevel[] dataByLevel;
}

[Serializable]
public class CommonBuildingData
{
    public BuildingIndex ID; // 건물 ID
    public string name; // 건물 이름
    public string description; // 건물 설명
    public Sprite sprite; // 건물 이미지
}

// 레벨 별로 달라질 수 있는 데이터들의 공통된 부분만 묶은 기본형
[Serializable]
public class BasicBuildingDataByLevel
{
    public float time; // 건설 시간
    public int hpMax; // 체력
    public ResourceRequire[] resources; // 건설에 필요한 자원들 종류 및 갯수
}

// 건설에 필요한 한 종류의 자원과 양
[Serializable]
public class ResourceRequire
{
    // 자원 종류
    public ItemData resourceSort;
    // 필요 갯수
    public int amount;
}