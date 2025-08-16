using System;
using UnityEngine;

[CreateAssetMenu(fileName = "DebuffTurret", menuName = "BuildingData/DebuffTurret")]
public class DebuffTurretForm : BaseBuildingForm<DebuffTurretData>
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
public class DebuffTurretData : CommonBuildingData
{
    public DebuffTurretDataByLevel[] dataByLevel;
}
// 터렛을 상속, 디버프 요소를 추가한 디버프 터렛
[Serializable]
public class DebuffTurretDataByLevel : TurretDataByLevel
{
    [Header("디버프 정보")]
    // !!! 적 머지 이후, 디버프 종류를 나타내는 것 추가 or 디버프 적용 구문을 넣던지 등
    public float debuffTime; // 디버프 지속 시간
}
