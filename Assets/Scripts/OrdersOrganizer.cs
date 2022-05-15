using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class OrdersOrganizer : MonoBehaviour
{
    [SerializeField]
    float spacing = 0;

    [SerializeField]
    [Range(0, 50)]
    int breakPoint = 5;

    [SerializeField]
    float animationSpeed = 10;


    [ExecuteInEditMode]
    private void FixedUpdate()
    {
        int max = transform.childCount;

        if (max > 0)
        {
            float childSize = transform.GetChild(0).localScale.x;

            float totalSizeUp, middleUp;
            float totalSizeDown, middleDown;

            if (max > breakPoint * 2)
            {
                var t = (max - 1) * (childSize + spacing) - spacing;
                totalSizeUp = t / 2;
                totalSizeDown = t / 2;

                breakPoint = (int)Mathf.Ceil(max / 2f);
            }
            else if (max > breakPoint)
            {
                totalSizeUp = (breakPoint - 1) * (childSize + spacing) - spacing;
                totalSizeDown = (max - breakPoint - 1) * (childSize + spacing) - spacing;
            }
            else
            {
                totalSizeUp = (max - 1) * (childSize + spacing) - spacing;
                totalSizeDown = 0;
            }

            middleUp = totalSizeUp / 2;
            middleDown = totalSizeDown / 2;

            int i = 0, j = 0;
            foreach (Transform child in transform)
            {
                child.localEulerAngles = new Vector3(0, 0, 0);

                if (i < breakPoint)
                {
                    child.localPosition = Vector2.Lerp(child.localPosition, new Vector2(i * (childSize + spacing), -57), animationSpeed * Time.deltaTime);
                    i++;
                }
                else
                {
                    child.localPosition = Vector2.Lerp(
                        child.localPosition,
                        new Vector2(j * (childSize + spacing) - middleDown, -1.5f + 0),
                        animationSpeed * Time.deltaTime
                        );
                    j++;
                }
            }
        }
    }
}
