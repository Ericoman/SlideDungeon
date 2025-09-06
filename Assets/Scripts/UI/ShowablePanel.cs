using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class ShowablePanel : MonoBehaviour
    {
        [SerializeField]
        private bool tryUseCanvasGroup = true;
        [SerializeField]
        private bool startHidden = true; 
        [SerializeField]
        private float alphaShow = 1.0f;
        [SerializeField]
        private float alphaHide = 0.0f;
        
        private bool bIsShown = false;
        public bool IsShown => bIsShown;
        
        public UnityEvent OnShow;
        public UnityEvent OnHide;
        
        private CanvasGroup _canvasGroup;

        private void Awake()
        {
            if (tryUseCanvasGroup)
            {
                _canvasGroup = GetComponent<CanvasGroup>();
            }
        }

        private void Start()
        {
            if (startHidden)
            {
                Show(false);
            }
        }

        public void Show(bool show)
        {
            if (_canvasGroup != null)
            {
                _canvasGroup.interactable = show;
                _canvasGroup.blocksRaycasts = show;
                _canvasGroup.alpha = show ? alphaShow : alphaHide;
            }
            else
            {
                gameObject.SetActive(show);
            }
            
            bIsShown = show;

            if (bIsShown)
            {
                OnShow?.Invoke();
            }
            else
            {
                OnHide?.Invoke();
            }
        }
    }
}
