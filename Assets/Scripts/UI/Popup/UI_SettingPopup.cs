using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_SettingPopup : UI_Popup
{
    [SerializeField]
    private Vector3 pos;
    enum Sliders
    {
        BGMSlider,
        SFXSlider
    }
    enum Buttons
    {
        PointButton,
        GoStartButton
    }
    enum Pivot
    {
        Pivot
    }
    public override void Init()
    {
        base.Init();

        Bind<Slider>(typeof(Sliders));
        Bind<Button>(typeof(Buttons));
        Bind<RectTransform>(typeof(Pivot));

        Get<Button>((int)Buttons.PointButton).onClick.AddListener(OnPointButtonClicked);

        Button goStartButton = Get<Button>((int)Buttons.GoStartButton);
        goStartButton.onClick.AddListener(OnGoMainButtonClicked);
        if (SceneManager.GetActiveScene().name == "Start")
            goStartButton.gameObject.SetActive(false);


        // Slider bgmSlider = Get<Slider>((int)Sliders.BGMSlider);
        //  bgmSlider.onValueChanged.AddListener(OnBgmSliderValueChange);
        // bgmSlider.value = SoundManager.instance.BgmVolume;
        //  Slider sfxSlider = Get<Slider>((int)Sliders.SFXSlider);
        //  sfxSlider.onValueChanged.AddListener(OnSFXSliderValueChange);
        // sfxSlider.value = SoundManager.instance.SfxVolume;

        // 피벗 설정

        SetPivot(pos, Get<RectTransform>((int)Pivot.Pivot));
    }


    protected void OnPointButtonClicked()
    {
        UIManager.instance.ClosePopupUI();
    }
    protected void OnGoMainButtonClicked()
    {
        SceneManager.LoadScene("Start");
    }
    private void OnBgmSliderValueChange(float value)
    {
    //    SoundManager.instance.SetVolume(value, Define.SoundType.BGM);
    }
    private void OnSFXSliderValueChange(float value)
    {
       // SoundManager.instance.SetVolume(value, Define.SoundType.SFX);
    }
}
