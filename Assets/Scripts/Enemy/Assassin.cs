using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Assassin : MonoBehaviour
{
    private Renderer[] renderers;
    private Collider[] colliders;

    private float fadeDuration = 0.5f;

    private bool fadeLoopStarted = false;
    private AI_Stealth stealth; 


    void Start()
    {
        stealth = GetComponent<AI_Stealth>();   
        renderers = GetComponentsInChildren<Renderer>();
        colliders = GetComponentsInChildren<Collider>();
    }


    void Update()
    {
        float distance = Vector3.Distance(transform.position, TestGameManager.Inst.testPlayer.transform.position);

        if (!fadeLoopStarted && distance <= stealth.enemyData.StealthRange)
        {
            fadeLoopStarted = true;
            StartCoroutine(FadeLoop());
        }
    }
    IEnumerator FadeLoop()
    {
        while (true)
        {
            //  숨김 상태 (0~5초)
            yield return StartCoroutine(FadeOut());
            yield return new WaitForSeconds(5f - fadeDuration);

            // 나타남 상태 (5~10초)
            yield return StartCoroutine(FadeIn());
            yield return new WaitForSeconds(5f - fadeDuration);
        }
    }

    IEnumerator FadeOut()
    {
        float timer = 0f;
        EnableColliders(false);

        while (timer < fadeDuration)
        {
            float t = timer / fadeDuration;
            SetAlpha(1f - t);  
            timer += Time.deltaTime;
            yield return null;
        }

        SetAlpha(0f);
    }

    IEnumerator FadeIn()
    {
        float timer = 0f;

        while (timer < fadeDuration)
        {
            float t = timer / fadeDuration;
            SetAlpha(t);  
            timer += Time.deltaTime;
            yield return null;
        }

        SetAlpha(1f);
        EnableColliders(true);
    }

    void SetAlpha(float a)
    {
        foreach (var r in renderers)
        {
            if (r.material.HasProperty("_Color"))
            {
                Color c = r.material.color;
                c.a = a;
                r.material.color = c;
            }
        }
    }

    void EnableColliders(bool value)
    {
        foreach (var c in colliders)
            c.enabled = value;
    }
}
