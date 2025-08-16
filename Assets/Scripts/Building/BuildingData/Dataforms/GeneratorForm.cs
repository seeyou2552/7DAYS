using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Generator", menuName = "BuildingData/Generator")]
public class GeneratorForm : BaseBuildingForm<GeneratorData>
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

// 발전기 주변에 출력이 닿는 범위 안의 건물만 동작(스타 파일런과 비슷)
[Serializable]
public class GeneratorData : CommonBuildingData
{
    public GeneratorDataByLevel[] dataByLevel;
}

// 발전기의 레벨 별 데이터
[Serializable]
public class GeneratorDataByLevel : BasicBuildingDataByLevel
{
    [Header("발전기 정보"), Tooltip("발전기의 출력이 미치는 범위 = 건설 가능 범위")]
    public float activeRange;
}