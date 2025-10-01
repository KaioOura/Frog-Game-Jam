using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "QualityLevelSO", menuName = "Scriptable Objects/QualityLevelSO")]
public class QualityLevelSO : ScriptableObject
{
    public RenderPipelineAsset[] QualityLevels => qualityLevels;
    [SerializeField] private RenderPipelineAsset[] qualityLevels;
}
