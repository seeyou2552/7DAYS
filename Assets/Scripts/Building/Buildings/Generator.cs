using UnityEngine;

public class Generator : BaseBuilding
{
    [SerializeField]
    InteractArea interactArea;
    public GeneratorData data { get; private set; }


    protected override void Init()
    {
        // 데이터 받아오기
        data = FormManager.Instance.GetForm<GeneratorForm>().GetDataByID((int)buildingIndex);
        // 최대 레벨
        levelMax = data.dataByLevel.Length - 1;
        // 건설 필요 시간 써주기
        requireTime = data.dataByLevel[0].time;
    }

    protected override void SetBuildingStatus()
    {
        // 레벨업으로 인한 최대 HP 증가
        hpMax = hpCurrent = data.dataByLevel[level].hpMax;
        // 발전기 적용 범위 변경
        interactArea.ChangeRange(data.dataByLevel[level].activeRange);
    }

    // 건설 기능 시작/종료 때 호출 >> 상호작용 영역 및 표시 활성화/비활성화
    public void EnableGeneratorZone() => interactArea.gameObject.SetActive(true);
    public void DisableGeneratorZone() => interactArea.gameObject.SetActive(false);

    // 건물마다 고유로 가지는 값들 반환
    public override BuildingStatus GetIndividualBuildingInfo() => new GeneratorStatus(level, levelMax, hpMax);

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

    // 대미지를 받아 체력 감소 및 파괴
    public override void TakeDamage(float amount)
    {
        hpCurrent = Mathf.Clamp(hpCurrent - amount, 0, hpMax);
        if (hpCurrent <= 0)
        {
            // 전력 공급 끊기
            interactArea.SetEnegyForBuildingsInRange(false);
            // 매니저에 등록된 해당 발전기 제외
            GeneratorManager.Instance.DestroyGenerator(this);
            Destroy(gameObject);
        }
    }

    protected override void EndConstruct()
    {
        base.EndConstruct();
        // 영역 내 전력 공급 시작
        interactArea.SetEnegyForBuildingsInRange(true);
        // 발전기 추가
        GeneratorManager.Instance.BuildGenerator(this);
    }
}

public class GeneratorStatus : BuildingStatus
{
    public GeneratorStatus(int level, int levelMax, float hpCurrent) : base(level, levelMax, hpCurrent)
    {
    }
}
