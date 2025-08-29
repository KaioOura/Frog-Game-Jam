using System;
using UnityEngine;
using UnityEngine.Serialization;


[Serializable]
public class TutorialStep
{
    public TutorialAction TutorialAction => tutorialAction;
    
   [SerializeField] private TutorialAction tutorialAction;
   
}
