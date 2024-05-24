using Ability;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class AbilityUIBind : MonoBehaviour
{
    
    [SerializeField] private CustomButton abilityButton;
    [SerializeField] private Image abilityFillBar;
    [SerializeField] private Image abilityIcon;
    
    private AbilityBase _targetAbility;
    
    
    public void SetTargetAbility(AbilityBase ability)
    {
        //Bind the ability
        _targetAbility = ability;
        
        //Bind Image,
        abilityIcon.sprite = ability.Icon;
        //Bind OnClick and CUSTOM OnRelease
        abilityButton.onClick.RemoveAllListeners();
        abilityButton.onReleased.RemoveAllListeners();
        abilityButton.onClick.AddListener(() => _targetAbility.StartAbility());
        abilityButton.onReleased.AddListener(() => _targetAbility.StopAbility());
    }
    
    //Every single frame, we need to be updating and checking for ability status changes.
    //NOTE: This can be done better by batching abilities in a manager and updating everything simultaneously instead of individually.
    private void LateUpdate()
    {
        abilityFillBar.fillAmount = _targetAbility.GetReadyPercent();
        abilityButton.interactable = _targetAbility.CanActivate();
    }
}
