using System.Collections.Generic;
using UnityEngine;

public enum DroneMode
{
    Idle = 0,
    Gather,
    Construct,
    Repair,
    Stun
}

public class DroneHandler : MonoBehaviour
{
    private DroneManagerOffice droneManagerOffice;
    private List<DroneUnit> activeDrones = new List<DroneUnit>();

    [SerializeField] Transform spawnTransform;

    DroneUnit selectedDrone;
    private int droneIdx;

    public void Init(DroneManagerOffice droneManagerOffice)
    {
        this.droneManagerOffice = droneManagerOffice;
    }

    public void GenerateDrone(GameObject dronePrefab)
    {
        Vector3 spawnPostion = spawnTransform.position + new Vector3(Random.Range(-5, 5), 0f, Random.Range(-5, 5));
        GameObject drone = Instantiate(dronePrefab, spawnPostion, Quaternion.identity);

        if (drone.TryGetComponent(out DroneUnit droneUnit))
        {
            droneUnit.Init(this, droneIdx);
            activeDrones.Add(droneUnit);
            DroneManager.AliveDrones.Add(droneUnit.transform);

            droneIdx++;
        }
    }

    public bool SelectDrone(int idx)
    {
        if (idx <= -1 || idx >= activeDrones.Count) return false;

        selectedDrone = activeDrones[idx];

        return true;
    }

    public void ChangeDroneMode(int idx, DroneMode mode)
    {
        if(SelectDrone(idx))
            selectedDrone.ChangeMode(mode);
    }

    public void SaveResouceToStorage(Dictionary<ItemData, int> gatherResource)
    {
        if (gatherResource != null)
            droneManagerOffice.SaveResouce(gatherResource);
    }
}
