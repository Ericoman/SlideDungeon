using System;
using UnityEngine;

public class UI_Showable : MonoBehaviour
{
    private CanvasGroup _canvasGroup;
    bool shown = false;
    public bool IsShown => shown;
    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Show(bool show)
    {
        if (_canvasGroup != null)
        {
            _canvasGroup.alpha = show ? 1 : 0;
            _canvasGroup.blocksRaycasts = show;
            _canvasGroup.interactable = show;
        }
        else
        {
            gameObject.SetActive(show);
        }
        shown = show;
    }
}
