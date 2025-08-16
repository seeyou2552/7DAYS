using System.Collections.Generic;
using UnityEngine;

public static class DroneManager
{
    public static List<Transform> AliveDrones = new();
    public static List<Transform> DroneBuildings = new();
    public static void RegisterDrone(Transform drone)
    {
        if (!AliveDrones.Contains(drone))
            AliveDrones.Add(drone);
    }

    public static void UnregisterDrone(Transform drone)
    {
        AliveDrones.Remove(drone);
    }
    public static bool HasAliveDrones => AliveDrones.Count > 0;
    public static Transform ClosestDrone(Vector3 from)
    {
        return GetClosest(from, AliveDrones);
    }

    public static Transform ClosestDroneBuilding(Vector3 from)
    {
        return GetClosest(from, DroneBuildings);
    }

    private static Transform GetClosest(Vector3 from, List<Transform> list)
    {
        Transform closest = null;
        float minDist = float.MaxValue;

        foreach (var t in list)
        {
            if (t == null || !t.gameObject.activeInHierarchy) continue;
            float dist = Vector3.Distance(from, t.position);
            if (dist < minDist)
            {
                closest = t;
                minDist = dist;
            }
        }
        return closest;
    }
}