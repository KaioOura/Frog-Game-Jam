using UnityEngine;

public class InitialSettingSetter : MonoBehaviour
{
    [SerializeField] private QualitySetter qualitySetter;
    private int _qualitySaved;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (PlayerPrefs.HasKey("SettingQuality"))
        {
            _qualitySaved = PlayerPrefs.GetInt("Quality", qualitySetter.QualityLevelSo.QualityLevels.Length - 1);
            qualitySetter.ChangeQuality(_qualitySaved);
        }
        else
        {
            qualitySetter.SetAutoSetting();
        }
    }
    
}
