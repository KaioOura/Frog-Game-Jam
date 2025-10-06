using UnityEngine;
using UnityEngine.UI;

public class DebugTools : MonoBehaviour
{
    [SerializeField] private GameObject debugTools;
    
    [Header("God Mode")]
    [SerializeField] private Image godModeImage;
    [SerializeField] private Health health;
    
    private bool isOpened;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isOpened = false;
        debugTools.gameObject.SetActive(isOpened);

        godModeImage.color = health.IsGodMode ? Color.green : Color.red;
    }

    public void SwitchDebugTools()
    {
        isOpened = !isOpened;
        
        debugTools.gameObject.SetActive(isOpened);
    }
    
    public void SwitchGodMode()
    {
        health.IsGodMode = !health.IsGodMode;
        godModeImage.color = health.IsGodMode ? Color.green : Color.red;
    }

    public void KillChar()
    {
        health.KillChar();
    }
}
