using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "EnemyAI/EnemyType")]
public class SO_EnemyAI : ScriptableObject
{
    public EENEMYTYPE type;
    public float maxHealth;
    public float attackPower;
    public float attackCoolTime;
    [SerializeField, Range(1, 5)] public float moveSpeed;
    public bool canClimbWall;
    public bool canJumpOverWall;
    public bool isStealth;
    public bool isRanged;
    public bool targetsDronesFirst;
    public float attackRange;
    public float chasingRange;
    public float wallDetectDistance;
    public float StealthTime = 5f;
    public float StealthRange = 10f;
    public float StealthDurtaion = 5f;
}
