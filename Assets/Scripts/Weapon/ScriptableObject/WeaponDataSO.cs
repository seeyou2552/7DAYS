using UnityEngine;

public enum WeaponType { Buff, Debuff }

[System.Serializable]
public struct BuffEffect
{
    public float attackSpeedMultiplier; 
}

[System.Serializable]
public struct DebuffEffect
{
    public float speedMultiplier;
}

[CreateAssetMenu(fileName = "WeaponData", menuName = "Weapon/New WeaponData")]
public class WeaponDataSO : ScriptableObject
{
    [Header("Weapon Type")]
    public WeaponType weaponType;

    [Header("Weapon Stats")]
    public float range;  
    public float duration;    
    public float cooldown;
    public float useStamina;
    public float explosionDelay;

    [Header("Effect")]
    public BuffEffect buffEffect;
    public DebuffEffect debuffEffect;

    [Header("Unlock")]
    public string unlockCondition; 
}
