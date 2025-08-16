using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SplatterEffect : MonoBehaviour
{
    [SerializeField] private Image splatterImage;
    [SerializeField] private float totalDuration = 10f;
    [SerializeField] private float flickerInterval = 1f; 
    [SerializeField] private Color baseColor = Color.white;


 
    public void ShowSplatter()
    {
        splatterImage.color = new Color(baseColor.r, baseColor.g, baseColor.b, 0f);
        StartCoroutine(FlickerAlpha());
    }

    private IEnumerator FlickerAlpha()
    {
        float elapsed = 0f;
        bool visible = false;

        while (elapsed < totalDuration)
        {
            visible = !visible;
            float targetAlpha = visible ? 1f : 0.5f;

         
            for (float t = 0f; t < flickerInterval; t += Time.deltaTime)
            {
                float lerpedAlpha = Mathf.Lerp(splatterImage.color.a, targetAlpha, t / flickerInterval);
                splatterImage.color = new Color(baseColor.r, baseColor.g, baseColor.b, lerpedAlpha);
                yield return null;
            }

            splatterImage.color = new Color(baseColor.r, baseColor.g, baseColor.b, targetAlpha);
            elapsed += flickerInterval;
        }
        splatterImage.color = new Color(baseColor.r, baseColor.g, baseColor.b, 0f);

    }
}
