using UnityEngine;
using UnityEngine.UI;

public class ManaBarUI : MonoBehaviour
{
    public Slider manaSlider;         
    public PlayerMana playerMana;     

    void Start()
    {
        if (playerMana != null)
        {
            playerMana.onManaChanged.AddListener(UpdateManaBar);
            UpdateManaBar(); 
        }
    }

    void UpdateManaBar()
    {
        if (manaSlider != null && playerMana != null)
        {
            manaSlider.value = playerMana.GetManaPercent(); 
        }
    }
}
