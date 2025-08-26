using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class VerticalUIList : MonoBehaviour
{
    [Header("Configuração de Layout")] public float verticalSpacing = 100f; // Espaço entre os elementos
    public float offsetX = 0f; // Offset horizontal
    public float offsetY = 0f; // Offset vertical (ponto inicial)

    [Header("Tween Config")] public float tweenDuration = 0.25f; // Duração da animação
    public Ease tweenEase = Ease.OutQuad; // Tipo de easing

    private List<RectTransform> uiElements = new List<RectTransform>();

    public void AddUI(RectTransform newElement)
    {
        uiElements.Add(newElement);
        RepositionUI();
    }

    public void RemoveUI(RectTransform element)
    {
        if (uiElements.Contains(element))
        {
            uiElements.Remove(element);
            RepositionUI();
        }
    }

    private void RepositionUI()
    {
        for (int i = 0; i < uiElements.Count; i++)
        {
            Vector2 targetPos = new Vector2(offsetX, offsetY - (i * verticalSpacing));

            uiElements[i].DOAnchorPos(targetPos, tweenDuration).SetEase(tweenEase);
        }
    }

    private void OnValidate()
    {
        RepositionUI();
    }
}