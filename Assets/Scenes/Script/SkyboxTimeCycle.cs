using UnityEngine;

public class SkyboxTimeCycle : MonoBehaviour
{
    [Header("Skybox Materials")]
    public Material morningSkybox;    // 6h - 12h
    public Material afternoonSkybox;  // 12h - 18h
    public Material eveningSkybox;    // 18h - 21h
    public Material nightSkybox;      // 21h - 6h

    [Header("Time Settings")]
    [Range(0f, 24f)] public float timeOfDay = 6f;
    public float daySpeed = 0.1f; // tốc độ thời gian (1 = 24h trong 240s)

    private void Update()
    {
        // Tăng thời gian
        timeOfDay += Time.deltaTime * daySpeed;
        if (timeOfDay >= 24f) timeOfDay = 0f;

        // Đổi skybox mượt
        if (timeOfDay >= 6f && timeOfDay < 12f)
        {
            float t = Mathf.InverseLerp(6f, 12f, timeOfDay);
            RenderSettings.skybox.Lerp(morningSkybox, afternoonSkybox, t);
        }
        else if (timeOfDay >= 12f && timeOfDay < 18f)
        {
            float t = Mathf.InverseLerp(12f, 18f, timeOfDay);
            RenderSettings.skybox.Lerp(afternoonSkybox, eveningSkybox, t);
        }
        else if (timeOfDay >= 18f && timeOfDay < 21f)
        {
            float t = Mathf.InverseLerp(18f, 21f, timeOfDay);
            RenderSettings.skybox.Lerp(eveningSkybox, nightSkybox, t);
        }
        else // 21h - 6h sáng hôm sau
        {
            float t;
            if (timeOfDay < 6f) // sau nửa đêm
                t = Mathf.InverseLerp(0f, 6f, timeOfDay);
            else // từ 21h đến 24h
                t = Mathf.InverseLerp(21f, 24f, timeOfDay);

            RenderSettings.skybox.Lerp(nightSkybox, morningSkybox, t);
        }
    }
}
