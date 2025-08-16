using UnityEngine;

// 에너지 공급이 필요한 건물 스크립트에 부착
public interface IBuildingRequireEnegy
{
    // 에너지 공급 유무
    public bool isSupplied {  get; set; }
}

public class InteractArea : MonoBehaviour
{
    [SerializeField] LayerMask buildingLayer;
    float range;
    // 상호작용 가능 영역 크기 변경
    public void ChangeRange(float range)
    { 
        transform.localScale = new Vector3(range, 1, range);
        this.range = range;
    }

    // 영역 내 전력 공급이 필요한 건물들에 전력 공급/끊기
    public void SetEnegyForBuildingsInRange(bool isSupply)
    {
        Collider[] buildings = Physics.OverlapSphere(transform.position, range, buildingLayer);
        foreach (Collider building in buildings) 
        {
            if(building.TryGetComponent(out IBuildingRequireEnegy target))
            {
                target.isSupplied = isSupply;
            }
        }
    }
}
