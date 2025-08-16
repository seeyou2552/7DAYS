using UnityEngine;
public class Turret : BaseBuilding, IBuildingRequireEnegy
{
    // 포탑의 상부(상하 회전용), 총알 발사 위치
    [SerializeField] Transform top, firePos;
    [SerializeField] Bullet bulletPrefab; // 총알 프리팹
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] ParticleSystem vfx;
    // 현재 참조중인 터렛 데이터
    public TurretData data { get; private set; }
    public bool isSupplied { get; set; }

    // 자주 찾을 값들은 변수에 넣고 레벨업 때마다 경신
    float count, atk, atkDelay, range;
    // 적 탐색 주기 >> 너무 짧으면 성능을 많이 먹기에 부자연스럽지 않으면서도 적당한 주기로 탐색
    const float searchDelay = 0.2f;
    
    Transform target;

    protected override void Init()
    {
        // 설치 때는 전력 공급을 받는 영역에만 설치가 가능하기에
        isSupplied = true;
        // 데이터 받아오기
        data = FormManager.Instance.GetForm<TurretForm>().GetDataByID((int)buildingIndex);
        // 최대 레벨
        levelMax = data.dataByLevel.Length - 1;
        // 주기적으로 타겟 지정
        InvokeRepeating("FindClosestTarget", 0, searchDelay);
    }

    protected override void SetBuildingStatus()
    {
        // 레벨업으로 인한 최대 HP 증가
        hpMax = hpCurrent = data.dataByLevel[level].hpMax;
        // 공격력
        atk = data.dataByLevel[level].atk;
        // 공격 딜레이
        atkDelay = data.dataByLevel[level].atkDelay;
        // 사거리
        range = data.dataByLevel[level].range;
    }

    protected override void FixedOverridePart()
    {
        // 전력이 공급 중이고, 타겟이 살아있다면
        if (isSupplied && target != null && target.gameObject.activeSelf)
        {
            // 타겟을 바라보도록 추적
            LookTarget();
            // 공격 딜레이 세어주다가 공격
            count += Time.fixedDeltaTime;
            if (count > atkDelay)
            {
                Attack();
                count = 0;
            }
        }
        else
        {
            target = null;
        }
    }

    // 공격
    void Attack()
    {
        // VFX 재생
        vfx.Play();
        // SFX 재생
        AudioManager.Instance.PlaySFX("ShootSound");
        // 총알 생성
        Bullet bullet = Instantiate(bulletPrefab);
        // 초기 스텟 부여
        bullet.InitBullet(atk, target);
        // 초기 위치로 이동
        bullet.transform.position = firePos.position;
    }

    // 포탑의 사거리 내 가장 가까운 적 탐지
    Transform FindClosestTarget()
    {
        int closestIndex = 0;
        float closestDist = range; 
        Collider[] enemiesInRange = Physics.OverlapSphere(transform.position, range, enemyLayer);
        if (!enemiesInRange.Length.Equals(0))
        {
            int i = 0;
            for (; i < enemiesInRange.Length; i++)
            {
                float dist = Vector3.SqrMagnitude(enemiesInRange[i].transform.position - transform.position);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closestIndex = i;
                }
            }
            target = enemiesInRange[closestIndex].transform;
        }

        return null;
    }

    // 터렛이 타겟을 바라보도록 회전
    void LookTarget()
    {
        // 1. 좌우 회전
        // 타겟 방향
        Vector3 directionBase = target.position - transform.position;
        // Y축 고정 (xz 평면 수평 방향만 사용)
        directionBase.y = 0; 
        // 목표 방향
        Quaternion lookRotationBase = Quaternion.LookRotation(directionBase);
        // 타겟 방향으로 포탑이 회전(조금만 움직여도 되는데 반대로 회전해버리는 상황을 방지하기 위해 Slerp 사용)
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotationBase, 0.1f);
    }

    // 건물마다 고유로 가지는 값들 반환
    public override BuildingStatus GetIndividualBuildingInfo() => new TurretStatus(level, levelMax, hpCurrent);

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

    // 공격 딜레이 변화/복원
    public void DecreaseDelay(float ratio)=> atkDelay *= ratio;
    public void RestoreDelay() => atkDelay = data.dataByLevel[level].atkDelay;
}

public class TurretStatus : BuildingStatus
{
    public TurretStatus(int level, int levelMax, float hpCurrent) : base(level, levelMax, hpCurrent)
    {
    }
}
