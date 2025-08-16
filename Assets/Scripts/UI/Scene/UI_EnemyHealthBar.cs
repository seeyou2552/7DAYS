using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_EnemyHealthBar : UI_Scene
{
    AI_Base aiBase;
    Health enemyHealth;
    float maxHealth;
    private Image fillbar;

    enum Images
    {
        EmptyBar,
        FillBar
    }

    public override void Init()
    {
        base.Init();
    }
    private void Start()
    {

        aiBase = transform.parent.GetComponent<AI_Base>();
        enemyHealth = transform.parent.GetComponent<Health>();
        maxHealth = aiBase.enemyData.maxHealth;
        Bind<Image>(typeof(Images));
        fillbar = Get<Image>((int)Images.FillBar);

        enemyHealth.OnTakeDamage += UpdateCurrent;
    }
    private void OnDisable()
    {
        enemyHealth.OnTakeDamage -= UpdateCurrent;
    }
    void UpdateCurrent()
    {
        if (fillbar != null)
        {
            fillbar.fillAmount = enemyHealth.GetHealth() / maxHealth;
        }
    }
   

    public void Update()
    {
        transform.forward = Camera.main.transform.forward;
        if (Input.GetKeyDown(KeyCode.F1))
        {
            enemyHealth.TakeDamage(5);
        }
    }
}
