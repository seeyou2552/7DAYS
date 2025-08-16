using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CollapserPhaseController : MonoBehaviour
{
    public EBOSSPHASE CurrentPhase { get; private set; } = EBOSSPHASE.PHASE1;
    private Health health;

    private bool isPhase2 = false;
    private bool isPhase3 = false;

    private void Awake()
    {
        health = GetComponent<Health>();
    }
    private void Update()
    {
        float hpRatio = health.CurrentHealth / health.MaxHealth;

        if (hpRatio <= 0.4f && !isPhase3)
        {
            CurrentPhase = EBOSSPHASE.PHASE3;
            isPhase3 = true;
            Phase3();
        }
        else if (hpRatio <= 0.7f && !isPhase2)
        {
            CurrentPhase= EBOSSPHASE.PHASE2;
            isPhase2 = true;
            Phase2();
        }
    }

    private void Phase2()
    {
        Debug.Log("페이즈 2");
        //플레이어 시야 혼선, 드론 혼란상태
        var shaker = Camera.main.GetComponentInChildren<CameraShaker>();
        if (shaker != null)
            shaker.Shake(10f, 0.2f, 20f);

        var Splatter = Camera.main.GetComponent<SplatterEffect>();
        Splatter.ShowSplatter();
    }

    private void Phase3()
    {
        Debug.Log("페이즈 3");
        //플레이어의 분신을 소환해서 공격 / 광역 타격
    }
}
