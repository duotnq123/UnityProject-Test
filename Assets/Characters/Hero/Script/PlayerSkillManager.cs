using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSkillManager : MonoBehaviour
{
    public Skill1 skill1;
    public Skill2 skill2;
    public Skill3 skill3;
    public Skill4 skill4;
    public Skill5 skill5;
      public bool isSkillPlaying = false;

    public void On1(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (skill1 != null)
                skill1.TryUseSkill();
            else
                Debug.LogWarning("Skill 1 chưa được gán!");
        }
    }

    public void On2(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (skill2 != null)
                skill2.TryUseSkill();
            else
                Debug.LogWarning("Skill 2 chưa được gán!");
        }
    }

    public void On3(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (skill3 != null)
                skill3.TryUseSkill();
            else
                Debug.LogWarning("Skill 3 chưa được gán!");
        }
    }

    public void On4(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (skill4 != null)
                skill4.TryUseSkill();
            else
                Debug.LogWarning("Skill 4 chưa được gán!");
        }
    }
    public void On5(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (skill5 != null)
                skill5.TryUseSkill();
            else
                Debug.LogWarning("Skill 5 chưa được gán!");
        }
    }
}
