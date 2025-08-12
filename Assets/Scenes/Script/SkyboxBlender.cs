using UnityEngine;

public class SkyboxBlender : MonoBehaviour
{
    public Material skyboxMat;       // Material dùng shader blend
    public Cubemap[] skyboxes;       // Danh sách skybox theo thứ tự thời gian
    public float blendDuration = 5f; // Thời gian blend (giây)
    public float holdTime = 10f;     // Thời gian giữ mỗi skybox trước khi blend sang cái mới

    private int currentIndex = 0;
    private int nextIndex = 1;
    private float blendValue = 0f;
    private bool isBlending = false;
    private float timer = 0f;

    void Start()
    {
        if (skyboxes.Length < 2)
        {
            Debug.LogError("Cần ít nhất 2 skybox để blend!");
            enabled = false;
            return;
        }

        // Gán skybox đầu tiên và tiếp theo
        skyboxMat.SetTexture("_Tex1", skyboxes[currentIndex]);
        skyboxMat.SetTexture("_Tex2", skyboxes[nextIndex]);
        skyboxMat.SetFloat("_Blend", 0f);
        RenderSettings.skybox = skyboxMat;
    }

    void Update()
    {
        if (!isBlending)
        {
            // Đếm thời gian chờ trước khi blend
            timer += Time.deltaTime;
            if (timer >= holdTime)
            {
                isBlending = true;
                timer = 0f;
            }
        }
        else
        {
            // Blend từ 0 -> 1
            blendValue += Time.deltaTime / blendDuration;
            skyboxMat.SetFloat("_Blend", blendValue);

            if (blendValue >= 1f)
            {
                // Hoàn tất blend
                currentIndex = nextIndex;
                nextIndex = (nextIndex + 1) % skyboxes.Length;

                skyboxMat.SetTexture("_Tex1", skyboxes[currentIndex]);
                skyboxMat.SetTexture("_Tex2", skyboxes[nextIndex]);
                skyboxMat.SetFloat("_Blend", 0f);

                blendValue = 0f;
                isBlending = false;
            }
        }
    }
}
