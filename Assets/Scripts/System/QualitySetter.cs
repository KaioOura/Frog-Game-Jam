using System;
using UnityEngine;
using UnityEngine.Rendering;

public class QualitySetter : MonoBehaviour
{
    public QualityLevelSO QualityLevelSo => qualityLevelSo;
    public int CurrentQuality => _currentQuality;

    [SerializeField] private QualityLevelSO qualityLevelSo;

    private int _currentQuality;

    private void Awake()
    {
        // Fonte de verdade do nível atual, válida mesmo antes do menu abrir.
        _currentQuality = PlayerPrefs.GetInt("SettingQuality", qualityLevelSo.QualityLevels.Length - 1);
    }

    public void SetAutoSetting()
    {
        ChangeQuality(FindSetting());
    }
    
    private int FindSetting()
    {
        int qualityScore = SettingFinder.GetSettingScore();
        int quality = qualityLevelSo.QualityLevels.Length - 1;
            
        switch (qualityScore)
        {
            case >= 8:
                QualitySettings.SetQualityLevel(4); // High
                break;
            case >= 5:
                quality = 1;
                break;
            default:
                quality = 0;
                break;
        }
        
        return quality;
    }
    
    public void ChangeQuality(int value)
    {
        _currentQuality = value;

        QualitySettings.SetQualityLevel(value);
        QualitySettings.renderPipeline = qualityLevelSo.QualityLevels[value];

        
        if (value >= qualityLevelSo.QualityLevels.Length - 1)
        {
            Application.targetFrameRate = 60;
            Debug.Log("Config: High");
        }
        else if (value >= 1)
        {
            Application.targetFrameRate = 60;
            Debug.Log("Config: Medium");
        }
        else
        {
           Application.targetFrameRate = 30;
            Debug.Log("Config: Low"); 
        }
        
        PlayerPrefs.SetInt("SettingQuality", value);
    } 
    
}
