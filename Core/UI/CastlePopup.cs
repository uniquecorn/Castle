using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace Castle.Core.UI
{
    [RequireComponent(typeof(RectTransform)),ExecuteAlways]
    public class CastlePopup : MonoBehaviour
    {
        #region Enums
        public enum VisibleState
        {
            NotVisible,
            TransitionIn,
            TransitionOut,
            Visible
        }
        public enum BackButtonAction
        {
            Passthrough,
            Close,
            CloseWhenVisible,
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
        public enum RaycastBlockMode
        {
            OnVisible,
            OnAbsVisible,
            OnVisibleAll
        }
        [Flags]
        public enum ImmediateActionTriggers
        {
            None = 0,
            Open = 1,
            Close = 2,
            OpenAndClose = 3
        }
        [Flags]
        public enum SlideCurveMode
        {
            None = 0,
            In = 1,
            Out = 2,
            InAndOut = 3
        }
        #endregion
#if ODIN_INSPECTOR
        [ReadOnly]
#endif
        public Vector2 cachedPosition;
        [SerializeField,HideInInspector]
        private new RectTransform transform;
        public RectTransform Transform => transform ? transform : transform = (RectTransform)base.transform;
#if ODIN_INSPECTOR
        [HideIf("HasHandler")]
        #endif
        public CastlePopupHandler handler;
        public static List<CastlePopup> OpenedPopups;
        public virtual RaycastBlockMode raycastBlockMode => RaycastBlockMode.OnVisible;
#if ODIN_INSPECTOR
        [ReadOnly,SuffixLabel("$StateSuffixLabel", Overlay = true)]
#endif
        public VisibleState visibleState;
#if ODIN_INSPECTOR
        [ShowInInspector,ShowIf("UseVisibleTimer"),PropertyRange(0,1)]
#endif
        public float Animation
        {
            get => visibleTimer;
            set
            {
                switch (value)
                {
                    case <= 0:
                        visibleTimer = 0;
                        OnClose();
                        visibleState = VisibleState.NotVisible;
                        break;
                    case >= 1:
                        visibleTimer = 1;
                        OpenAnim();
                        break;
                    default:
                    {
                        if (value < visibleTimer)
                        {
                            visibleState = VisibleState.TransitionOut;
                            visibleTimer = value;
                            CloseAnim(visibleTimer);
                        }
                        else if (value > visibleTimer)
                        {
                            visibleState = VisibleState.TransitionIn;
                            visibleTimer = value;
                            OpenAnim(visibleTimer);
                        }

                        break;
                    }
                }
            }
        }
        public bool Visible => visibleState == VisibleState.Visible;
        public bool AbsVisible => visibleState is VisibleState.Visible or VisibleState.TransitionIn;
        public bool NotVisible => visibleState == VisibleState.NotVisible;
        public bool InTransition => visibleState is VisibleState.TransitionIn or VisibleState.TransitionOut;
        protected float visibleTimer,openTimer;
        protected virtual float TransitionTime => unscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
        [HideInInspector]
        public CanvasGroup canvasGroup;
        protected virtual bool AnimateCanvasGroup => true;
        #if ODIN_INSPECTOR
        [HorizontalGroup("Transition Speed", VisibleIf = "UseVisibleTimer",Title="Transition Speed"),LabelText("In"),LabelWidth(40),SuffixLabel("$InSuffixLabel",Overlay = true)]
#endif
        public float transitionInSpeed = 1;
#if ODIN_INSPECTOR
        [HorizontalGroup("Transition Speed"),LabelText("Out"),LabelWidth(40),SuffixLabel("$OutSuffixLabel",Overlay = true)]
#endif
        public float transitionOutSpeed = 1;
#if ODIN_INSPECTOR
        [HorizontalGroup("Transition Speed",0.1f),HideLabel,SuffixLabel("Unscaled?")]
#endif
        public bool unscaledTime;
#if ODIN_INSPECTOR
        [ShowIf("UseVisibleTimer")]
#endif
        public SlideTransition slideTransition;
        [BoxGroup("Slide Transition",VisibleIf = "UseSlideTransition",ShowLabel = false)]
        public float transitionDistance = 10;
        [BoxGroup("Slide Transition")]
        public bool transitionFollowThrough;
        [BoxGroup("Slide Transition")]
        public SlideCurveMode useSlideCurve;
        [BoxGroup("Slide Transition"),ShowIf("useSlideCurve",SlideCurveMode.InAndOut)]
        public bool separateCurves;
        [BoxGroup("Slide Transition"),HideIf("useSlideCurve",SlideCurveMode.None)]
        public AnimationCurve slideCurve;
        [BoxGroup("Slide Transition"),ShowIf("UseSeparateCurves")]
        public AnimationCurve slideCurveOut;
        public bool UseSeparateCurves => useSlideCurve == SlideCurveMode.InAndOut && separateCurves;
        protected virtual Vector2 SlideOffset =>
            slideTransition switch
            {
                SlideTransition.SlideUp => Vector2.down * transitionDistance,
                SlideTransition.SlideDown => Vector2.up * transitionDistance,
                SlideTransition.SlideLeft => Vector2.right * transitionDistance,
                SlideTransition.SlideRight => Vector2.left * transitionDistance,
                _ => Vector2.zero
            };
        protected bool ImmediateOpenActions => (immediateActionTriggers & ImmediateActionTriggers.Open) != 0;
        protected bool ImmediateCloseActions => (immediateActionTriggers & ImmediateActionTriggers.Close) != 0;
        [ShowIf("UseVisibleTimer")]
        public ImmediateActionTriggers immediateActionTriggers;
        public string InSuffixLabel => 1 / transitionInSpeed + "sec";
        public string OutSuffixLabel => 1 / transitionOutSpeed + "sec";
        public string StateSuffixLabel => "("+visibleTimer + ") sec  ";
        public virtual BackButtonAction BackButtonAffected => BackButtonAction.Close;
        protected bool HasCanvasGroup => canvasGroup != null;
        protected bool HasHandler => handler != null;
        protected virtual bool UseVisibleTimer => HasCanvasGroup;
        protected bool UseSlideTransition => slideTransition != SlideTransition.None;
        public void UIUpdate()
        {
            switch (visibleState)
            {
                case VisibleState.Visible:
                    OpenUpdate();
                    break;
                case VisibleState.TransitionIn:
                    if (ChangeVisibleTimer(TransitionTime * transitionInSpeed))
                    {
                        openTimer = 0;
                        visibleState = VisibleState.Visible;
                        OnOpen();
                    }
                    else
                    {
                        OpenAnim(visibleTimer);
                    }
                    break;
                case VisibleState.TransitionOut:
                    if (ChangeVisibleTimer(-TransitionTime * transitionOutSpeed))
                    {
                        openTimer = 0;
                        visibleState = VisibleState.NotVisible;
                        OnClose();
                    }
                    else
                    {
                        CloseAnim(visibleTimer);
                    }
                    break;
            }
        }
        protected virtual void OpenUpdate() => openTimer += TransitionTime;
        public virtual void Toggle(bool isOn)
        {
            if (InTransition) return;
            if (isOn)
                Open();
            else
                Close();
        }
        public void Toggle() => Toggle(Visible);
        [Button]
        public void Open() => Open(false);
        void Open(bool safe)
        {
            if(safe && visibleState != VisibleState.NotVisible) return;
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                OpenImmediate();
                return;
            }
#endif
            if (!UseVisibleTimer)
            {
                OpenImmediate();
                return;
            }
            if (ImmediateOpenActions) OpenActions();
            visibleState = VisibleState.TransitionIn;
            OpenAnim(visibleTimer);
            if (canvasGroup != null)
            {
                if (raycastBlockMode == RaycastBlockMode.OnVisible)
                {
                    canvasGroup.blocksRaycasts = canvasGroup.interactable = false;
                }
                else
                {
                    canvasGroup.blocksRaycasts = true;
                    canvasGroup.interactable = false;
                }
            }
            AddToOpenedPopups();
        }
        void OpenImmediate()
        {
            visibleState = VisibleState.Visible;
            visibleTimer = 1;
            OnOpen();
#if UNITY_EDITOR
            if (Application.isPlaying && ImmediateOpenActions)
#else
            if(ImmediateOpenActions)
#endif
                OpenActions();
        }
        protected virtual void OnOpen()
        {
            openTimer = 0;
            OpenAnim();
            Transform.anchoredPosition = cachedPosition;
            if (canvasGroup != null) canvasGroup.blocksRaycasts = canvasGroup.interactable = true;
            if (Application.isPlaying && !ImmediateOpenActions) OpenActions();
        }
        protected virtual void OpenActions(){}
        protected void AddToOpenedPopups()
        {
            if (OpenedPopups == null) OpenedPopups = new List<CastlePopup>(32) {this};
            else
            {
                for (var i = 0; i < OpenedPopups.Count; i++)
                {
                    if (OpenedPopups[i] != this) continue;
                    OpenedPopups.Move(i,OpenedPopups.Count-1);
                    return;
                }
                OpenedPopups.Add(this);
            }
        }
        [Button]
        public void Close() => Close(false);
        public virtual void Close(bool safe)
        {
            if (safe && visibleState != VisibleState.Visible) return;
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                CloseImmediate();
                return;
            }
#endif
            if (!UseVisibleTimer)
            {
                OpenImmediate();
                return;
            }
            if (ImmediateCloseActions) CloseActions();
            visibleState = VisibleState.TransitionOut;
            CloseAnim(visibleTimer);
            if (canvasGroup != null)
            {
                if (raycastBlockMode == RaycastBlockMode.OnVisibleAll)
                {
                    canvasGroup.blocksRaycasts = true;
                    canvasGroup.interactable = false;
                }
                else
                {
                    canvasGroup.blocksRaycasts = canvasGroup.interactable = false;
                }
            }
        }
        void CloseImmediate()
        {
            visibleState = VisibleState.NotVisible;
            visibleTimer = 0;
            OnClose();
#if UNITY_EDITOR
            if (Application.isPlaying && ImmediateCloseActions)
#else
            if(ImmediateCloseActions)
#endif
            CloseActions();
        }
        protected virtual void OnClose()
        {
            CloseAnim();
            Transform.anchoredPosition = cachedPosition;
            if (canvasGroup != null) canvasGroup.blocksRaycasts = canvasGroup.interactable = false;
            if (Application.isPlaying && !ImmediateCloseActions) CloseActions();
        }
        protected virtual void CloseActions(){}
        /// <summary>
        /// Animation driven by progress float moving from 0 to 1. If called without parameters, run last step in animation.
        /// </summary>
        /// <param name="progress"></param>
        protected virtual void OpenAnim(float progress = 1)
        {
            if (canvasGroup != null && AnimateCanvasGroup) CanvasAlphaAnim(progress);
            if (slideTransition != SlideTransition.None) SlideAnimIn(progress);
        }
        /// <summary>
        /// Returns popup position during slide in transition
        /// </summary>
        /// <param name="progress"></param>
        protected virtual Vector2 SlideInPosition(float progress) =>
            (useSlideCurve & SlideCurveMode.In) != 0 ? Vector2.LerpUnclamped(cachedPosition + SlideOffset, cachedPosition, slideCurve.Evaluate(progress)) :
            Vector2.Lerp(cachedPosition + SlideOffset, cachedPosition, Mathf.SmoothStep(0, 1, progress));
        /// <summary>
        /// Animation driven by progress float moving from 1 to 0. If called without parameters, run last step in animation.
        /// </summary>
        /// <param name="progress"></param>
        protected virtual void CloseAnim(float progress=0)
        {
            if (canvasGroup != null && AnimateCanvasGroup) CanvasAlphaAnim(progress);
            if (slideTransition != SlideTransition.None) SlideAnimOut(progress);
        }
        /// <summary>
        /// Returns popup position during slide out transition
        /// </summary>
        /// <param name="progress"></param>
        protected virtual Vector2 SlideOutPosition(float progress) =>
            (useSlideCurve & SlideCurveMode.Out) != 0 ? Vector2.LerpUnclamped(cachedPosition + (transitionFollowThrough ? -SlideOffset : SlideOffset), cachedPosition,UseSeparateCurves ? slideCurveOut.Evaluate(progress) : slideCurve.Evaluate(progress)) :
            Vector2.Lerp(cachedPosition + (transitionFollowThrough ? -SlideOffset : SlideOffset), cachedPosition, Mathf.SmoothStep(0, 1, progress));
        /// <summary>
        /// Increases/Decreases visible timer and keeps it between 0 and 1
        /// </summary>
        /// <param name="diff">Num to change visible timer by. Can be negative</param>
        /// <returns>If value would have gone out of bounds, returns true</returns>
        public bool ChangeVisibleTimer(float diff)
        {
            visibleTimer += diff;
            if (visibleTimer is > 0f and < 1f) return false;
            visibleTimer = Mathf.Clamp(visibleTimer, 0f, 1f);
            
            return true;
        }
        public void AnimationStep(Action<float> animStep, float timer,float start,float end) => animStep(Tools.InverseLerp(start,end,timer));
        protected void CanvasAlphaAnim(float progress) => canvasGroup.alpha = Mathf.Lerp(0, 1, progress);
        protected void SlideAnimIn(float progress) => Transform.anchoredPosition = SlideInPosition(progress);
        protected void SlideAnimOut(float progress) => Transform.anchoredPosition = SlideOutPosition(progress);
        /// <summary>
        /// Enables Android Back Button Logic. Put this in Update or something.
        /// </summary>
        public static void HandleBackButton()
        {
            if (!Input.GetKeyDown(KeyCode.Escape) || OpenedPopups == null) return;
            for (var i = OpenedPopups.Count - 1; i >= 0; i--)
            {
                if (OpenedPopups[i].NotVisible) continue;
                switch (OpenedPopups[i].BackButtonAffected)
                {
                    case BackButtonAction.Passthrough:
                        continue;
                    case BackButtonAction.Close:
                        OpenedPopups[i].Close();
                        return;
                    case BackButtonAction.CloseWhenVisible:
                        if (OpenedPopups[i].InTransition) return;
                        OpenedPopups[i].Close();
                        return;
                    case BackButtonAction.Stop:
                        return;
                }
            }
        }
#if UNITY_EDITOR
        public CastlePopupHandler Handler => Transform.GetParentComponent(out CastlePopupHandler handler) ? handler : null;
        private void Update()
        {
            if (!Application.isPlaying)
            {
                if (Visible)
                {
                    cachedPosition = Transform.anchoredPosition;
                }
                if (canvasGroup == null)
                {
                    TryGetComponent(out canvasGroup);
                }
            }
        }
        private void OnDestroy()
        {
            if (handler != null) handler.popups=handler.popups.RemoveFromArray(this);
        }
        protected virtual void Reset()
        {
            TryGetComponent(out transform);
            TryGetComponent(out canvasGroup);
            handler = Handler;
            if (handler != null && !handler.popups.Contains(this)) handler.popups = handler.popups.AddToArray(this);
        }
        private void OnTransformParentChanged()
        {
            var newHandler = Handler;
            if (newHandler == handler) return;
            if (handler != null) handler.popups=handler.popups.RemoveFromArray(this);
            handler = newHandler;
            if(handler != null) handler.popups = handler.popups.AddToArray(this);
        }
#endif
    }
}