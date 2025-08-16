using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using static DialogueManager;

public class DroneUnit : MonoBehaviour
{
    private DroneHandler droneHandler;
    private BuildingsManager buildingsManager;
    public DroneMode droneMode;

    private Dictionary<ItemData, int> gatherResources;
    private Vector3 initPosition;
    public Vector3 target;
    public float moveSpeed = 3f;
    public float actionCooldown = 2f;
    public int gatherAmount = 1;
    private GameObject billborad;
    [Header("Repair Settings")]
    [SerializeField] ItemData repairResource; // 수리에 필요한 자원
    [SerializeField] int costPerRepair = 1;             // 1회 수리당 소모 자원 개수
    [SerializeField] int repairUnitAmount = 20;      // 1회 수리량
    [SerializeField] float repairPerTime = 3f;

    private int droneIdx;

    private bool isWorking = false;
    public bool IsWorking { get => isWorking; }
    private bool IsIdle => droneMode == DroneMode.Idle;
    private bool IsGathering => droneMode == DroneMode.Gather;
    private bool IsConstructing => droneMode == DroneMode.Construct;
    private bool IsRepairing => droneMode == DroneMode.Repair;
    private bool IsStunned => droneMode == DroneMode.Stun;

    public void Init(DroneHandler droneHandler, int droneIdx)
    {
        this.droneHandler = droneHandler;

        DroneManager.RegisterDrone(transform);
        buildingsManager = BuildingsManager.Instance;
        initPosition = transform.position;
        gatherResources = new Dictionary<ItemData, int>();

        this.droneIdx = droneIdx;
    }

    void OnDestroy()
    {
        DroneManager.UnregisterDrone(transform);
    }

    IEnumerator GatherRoutine()
    {
        if (droneMode != DroneMode.Gather) yield break;

        gatherResources.Clear();

        while (IsGathering)
        {
            target = transform.position + new Vector3(UnityEngine.Random.Range(-150, 150), 0f, UnityEngine.Random.Range(-150, 150));

            while (gatherResources.Count == 0)
            {
                MoveToTarget(target);

                if (Vector3.Distance(transform.position, target) <= 0.9f)
                {
                    gatherResources = ResourceManager.Instance.GetRandomResource();
                }

                yield return null;
            }

            target = initPosition;

            while (gatherResources.Count > 0)
            {
                MoveToTarget(target);

                if (Vector3.Distance(transform.position, target) <= 0.95f)
                {
                    droneHandler.SaveResouceToStorage(gatherResources);
                    gatherResources.Clear();

                    yield return new WaitForSeconds(5f);
                    DestoryDialogue();
                   billborad = DialogueManager.instance.ShowBillBoardDialogue(BillboradName.Dron_Get, this.transform);
                }

                yield return null;
            }

            yield return null;
        }
    }


    IEnumerator RepairRoutine()
    {
        if (droneMode != DroneMode.Repair) yield break;

        while (IsRepairing)
        {
            BaseBuilding building = buildingsManager.GetNeedRepairBuilding();

            if (building == null)
            {
                yield return new WaitForSeconds(5f);
                continue;
            }
            else
            {
                DestoryDialogue();
                billborad = DialogueManager.instance.ShowBillBoardDialogue(BillboradName.Dron_Repair, this.transform);
            }

            target = building.transform.position + Vector3.up * transform.position.y;

            while (Vector3.Distance(transform.position, target) > 0.95f)
            {
                MoveToTarget(target);
                yield return null;
            }
            AudioManager.Instance.PlaySFXLoop("BuildSound");
            bool isDone = false;

            StartCoroutine(RepairBuilding(building, () =>
            {
                isDone = true;
                AudioManager.Instance.StopSFXLoop();
            }));

            yield return new WaitUntil(() => isDone);

        }
    }

    IEnumerator ConstructRoutine()
    {
        if (droneMode != DroneMode.Construct) yield break;

        while (IsConstructing)
        {
            BaseBuilding building = buildingsManager.GetNeedConstructBuilding();
            if (building == null)
            {
                yield return new WaitForSeconds(5f);
                continue;
            }
            else
            {
                DestoryDialogue();
                billborad = DialogueManager.instance.ShowBillBoardDialogue(BillboradName.Dron_Build, this.transform);
            }

            target = building.transform.position + Vector3.up * transform.position.y;

            while (Vector3.Distance(transform.position, target) > 0.95f)
            {
                MoveToTarget(target);
                yield return null;
            }
            AudioManager.Instance.PlaySFXLoop("BuildSound");
            bool isDone = false;
            building.StartConstruct(() => { isDone = true; AudioManager.Instance.StopSFXLoop(); });
     
            yield return new WaitUntil(() => isDone);
        }
    }

    IEnumerator StunRoutine()
    {
        if (droneMode != DroneMode.Stun) yield break;
    }

    public void ChangeMode(DroneMode mode)
    {
        StopAllCoroutines();

        droneMode = mode;
        DestoryDialogue();
        DialogueManager.instance.ShowBillBoardDialogue(BillboradName.Dron_SetWalking, this.transform);
        AudioManager.Instance.PlaySFX("SetDronSound");
        switch (mode)
        {
            case DroneMode.Repair:
                StartCoroutine(RepairRoutine());
                break;

            case DroneMode.Gather:
                StartCoroutine(GatherRoutine());
                break;

            case DroneMode.Construct:
                StartCoroutine(ConstructRoutine());
                break;

            case DroneMode.Stun:
                StartCoroutine(StunRoutine());
                break;

            default:
                droneMode = DroneMode.Idle;
                break;
        }
    }

    IEnumerator RepairBuilding(BaseBuilding building, Action onRepaired)
    {
        if (building == null) yield break;

        while (building.NeedsRepair)
        {
            if (InventoryManager.instance.HasResource(repairResource, costPerRepair))
            {
                building.Fix(repairUnitAmount);
                yield return new WaitForSeconds(repairPerTime);
            }

            else break;
        }

        onRepaired?.Invoke();
    }
    void DestoryDialogue()
    {
        if (billborad != null)
        {
            Destroy(billborad);
        }
    }

    void MoveToTarget(Vector3 target)
    {
        Vector3 dir = (target - transform.position).normalized;
        if (dir != Vector3.zero)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 5f);

        transform.position = Vector3.MoveTowards(transform.position, target, Time.deltaTime * moveSpeed);
    }
}

