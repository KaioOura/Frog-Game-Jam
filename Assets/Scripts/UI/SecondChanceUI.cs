using System;
using UnityEngine;
using UnityEngine.UI;

public class SecondChanceUI : MonoBehaviour
{
    [SerializeField] private Button adButton;

    private void OnEnable()
    {
        adButton.interactable = AdManager.IsAdAvailable();
    }
}
