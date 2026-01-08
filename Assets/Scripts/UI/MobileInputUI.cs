using System.Collections.Generic;
using UnityEngine;

public class MobileInputUI : MonoBehaviour
{
    public Joystick Joystick => joystick;
    
    [SerializeField] private bool useMobileInput = true;
    [SerializeField] private bool useStandardMobileInput;

    [SerializeField] private Joystick joystick;
    [SerializeField] private GameObject actionButton;
    [SerializeField] private GameObject deliveryButton;
    [SerializeField] private GameObject tapSwipeButton;
    [SerializeField] private GameObject mealMathUI;
    [SerializeField] private GameObject settingIcon;
    [SerializeField] public EventChannelAction eventChannelAction;
    
    private List<GameObject> _activeButtons = new List<GameObject>();
    private InputManager _inputManager;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void Initialize(InputManager inputManager)
    {
        _inputManager = inputManager;

        joystick.SetEventChannelAction(eventChannelAction);
        
        ShowStandardMobileInputUIs(useStandardMobileInput);
        ShowUI(false);
    }
    
    public void ShowStandardMobileInputUIs(bool isStandard)
    {
        _activeButtons.Clear();
        
        actionButton.SetActive(isStandard);
        deliveryButton.SetActive(isStandard);
        
        tapSwipeButton.SetActive(!isStandard);
        mealMathUI.SetActive(!isStandard);

        if (isStandard)
        {
            _activeButtons.Add(actionButton);
            _activeButtons.Add(deliveryButton);
            _inputManager.UnSubscribe();
        }
        else
        {
            _activeButtons.Add(tapSwipeButton);
            _activeButtons.Add(mealMathUI);
            _inputManager.Subscribe();
        }
    }

    public void ShowUI(bool shouldShow)
    {
        foreach (var button in _activeButtons)
        {
            button.SetActive(shouldShow);
        }
        
        joystick.gameObject.SetActive(shouldShow);
    }
}
