using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class ApplySquaredSize : MonoBehaviour
{
    public RectTransform squaredRect;
    public float space;
    public bool left;
    public bool bottom;
    public bool right;
    public bool top;

    private void Update()
    {
        if(squaredRect)
        {
            if (left || bottom)
            {
                (transform as RectTransform).anchorMin = Vector2.right * (left ? 0 : (transform as RectTransform).anchorMin.x) +
                    Vector2.up * (bottom ? 0 : (transform as RectTransform).anchorMin.y);
                (transform as RectTransform).offsetMin =
                    Vector2.right * (left ? (squaredRect.sizeDelta.x + squaredRect.anchoredPosition.x + space) : (transform as RectTransform).offsetMin.x) +
                        Vector2.up * (bottom ? (squaredRect.sizeDelta.y + squaredRect.anchoredPosition.y + space) : (transform as RectTransform).offsetMin.y);
            }
            if(right ||top)
            {
                (transform as RectTransform).offsetMax =
                    Vector2.right * (right ? -(squaredRect.sizeDelta.x + squaredRect.anchoredPosition.x + space) : (transform as RectTransform).offsetMax.x) +
                        Vector2.up * (top ? -(squaredRect.sizeDelta.y + squaredRect.anchoredPosition.y + space) : (transform as RectTransform).offsetMax.y);
            }
        }

    }
}
