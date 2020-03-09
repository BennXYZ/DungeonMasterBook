using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class CampaignLoadingItem : MonoBehaviour
{
    public UnityEvent onPress;

    public UnityEvent onRemove;

    public TMP_Text title;

    public void OnRemove()
    {
        onRemove.Invoke();
    }

    public void Init(string title)
    {
        this.title.text = title;
    }

    public void OnPresses()
    {
        onPress.Invoke();
    }
}
