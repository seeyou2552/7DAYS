using System;
using UnityEngine;

[CreateAssetMenu(fileName = "WorkerOffice", menuName = "BuildingData/WorkerOffice")]
public class WorkerOfficeForm : BaseBuildingForm<WorkerOfficeData>
{
    public override void CreateForm()
    {
        base.CreateForm();
        foreach(var data in dataList)
        {
            DataDic[(int)data.ID] = data;
        }
    }
}

// 일꾼을 관리하는 사무소
[Serializable]
public class WorkerOfficeData : CommonBuildingData
{
    public GameObject WorkerPrefab;
    public WorkerOfficeDataByLevel[] dataByLevel;
}
// 레벨 별 일꾼 정보
[Serializable]
public class WorkerOfficeDataByLevel : BasicBuildingDataByLevel
{
    public int workerCount;
}
