using System;
using UnityEngine;

// 상호작용 가능한 건물들에 부여
public interface IInteractactble
{
    public abstract void OnInteract();
}

public class Refinery : BaseBuilding, IInteractactble, IBuildingRequireEnegy
{
    public ProductionBuildingData data { get; private set; }
    public bool isSupplied { get; set; }
    public ItemData water;
    // 제작 진행 시간, 제작에 필요한 시간
    [SerializeField] float progressProduction, requireProduction;
    // 생산된 양
    public int productAmount = 0;
    public int inputAmount = 0;

    public event Action<int, int> onChangeStack;

    protected override void Init()
    {
        // 설치 때는 전력 공급을 받는 영역에만 설치가 가능하기에
        isSupplied = true;
        // 데이터 받아오기
        data = FormManager.Instance.GetForm<ProductionBuildingForm>().GetDataByID((int)buildingIndex);
        // 최대 레벨
        levelMax = data.dataByLevel.Length - 1;
        // 건설 필요 시간 써주기
        requireTime = data.dataByLevel[0].time;
    }

    protected override void FixedOverridePart()
    {
        // 전력 공급이 될 때만 동작
        if (isSupplied)
        {
            // 시간을 세어주다, 생산 필요 시간을 넘었다면 생산!
            progressProduction += Time.fixedDeltaTime;
            if (progressProduction > requireProduction)
            {
                // 생산에 필요한 자원의 소모에 성공했다면
                if (inputAmount > 0)
                {
                    progressProduction = 0; // 시간 초기화
                    productAmount++;
                    inputAmount -= 1;
                    onChangeStack?.Invoke(inputAmount, productAmount);
                }
            }
        }
    }

    protected override void SetBuildingStatus()
    {
        // 레벨업으로 인한 최대 HP 증가
        hpMax = hpCurrent = data.dataByLevel[level].hpMax;
        // 레벨업으로 변한 생산 시간 반영
        requireProduction = data.dataByLevel[level].productionTime;
    }

    // 생산에 필요한 자원이 있다면 소모하고 true, 없으면 false 반환
    public bool TryConsumeForProduct()
    {
        var isActive = InventoryManager.instance.DeductItem(water, 1);
        if (isActive == false)
            return false;
        inputAmount += 1;
        onChangeStack.Invoke(inputAmount, productAmount);
        return true;
    }

    // 생산량만큼 건물에 적재
    public void Production() => productAmount = Mathf.Clamp(productAmount + data.dataByLevel[level].amount, 0, data.dataByLevel[level].capacity);

    // 생산한 아이템을 인벤토리에 넣게끔
    public void OnInteract()
    {
        InventoryManager.instance.AddResource(data.dataByLevel[level].product, productAmount);
        productAmount = 0;
        onChangeStack.Invoke(inputAmount, productAmount);
    }

    // 건물마다 고유로 가지는 값들 반환
    public override BuildingStatus GetIndividualBuildingInfo() => new ProductBuildingStatus(level, levelMax, hpCurrent, progressTime, productAmount);

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

    private bool isOpenUI = false;
    private UI_Refinery refinery;
    public void ShowUI()
    {
        isOpenUI = true;
        var go = UIManager.instance.ShowPopupUI("UI_Refinery", null, null, true);
        var ui = go.GetComponent<UI_Refinery>();
        refinery = ui;
        ui.refinery = this;
    }
}

public class ProductBuildingStatus : BuildingStatus
{
    public float progressTime; // 제작 진행 시간
    public int productAmount; // 생산된 양

    public ProductBuildingStatus(int level, int levelMax, float hpCurrent, float progressTime, int productAmount) : base(level, levelMax, hpCurrent)
    {
        this.progressTime = progressTime;
        this.productAmount = productAmount;
    }
}
