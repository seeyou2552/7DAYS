using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UI_StartScene : UI_Scene
{
    enum Texts
    {
        TitleText
    }
    enum Buttons
    {
        NomalModeButton,
        HardModeButton,
        QuitButton,
    }
    public override void Init()
    {
        base.Init();

        Bind<Button>(typeof(Buttons));
        Bind<Text>(typeof(Texts));

        Get<Button>((int)Buttons.NomalModeButton).onClick.AddListener(OnNomalButtonClick);
        Get<Button>((int)Buttons.HardModeButton).onClick.AddListener(OnHardButtonClick);
        Get<Button>((int)Buttons.QuitButton).onClick.AddListener(OnQuitButtonClick);

    }

    protected void OnNomalButtonClick()
    {
        SceneManager.LoadScene("Game");
    }
    protected void OnHardButtonClick()
    {
        SceneManager.LoadScene("Game");

    }
    protected void OnQuitButtonClick()
    {
        Application.Quit();
    }
}
