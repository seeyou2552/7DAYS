using UnityEngine;

/// <summary>
/// 수리가 가능한 건물의 체력 및 수리 로직을 담당합니다.
/// </summary>
public class RepairableBuilding : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 150f; // 최대 체력 (건물별로 다르게 설정 가능)
    public float currentHealth;    // 현재 체력

    [Header("Repair Settings")]
    public ResourceType requiredResource = ResourceType.Scrap; // 수리에 필요한 자원
    public int costPerRepair = 1;             // 1회 수리당 소모 자원 개수
    public float repairUnitAmount = 10f;      // 1회 수리량

    void Awake()
    {
        // 이미 세팅된 값이 있다면 유지
        if (currentHealth <= 0)
            currentHealth = maxHealth;
    }

    /// <summary>
    /// 수리가 필요한 상태인지 반환합니다.
    /// </summary>
    public bool NeedsRepair => currentHealth < maxHealth;

    /// <summary>
    /// 드론 또는 수리자가 호출하여 수리를 시도합니다.
    /// </summary>
    /// <returns>수리 성공 여부</returns>
    public bool TryRepair()
    {
        if (!NeedsRepair) return false;

        // 자원 소모 확인
        int available = ResourceManager.Instance.GetResource(requiredResource);
        if (available < costPerRepair)
        {
            Debug.LogWarning($"[{gameObject.name}] 수리에 필요한 자원이 부족합니다!");
            return false;
        }

        // 자원 차감 및 수리
        ResourceManager.Instance.AddResource(requiredResource, -costPerRepair);
        currentHealth = Mathf.Min(currentHealth + repairUnitAmount, maxHealth);
        Debug.Log($"[{gameObject.name}] 수리됨: {currentHealth}/{maxHealth}");
        return true;
    }
}