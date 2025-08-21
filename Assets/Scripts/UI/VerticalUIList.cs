using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class VerticalUIList : MonoBehaviour
{
    [Header("Configuração de Layout")]
    public float verticalSpacing = 100f;   // Espaço entre os elementos
    public float offsetX = 0f;             // Offset horizontal
    public float offsetY = 0f;             // Offset vertical (ponto inicial)

    [Header("Tween Config")]
    public float tweenDuration = 0.25f;    // Duração da animação
    public Ease tweenEase = Ease.OutQuad;  // Tipo de easing

    private List<RectTransform> uiElements = new List<RectTransform>();

    /// <summary>
    /// Adiciona um novo elemento ao final da lista.
    /// </summary>
    public void AddUI(RectTransform newElement)
    {
        uiElements.Add(newElement);
        RepositionUI();
    }

    /// <summary>
    /// Remove um elemento e reorganiza.
    /// </summary>
    public void RemoveUI(RectTransform element)
    {
        if (uiElements.Contains(element))
        {
            uiElements.Remove(element);
            RepositionUI();
        }
    }

    /// <summary>
    /// Reorganiza todos os elementos com DOTween.
    /// </summary>
    private void RepositionUI()
    {
        for (int i = 0; i < uiElements.Count; i++)
        {
            Vector2 targetPos = new Vector2(
                offsetX,                             // deslocamento no X
                offsetY - (i * verticalSpacing)      // deslocamento no Y
            );

            uiElements[i].DOAnchorPos(targetPos, tweenDuration).SetEase(tweenEase);
        }
    }

    private void OnValidate()
    {
        RepositionUI();
    }
}