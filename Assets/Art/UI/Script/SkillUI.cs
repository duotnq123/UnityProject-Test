using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillUI : MonoBehaviour
{
    public Image iconImage;
    public Image overlayImage;
    public TextMeshProUGUI cooldownText;

    [Range(1, 5)] public int skillSlot = 1;

    private SkillBase skill;
    private PlayerMana playerMana;

    void Start()
    {
        var player = GameObject.FindGameObjectWithTag("Player"); // Hoặc cách khác tùy bạn
        if (player != null)
        {
            var skillManager = player.GetComponent<PlayerSkillManager>();
            playerMana = player.GetComponent<PlayerMana>();

            if (skillManager != null)
            {
                switch (skillSlot)
                {
                    case 1: skill = skillManager.skill1; break;
                    case 2: skill = skillManager.skill2; break;
                    case 3: skill = skillManager.skill3; break;
                    case 4: skill = skillManager.skill4; break;
                    case 5: skill = skillManager.skill5; break;
                }
            }
        }
    }

    void Update()
    {
        if (skill == null || playerMana == null) return;

        if (skill.IsOnCooldown)
        {
            float cdLeft = skill.GetCooldownLeft();
            overlayImage.fillAmount = cdLeft / skill.cooldown;
            cooldownText.text = Mathf.CeilToInt(cdLeft).ToString();
            overlayImage.color = new Color(0, 0, 0, 0.6f); // Cooldown màu đen mờ
        }
        else if (playerMana.currentMana < skill.manaCost)
        {
            overlayImage.fillAmount = 1f;
            cooldownText.text = "";
            overlayImage.color = new Color(0.2f, 0.2f, 1f, 0.5f); // Thiếu mana: xanh mờ
        }
        else
        {
            overlayImage.fillAmount = 0f;
            cooldownText.text = "";
        }
    }
}
