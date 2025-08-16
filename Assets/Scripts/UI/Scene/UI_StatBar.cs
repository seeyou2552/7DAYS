using UnityEngine;
using UnityEngine.UI;

public class UI_StatBar : UI_Scene
{
    private PlayerEventHandler eventHandler;
    private Image fillbar;

    public enum StatBarType
    {
        Health,
        Stamina,
        Thirst,
    }

    [SerializeField] private StatBarType statBarType;

    enum Images
    {
        EmptyBar,
        FillBar
    }


    public override void Init()
    {
        base.Init();
        eventHandler = InventoryManager.instance.player.PlayerEvents;
        Bind<Image>(typeof(Images));
        fillbar = Get<Image>((int)Images.FillBar);
        switch (statBarType)
        {
            case StatBarType.Health:
                eventHandler.onChangeHealth += UpdateCurrent;
                break;
            case StatBarType.Stamina:
                eventHandler.onChangeStamina += UpdateCurrent;
                break;
            case StatBarType.Thirst:
                eventHandler.onChangeHydration += UpdateCurrent;
                break;
        }
    }

    private void OnDisable()
    {
        switch (statBarType)
        {
            case StatBarType.Health:
                eventHandler.onChangeHealth -= UpdateCurrent;
                break;
            case StatBarType.Stamina:
                eventHandler.onChangeStamina -= UpdateCurrent;
                break;
            case StatBarType.Thirst:
                eventHandler.onChangeHydration -= UpdateCurrent;
                break;
        }
    }

    void UpdateCurrent(float max,float current)
    {
        if (fillbar != null)
        {
            fillbar.fillAmount = current / max;
        }
    }
}
