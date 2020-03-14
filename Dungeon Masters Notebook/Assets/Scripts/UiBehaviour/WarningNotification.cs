using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WarningNotification : MonoBehaviour
{
    public TMP_Text notificationTextfield;

    public Button yesButton;

    public GameObject blockerObject;

    private static WarningNotification instance;

    public void SetInstance()
    {
        instance = this;
    }

    public static void OpenNotificationWindow(string text, Action onYes)
    {
        instance.notificationTextfield.text = text;
        instance.yesButton.onClick.RemoveAllListeners();
        instance.yesButton.onClick.AddListener(delegate { onYes(); });
        instance.yesButton.onClick.AddListener(delegate { instance.blockerObject.SetActive(false); });
        instance.yesButton.onClick.AddListener(delegate { instance.gameObject.SetActive(false); });

        instance.blockerObject.SetActive(true);
        instance.gameObject.SetActive(true);
    }
}
