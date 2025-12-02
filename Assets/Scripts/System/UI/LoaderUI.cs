using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoaderUI : MonoBehaviour
{
    [SerializeField] private List<LoaderUIStep> fakeSteps;
    [SerializeField] private TextMeshProUGUI loaderText;

    private List<LoaderUIStep> _steps = new List<LoaderUIStep>();
    
    public void AddStep(string name, string desc, float time, Func<bool> condition = null)
    {
        _steps.Add(new LoaderUIStep {
            StepName = name,
            StepDescription = desc,
            StepTime = time,
            Condition = condition
        });
    }
    
    public void Initialize(Action callBack)
    {
        foreach (var loaderUIStep in fakeSteps)
        {
            AddStep(loaderUIStep.StepName, loaderUIStep.StepDescription, loaderUIStep.StepTime);
        }
        
        StartCoroutine(WaitAndLoadNextStep(callBack));
    }

    private IEnumerator WaitAndLoadNextStep(Action callBack)
    {
        foreach (var loaderUIStep in _steps)
        {
            loaderText.text = loaderUIStep.StepDescription;
            
            yield return new WaitForSeconds(loaderUIStep.StepTime);
            
            if (loaderUIStep.Condition != null)
            {
                yield return new WaitUntil(loaderUIStep.Condition);
            }
        }
        
        callBack?.Invoke();
    }
    
}

[Serializable]
public class LoaderUIStep
{
    public string StepName;
    public string StepDescription;

    public float StepTime;

    public Func<bool> Condition;
    

}
