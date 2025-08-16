using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Turret", menuName = "BuildingData/Turret")]
public class TurretForm : BaseBuildingForm<TurretData>
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
public class TurretData : CommonBuildingData
{
    public TurretDataByLevel[] dataByLevel;
}
// 터렛의 레벨 별 데이터
[Serializable]
public class TurretDataByLevel : BasicBuildingDataByLevel
{
    [Header("자동 포탑 정보")]
    // 타워에 필요한 요소인 공격력, 공격 딜레이, 사거리 추가
    public float atk;
    public float atkDelay;
    public float range;
}
