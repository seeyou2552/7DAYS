using System.Collections;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    [SerializeField] SurvivalConditionSO conditionSO;
    Player player;
    PlayerDataSO playerDataSO;
    PlayerEventHandler playerEvents;

    #region 플레이어 스탯 관련 변수 & 프로퍼티
    public float MoveSpeed { get => IsDanger ? conditionSO.MoveSpeedPenalty : playerDataSO.MoveSpeed; }

    [Header("Player Max Stats")]
    [SerializeField] private float maxHealth;
    [SerializeField] private float maxStamina;

    // 현재 스탯들
    [Header("Player Current Stats")]
    [SerializeField] private float curHealth;
    [SerializeField] private float curStamina;
    [SerializeField] private float curHydration;

    public float CurHealth
    {
        get => curHealth;
        set
        {
            curHealth = Mathf.Clamp(value, 0, maxHealth);
            playerEvents?.RaisedChangeHealth(maxHealth, curHealth);
        }
    }
    public float CurStamina
    {
        get => curStamina;
        set
        {
            curStamina = Mathf.Clamp(value, 0, maxStamina);
            playerEvents?.RaisedChangeStamina(maxStamina, curStamina);
        }
    }
    public float CurHydration
    {
        get => curHydration;
        set
        {
            curHydration = Mathf.Clamp(value, 0, playerDataSO.MaxHydration);
            playerEvents?.RaisedChangeHydration(playerDataSO.MaxHydration, curHydration);
        }
    }

    // 스탯 감소량
    private float healthDecayPerInterval;
    private float staminaDecayPerInterval;
    private float hydrationDecayPerInterval;
    private float HydrationDecayPerInterval => player.OnBattle ? hydrationDecayPerInterval * 5 : hydrationDecayPerInterval;
    private float interval;

    // 체력 회복량
    private float healthRecoverPerInterval;
    private float staminaRecoverPerInterval;
    #endregion

    private bool CanHeal { get => curStamina >= conditionSO.HealthRecoveryThreshold && curHealth < maxHealth; }
    private bool CanRecoverStamina { get => curHydration >= conditionSO.StaminaRecoveryThreshold && curStamina < maxStamina; }
    private bool IsDanger { get => curStamina <= conditionSO.MinStaminaToDecay || curHydration <= conditionSO.MinHydrationToDecay; }

    public void Init(Player player)
    {
        this.player = player;
        playerDataSO = player.PlayerDataSO;
        playerEvents = player.PlayerEvents;

        maxHealth = playerDataSO.MaxHealth;
        curHealth = maxHealth;
        maxStamina = playerDataSO.MaxStamina;
        curStamina = maxStamina;
        curHydration = playerDataSO.MaxHydration;

        healthDecayPerInterval = conditionSO.HealthDecayPerInterval;
        staminaDecayPerInterval = conditionSO.StaminaDecayPerInterval;
        hydrationDecayPerInterval = conditionSO.HydrationDecayPerInterval;

        healthRecoverPerInterval = conditionSO.HealthRecoverPerInterval;
        staminaRecoverPerInterval = conditionSO.StaminaDecayPerInterval;

        interval = conditionSO.Interval;

        StartCoroutine(DecayPerIntervalCoroutine());
    }

    IEnumerator DecayPerIntervalCoroutine()
    {
        while (!player.IsDead)
        {
            CurHydration -= HydrationDecayPerInterval;

            if (player.CurState == PlayerState.Walk)
                CurStamina -= staminaDecayPerInterval;

            if (CanRecoverStamina)
            {
                CurStamina += staminaRecoverPerInterval;
            }

            if (CanHeal)
            {
                CurStamina -= staminaDecayPerInterval;
                CurHealth += healthRecoverPerInterval;
            }

            if (IsDanger)
            {
                CurHealth -= healthDecayPerInterval;
            }

            yield return new WaitForSeconds(interval);
        }
    }

    public void TakeDamage(float amount)
    {
        CurHealth -= amount;

        if (CurHealth <= 0)
        {
            player.Dead();
        }
    }

    public bool UseStamina(float amount)
    {
        if (curStamina >= amount)
        {
            CurStamina -= amount;
            return true;
        }

        return false;
    }

    public bool UseHydration(float amount)
    {
        if (curHydration >= amount)
        {
            CurHydration -= amount;
            return true;
        }

        return false;
    }

    public bool UseStaminaAndHydration(float staminaAmount, float hydrationAmount)
    {
        if (curStamina >= staminaAmount && curHydration >= hydrationAmount)
        {
            CurStamina -= staminaAmount;
            CurHydration -= hydrationAmount;
            return true;
        }

        return false;
    }

    public void SetItemStat(ConsumableType type, float value)
    {
        switch (type)
        {
            case ConsumableType.Health:
                CurHealth += value;
                AudioManager.Instance.PlaySFX("FoodSound");
                break;
            case ConsumableType.Stamina:
                CurStamina += value;
                AudioManager.Instance.PlaySFX("FoodSound");
                break;
            case ConsumableType.Thirst:
                AudioManager.Instance.PlaySFX("DrinkSound");
                CurHydration += value;
                break;
        }
    }
}
