using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Notification : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI noticeText;
    [SerializeField] private GameObject panel;
    private float _t = 0;
    private float _alpha;
    private CanvasGroup canvasGroup;

    private void Start()
    {
        canvasGroup = panel.GetComponent<CanvasGroup>();
        _alpha = canvasGroup.alpha;
    }

    public void SetNotice(string txt)
    {
        noticeText.text = txt;
        _t = 0;
    }

    private void Update()
    {
        // Fade out over a second (or two)
        _t += 0.5f * Time.deltaTime;
        _alpha = 1-_t;
        if (_alpha < 0.99)
        {
            canvasGroup.alpha = _alpha;
        }
    }
}
