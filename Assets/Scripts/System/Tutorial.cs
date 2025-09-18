using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{

    public List<TutorialStep> TutorialSteps => tutorialSteps;
    
    [SerializeField] private List<TutorialStep> tutorialSteps = new List<TutorialStep>();

    public int GetStepsCount()
    {
        return tutorialSteps.Count;
    }
}
