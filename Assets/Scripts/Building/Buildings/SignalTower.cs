using System.Collections;
using System.Collections.Generic;
public class SignalTower : Wall
{
    protected override void Init()
    {
        // 신호탑은 초기에 지어져 있기에 건설중이 아닌 상태
        level = 0;
        isConstructing = false;
        // 데이터 받아오기
        data = FormManager.Instance.GetForm<WallForm>().GetDataByID((int)buildingIndex);
        // 최대 레벨
        levelMax = data.dataByLevel.Length - 1;
        // 건설 필요 시간 써주기 >> 신호탑은 1레벨부터
        requireTime = data.dataByLevel[1].time;
        SetBuildingStatus();
    }

    private void OnDestroy()
    {
        // !!! 게임 오버 구문 넣기!
    }
}

