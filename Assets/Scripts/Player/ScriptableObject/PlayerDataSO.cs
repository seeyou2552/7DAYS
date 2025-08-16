using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Player/New PlayerData")]
public class PlayerDataSO : ScriptableObject
{
    [Header("Player Stats")]
    [SerializeField] float maxHealth;
    [SerializeField] float maxStamina;
    [SerializeField] float maxHydration;
    public float MaxHealth { get => maxHealth; }
    public float MaxStamina { get => maxStamina; }
    public float MaxHydration { get => maxHydration; }

    [Header("Player Movement")]
    [SerializeField] float moveSpeed;
    [SerializeField] float dashSpeed;
    [SerializeField] float dashDuration;
    [SerializeField] float dashCoolDown;
    [SerializeField] float dashStamina;
    [SerializeField] float dashHydration;
    public float MoveSpeed { get => moveSpeed; }
    public float DashSpeed { get => dashSpeed; }
    public float DashDuration { get => dashDuration; }
    public float DashCoolDown { get => dashCoolDown; }
    public float DashStamina { get => dashStamina; }
    public float DashHydration { get => dashHydration; }

    [Header("Gathering")]
    [SerializeField] float gatherStamina;
    public float GatherStamina { get => gatherStamina; }
}
