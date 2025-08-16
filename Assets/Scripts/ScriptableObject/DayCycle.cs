using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DayCycle : MonoBehaviour
{
    [Range(0.0f, 1.0f)]
    public float time;
    public float dayForSecond;
    public float startTime = 0.4f;
    public float timeRate;
    public Vector3 noon; // Vector 90 0 0

    [Header("Sun")]
    public Light sun;
    public Gradient sunColor;
    public AnimationCurve sunIntensity;

    [Header("Moon")]
    public Light moon;
    public Gradient moonColor;
    public AnimationCurve moonIntensity;

    [Header("Sky")]
    public Material skyboxMaterial;

    [Header("Other Light")]
    public AnimationCurve lightingIntensityMultiplier;
    public AnimationCurve reflectionIntensityMultiplier;
    bool isNightFlag = false;


    void Start()
    {
        AudioManager.Instance.PlayBGM(0);
        timeRate = 1.0f / dayForSecond;
        time = startTime;
    }

    void Update()
    {
        time = (time + timeRate * Time.deltaTime) % 1.0f;

        UpdateLighting(sun, sunColor, sunIntensity);
        UpdateLighting(moon, moonColor, moonIntensity);

        if (IsNight() && !isNightFlag)
        {
            AudioManager.Instance.PlayBGM(1);
            Debug.Log("밤시작");
            isNightFlag = true;
            TestGameManager.Inst.StartWave();
        }
        else if (!IsNight() && isNightFlag)
        {
            AudioManager.Instance.PlayBGM(0);
            Debug.Log("낮시작");
            isNightFlag = false;

        }
        RenderSettings.ambientIntensity = lightingIntensityMultiplier.Evaluate(time);
        RenderSettings.reflectionIntensity = reflectionIntensityMultiplier.Evaluate(time);
    }

    void UpdateLighting(Light lightSource, Gradient gradient, AnimationCurve intensityCurve)
    {
        float intensity = intensityCurve.Evaluate(time);

        lightSource.transform.eulerAngles = (time - (lightSource == sun ? 0.25f : 0.75f)) * noon * 4f;
        lightSource.color = gradient.Evaluate(time);
        lightSource.intensity = intensity;

        float exposure = 1.0f / dayForSecond;
        if (time >= 0.5f)
        {
            float t = (time - 0.5f) * 2f;
            exposure = Mathf.Lerp(0.3f, 0.1f, t);
        }
        else
        {
            float t = time * 2f;
            exposure = Mathf.Lerp(0.1f, 0.3f, t);
        }

        RenderSettings.skybox.SetFloat("_Exposure", exposure);

        GameObject go = lightSource.gameObject;
        if (lightSource.intensity == 0 && go.activeInHierarchy)
        {
            go.SetActive(false);
        }
        else if (lightSource.intensity > 0 && !go.activeInHierarchy)
        {
            go.SetActive(true);
        }
    }

    bool IsNight()
    {
        return time >= 0.75f || time < 0.25f;
    }
}
