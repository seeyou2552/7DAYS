using System;
using UnityEngine;

[CreateAssetMenu(fileName = "VehicleData", menuName = "Vehicle/New VehicleData")]
public class VehicleDataSO : ScriptableObject
{
    [Header("Vehicle Movement")]
    [SerializeField] float speed;
    [SerializeField] float turnSpeed;
    public float Speed { get => speed; }
    public float TurnSpeed { get => turnSpeed; }
}
