using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DronFactory : MonoBehaviour
{
    void OnEnable()
    {
        DroneManager.DroneBuildings.Add(transform);
    }

    void OnDestroy()
    {
        DroneManager.DroneBuildings.Remove(transform);
    }

    private void OnDisable()
    {
        DroneManager.DroneBuildings.Remove(transform);
    }
}
