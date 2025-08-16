using System.Collections.Generic;
using UnityEngine;

public class DroneUI : MonoBehaviour
{
    DroneManagerOffice droneManagerOffice;
    [SerializeField] GameObject droneSlotPrefab;
    [SerializeField] Transform contents;

    private List<DroneSlot> droneSlots;
    private int slotIdx;

    public void Init(DroneManagerOffice droneManagerOffice)
    {
        this.droneManagerOffice = droneManagerOffice;

        droneSlots = new List<DroneSlot>();
        slotIdx = 0;
    }

    /// <summary>
    /// UI 드론 슬롯 추가
    /// </summary>
    public void AddDroneSlot()
    {
        GameObject obj = Instantiate(droneSlotPrefab, contents);

        if (obj.TryGetComponent(out DroneSlot slot))
        {
            slot.Init(this, slotIdx);
            droneSlots.Add(slot);

            slotIdx++;
        }
    }

    public void CommandToDrone(int idx, int mode)
    {
        droneManagerOffice.SelectAndCommand(idx, (DroneMode)mode);
    }

}
