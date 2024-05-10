
using Characters;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(-1000)]
public class HudManager : MonoBehaviour
{
    public static HudManager Instance { get; private set; }
    
    [SerializeField] private Button abilityButton;
    [SerializeField] private Image abilityFillBar;
    [SerializeField] private Image abilityIcon;
    [SerializeField] private Joystick stick;

    private Chicken _owner;

    private void Awake()
    {
        if(Instance && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        #if !UNITY_STANDALONE || UNITY_EDITOR
        stick.gameObject.SetActive(true);
        #endif
        
    }

    public void BindPlayer(Chicken player)
    {
        _owner = player;
        SetAbilityIcon();
        SetAbilityPercent(1);
        
        _owner.Ability.BindAbilityUpdated(SetAbilityPercent);

    }

    public void SetAbilityIcon()
    {
        abilityIcon.sprite = _owner.Ability.Icon;
    }

    public void SetAbilityPercent(float percent)
    {
        abilityFillBar.fillAmount = percent;
        abilityButton.interactable = percent >= 1;
    }
    
    //Bind actions the same way our controller binds actions
    public void MimicActionAbility()
    {
        _owner.TryAbility();
    }
#if !UNITY_STANDALONE || UNITY_EDITOR
    [SerializeField] private float lookSpeed = 20;
#if UNITY_EDITOR
    [SerializeField] private bool stickEnabled;
#endif
    private void Update()
    {
        #if UNITY_EDITOR
        if (!stickEnabled) return;
        #endif
        
        if(stick.Direction != Vector2.zero) _owner.Move(Vector2.up);
        else _owner.Move(Vector2.zero);
        _owner.Look(stick.Direction * lookSpeed);
    }
#endif
}
