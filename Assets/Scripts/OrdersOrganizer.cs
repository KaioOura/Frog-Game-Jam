using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[ExecuteInEditMode]
public class OrdersOrganizer : MonoBehaviour
{
    public enum Alignment
    {
        Horizontal,
        Vertical
    }

    [Header("Layout Settings")]
    [SerializeField] private Alignment alignment = Alignment.Horizontal;

    [SerializeField] private float spacing = 10f;

    [SerializeField, Range(1, 50)] private int breakPoint = 5;

    [SerializeField] private float animationSpeed = 10f;

    [Header("Start Position")]
    [SerializeField] private Vector2 firstRowStart = new Vector2(0, 0);
    [SerializeField] private Vector2 secondRowStart = new Vector2(0, -50f);
    [SerializeField] private List<Transform> orderDesiredPositions = new List<Transform>();
    [SerializeField] private List<Transform> orderPositions = new List<Transform>();

    private int _activeOrders = -1;

    public void AssignOrder(Transform orderTransform)
    {
        int orderPositionIndex = _activeOrders + 1 > orderPositions.Count ? 0 : _activeOrders + 1;
        
        orderTransform.DOMove(orderPositions[orderPositionIndex].position, animationSpeed);
    }

    public void UpdateOrderPositioning()
    {
        
    }
    
    private void Update()
    {
        int max = transform.childCount;
        if (max <= 0) return;

        // pega o tamanho do child no eixo do alinhamento
        float childSize = (alignment == Alignment.Horizontal)
            ? transform.GetChild(0).localScale.x
            : transform.GetChild(0).localScale.y;

        int i = 0, j = 0;
        foreach (Transform child in transform)
        {
            child.localEulerAngles = Vector3.zero;

            if (i < breakPoint)
            {
                child.localPosition = Vector2.Lerp(
                    child.localPosition,
                    GetAlignedPosition(i, firstRowStart, childSize),
                    animationSpeed * Time.deltaTime
                );
                i++;
            }
            else
            {
                child.localPosition = Vector2.Lerp(
                    child.localPosition,
                    GetAlignedPosition(j, secondRowStart, childSize),
                    animationSpeed * Time.deltaTime
                );
                j++;
            }
        }
    }

    private Vector2 GetAlignedPosition(int index, Vector2 start, float childSize)
    {
        float offset = index * (childSize + spacing);

        if (alignment == Alignment.Horizontal)
            return new Vector2(start.x + offset, start.y);
        else
            return new Vector2(start.x, start.y - offset); // -offset pra "descer"
    }
}
