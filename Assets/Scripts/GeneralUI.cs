using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralUI : MonoBehaviour
{
    [SerializeField] private GameObject scrollablePanel;
    public void ToggleUIVisible()
    {
        CanvasGroup cg = scrollablePanel.GetComponentInChildren<CanvasGroup>();
        cg.interactable= !cg.interactable;
        cg.alpha = cg.alpha == 0 ? 1 : 0;
    }
}
