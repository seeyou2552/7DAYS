using System;
using TMPro;
using UnityEngine;

public class DroneSlot : MonoBehaviour
{
    private DroneUI droneUI;
    private int slotIdx;

    [SerializeField] TMP_Dropdown dropdown;

    public void Init(DroneUI droneUI, int slotIdx)
    {
        dropdown = GetComponentInChildren<TMP_Dropdown>();

        this.droneUI = droneUI;
        this.slotIdx = slotIdx;

        if (dropdown != null)
        {
            dropdown.onValueChanged.AddListener(CommandToDrone);
        }
    }

    void CommandToDrone(int mode)
    {
        droneUI.CommandToDrone(slotIdx, mode);
    }
}
