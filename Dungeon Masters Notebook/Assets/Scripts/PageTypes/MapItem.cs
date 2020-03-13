using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MapItem : MonoBehaviour
{
    public Page page;

    public UnityEvent onEndMove;

    public RectTransform rectTransform
    {
        get
        {
            return transform as RectTransform;
        }
    }
}
