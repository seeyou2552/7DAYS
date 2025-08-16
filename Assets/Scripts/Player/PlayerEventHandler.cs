using System;

public class PlayerEventHandler
{
    public event Action<float, float> onChangeHealth;
    public event Action<float, float> onChangeStamina;
    public event Action<float, float> onChangeHydration;
    public event Action onDash;
    public event Action<int> onSelectSlot;
    public event Action<WeaponType> onWeaponUsed;
    public void RaisedChangeHealth(float maxValue, float curValue) => onChangeHealth?.Invoke(maxValue, curValue);
    public void RaisedChangeStamina(float maxValue, float curValue) => onChangeStamina?.Invoke(maxValue, curValue);
    public void RaisedChangeHydration(float maxValue, float curValue) => onChangeHydration?.Invoke(maxValue, curValue);
    public void RaisedDash() => onDash?.Invoke();
    public void RaisedSelectSlot(int idx) => onSelectSlot?.Invoke(idx);
    public void RaisedWeaponUsed(WeaponType type) => onWeaponUsed?.Invoke(type);
}
