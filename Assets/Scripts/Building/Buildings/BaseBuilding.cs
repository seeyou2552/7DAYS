using System;
using System.Collections;
using UnityEngine;

public enum BuildingIndex
{
    SignalTower,
    Wall,
    Turret,
    SlowTurret,
    Generator,
    DroneManageOffice,
    Refinery,
}

// 건물의 기본형
public abstract class BaseBuilding : MonoBehaviour, IDamageable
{
    [SerializeField] protected BuildingIndex buildingIndex;

    protected int level = -1, // 건물 레벨 (지을 때 레벨업으로 0이 됨)
                  levelMax; // 해당 건물 종류의 최대 레벨
    protected float hpCurrent, // 현재 체력
                    hpMax; // 현재 레벨의 최대 체력

    protected bool isConstructing = true; // 건설/업그레이드 중인지 여부 >> 처음 지을 때도 건설해야 하기에 true >> 신호탑, 드론 사무소는 초기부터 false여야 함 !!!
    protected float progressTime = 0, // 건설, 업그레이드 진행 시간
                    requireTime = 0; // 건설/업그레이드 필요 시간
    Coroutine construct; // 진행 중인 건설 코루틴

    public bool NeedsRepair => hpCurrent < hpMax;

    protected virtual void Start()
    {
        Init();
    }

    protected void FixedUpdate()
    {
        if (isConstructing)
            return;

        FixedOverridePart();
    }
    // 픽스드업데이트가 자식들도 건설 중에는 동작하지 않게 처리하기 위해,
    // 해당 조건이 아닐 때 자식들이 오버라이드하여 쓸 부분을 여기에
    protected virtual void FixedOverridePart() { }

    // 건물 처음 생성 때 초기화가 필요한 것 모음
    protected abstract void Init();

    // 레벨업 후 스테이터스 적용
    public virtual void BuildingLvUp()
    {
        if (level < levelMax)
        {
            // 레벨업
            level++;
            // 레벨업 스테이터스 반영
            SetBuildingStatus();
        }
    }

    // 레벨에 맞는 스테이터스 부여
    protected abstract void SetBuildingStatus();

    // 대미지를 받아 체력 감소 및 파괴
    public virtual void TakeDamage(float amount)
    {
        hpCurrent = Mathf.Clamp(hpCurrent - amount, 0, hpMax);
        if (hpCurrent <= 0)
        {
            // 파괴 전에 해당 건물을 건물들 리스트에서 제거
            BuildingsManager.Instance.buildings.Remove(this);
            Destroy(gameObject);
        }
    }

    // 수리
    public void Fix(float amount) => hpCurrent = Mathf.Clamp(hpCurrent + amount, 0, hpMax);

    // 최대 레벨인지 여부 반환 (건물 업그레이드 가능 체크에 사용)
    public bool isMaxLevel() => level.Equals(levelMax);

    // 건물마다 고유로 가지는 값들 반환
    public virtual BuildingStatus GetIndividualBuildingInfo() => new BuildingStatus(level, levelMax, hpCurrent);

    // 자원 소모(건설/업그레이드 가능한 상태로 전환 및 초기 설정)
    public abstract void ResourceConsumption(int nextLevel);
    // 건설 가능한지 체크
    public abstract bool ResourceCheck(int nextLevel);

    // 업그레이드 지시할 때 호출
    public virtual void CallUpgrade()
    {
        // 건설이 필요한 리스트에 추가
        BuildingsManager.Instance.buildingsNeedConstruct.Enqueue(this);
        isConstructing = true;
    }

    // 일꾼이 건설/업그레이드 시작할 때 호출하기
    public void StartConstruct(Action onCompleted)
    {
        // 작업이 가능한 상태고, 기존의 작업이 없다면 >> 드론 작업 시작(이때 드론이 다른 곳으로 못 가도록 해야 함)
        if (isConstructing && construct == null)
            construct = StartCoroutine(DroneWorking(onCompleted));
    }

    // 건설/업그레이드 진행
    IEnumerator DroneWorking(Action onCompleted)
    {
        // 필요 작업 시간까지 대기
        while (progressTime < requireTime)
        {
            yield return new WaitForFixedUpdate();
            progressTime += Time.fixedDeltaTime;
        }
        // 작업 완료
        EndConstruct();
        onCompleted?.Invoke();
    }

    // 건설/업그레이드 종료
    protected virtual void EndConstruct()
    {
        // 건물 청사진 오브젝트가 있다면 건설 완료!
        if(TryGetComponent(out BuildingBluePrint bluePrint))
        {
            bluePrint.BuildComplete();
        }

        // 처음 지어질 때 건물 리스트에 추가
        if (!BuildingsManager.Instance.buildings.Contains(this))
            BuildingsManager.Instance.buildings.Add(this);

        // 작업 종료 상태로 전환
        isConstructing = false;
        progressTime = 0;
        // 레벨업!
        BuildingLvUp();
    }
}

public class BuildingStatus
{
    public int level, // 건물 레벨
               levelMax; // 해당 건물 종류의 최대 레벨
    public float hpCurrent; // 현재 체력

    public BuildingStatus(int level, int levelMax, float hpCurrent)
    {
        this.level = level;
        this.levelMax = levelMax;
        this.hpCurrent = hpCurrent;
    }
}
