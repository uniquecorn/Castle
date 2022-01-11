using System.Collections;
using System.Collections.Generic;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using UnityEngine;

namespace Castle.Core.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class CastlePopup : MonoBehaviour
    {
        [SerializeField,HideInInspector]
        private new RectTransform transform;
        public RectTransform Transform => transform ? transform : transform = (RectTransform)base.transform;
        public enum VisibleState
        {
            NotVisible,
            TransitionIn,
            TransitionOut,
            Visible
        }
        public enum BackButtonAction
        {
            NoEffect,
            Close,
            Stop
        }
        public enum SlideTransition
        {
            None,
            SlideLeft,
            SlideRight,
            SlideUp,
            SlideDown
        }
        public VisibleState visibleState;
        public bool Visible => visibleState == VisibleState.Visible;
        public bool AbsVisible => visibleState == VisibleState.Visible || visibleState == VisibleState.TransitionIn;
        public bool NotVisible => visibleState == VisibleState.NotVisible;
        public bool InTransition => visibleState == VisibleState.TransitionIn || visibleState == VisibleState.TransitionOut;
        [HideInInspector]
        public float visibleTimer;
        protected virtual float TransitionTime => unscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
        protected float openTimer;
        [HideInInspector]
        public CanvasGroup canvasGroup;
        #if ODIN_INSPECTOR
        [ShowIf("UseVisibleTimer")]
#endif
        public float transitionInSpeed,transitionOutSpeed = 1;
        #if ODIN_INSPECTOR
        [ShowIf("UseVisibleTimer")]
#endif
        public bool unscaledTime,immediateOpenActions,immediateCloseActions;
        public virtual BackButtonAction BackButtonAffected => BackButtonAction.Close;
        protected virtual bool UseVisibleTimer() => CanvasGroupExists(out _);
        public bool CanvasGroupExists(out CanvasGroup c)
        {
            if (canvasGroup == null) return TryGetComponent(out c);
            c = canvasGroup;
            return true;
        }
    
        protected virtual void Reset()
        {
            CanvasGroupExists(out canvasGroup);
        }
    }
}
