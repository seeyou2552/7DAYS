using UnityEngine;

public class CameraLetterbox : MonoBehaviour
{
    public float targetAspectRatio = 16f / 9f;

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        UpdateCameraRect();
    }

    void Update()
    {
        // 해상도 변경 시 대응
        if (Screen.width != cam.pixelWidth || Screen.height != cam.pixelHeight)
        {
            UpdateCameraRect();
        }
    }

    void UpdateCameraRect()
    {
        float screenAspect = (float)Screen.width / Screen.height;
        float scaleHeight = screenAspect / targetAspectRatio;

        if (scaleHeight < 1.0f)
        {
            // 레터박스 (상하 검정)
            Rect rect = new Rect(0, (1f - scaleHeight) / 2f, 1f, scaleHeight);
            cam.rect = rect;
        }
        else
        {
            // 필러박스 (좌우 검정)
            float scaleWidth = 1f / scaleHeight;
            Rect rect = new Rect((1f - scaleWidth) / 2f, 0, scaleWidth, 1f);
            cam.rect = rect;
        }
    }
}
