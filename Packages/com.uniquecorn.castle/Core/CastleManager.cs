using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

namespace Castle.Core
{
    public static class CastleManager
    {
        public static TapState CurrentTapState;
        public static CastleObject SelectedObject;
        private static Vector2 firstTappedPos,lastTappedPos;
        private static float _tapTimer;
        public static bool NotTapped => CurrentTapState is TapState.Released or TapState.NotTapped;
        private static int _fingerId,_bufferUsed;
        private static Collider2D[] _colliderBuffer;
        public static Action<Vector2> FirstTouched, QuickTap;
        private static ContactFilter2D _filter;
        public enum TapState
        {
            NotTapped,
            Tapped,
            Held,
            Released
        }
        public static Camera Cam;
        public static Vector3 WorldTapPosition => Cam.ScreenToWorldPoint(lastTappedPos.RepZ(-1));
        /// <summary>
        /// Call this function using your game manager to initialize tap detection.
        /// </summary>
        public static void Init(Camera cam, LayerMask colliderLayerMask)
        {
            Cam = cam;
            _colliderBuffer = new Collider2D[16];
            _filter = new ContactFilter2D
            {
                layerMask = colliderLayerMask,
                useLayerMask = true
            };
            EnhancedTouchSupport.Enable();
        }
        /// <summary>
        /// Call this function using your game manager to handle touch input.
        /// </summary>
        public static void FUpdate() => UpdateTapState();
        public static bool CheckPoint(Vector2 position,out CastleObject hoveredObject)
        {
            _bufferUsed = Physics2D.OverlapPoint(position,_filter,_colliderBuffer);
            var closestDist = 999999999f;
            CastleObject _hoveredObject = null;
            for (var i = 0; i < _bufferUsed; i++)
            {
                if (!_colliderBuffer[i].TryGetComponent<CastleObject>(out var b)) continue;
                if (b.ZPos < closestDist)
                {
                    closestDist = b.ZPos;
                    _hoveredObject = b;
                }
            }
            hoveredObject = _hoveredObject;
            return hoveredObject != null;
        }
        public static void UpdateTapState()
        {
            switch (CurrentTapState)
            {
                case TapState.NotTapped:
                case TapState.Released:
                    _tapTimer = 0;
                    CurrentTapState = TapState.NotTapped;
                    if (IsMobile && TryNewTouch(out var newTouch))
                    {
                        _fingerId = newTouch.touchId;
                        StartTap(newTouch.screenPosition);
                    }
                    else if (!IsMobile && Mouse.current.leftButton.wasPressedThisFrame)
                    {
                        StartTap(Mouse.current.position.value);
                    }
                    break;
                case TapState.Tapped:
                case TapState.Held:
                    _tapTimer += Time.unscaledDeltaTime;
                    if (IsMobile && TryGetTouchWithID(_fingerId, out var currTouch))
                    {
                        if (currTouch.phase is TouchPhase.Ended or TouchPhase.Canceled)
                        {
                            lastTappedPos = currTouch.screenPosition;
                            ReleaseTap();
                        }
                        else
                        {
                            HoldTap(currTouch.screenPosition);
                        }
                    }
                    else if(!IsMobile && Mouse.current.leftButton.isPressed)
                    {
                        HoldTap(Mouse.current.position.value);
                    }
                    else
                    {
                        ReleaseTap();
                    }
                    break;
            }
        }
        public static bool QuickTapped => CurrentTapState is TapState.Released &&
                                          (lastTappedPos - firstTappedPos).sqrMagnitude < 3.5f &&
                                          _tapTimer < 0.2f;
        private static void StartTap(Vector2 tapPosition)
        {
            CurrentTapState = TapState.Tapped;
            firstTappedPos = lastTappedPos = tapPosition;
            FirstTouched?.Invoke(firstTappedPos);
            var worldTapPos = WorldTapPosition;
            if (CheckPoint(worldTapPos,out var b))
            {
                SelectedObject = b;
                b.Tap(worldTapPos);
            }
        }
        private static void HoldTap(Vector2 tapPosition)
        {
            lastTappedPos = tapPosition;
            CurrentTapState = TapState.Held;
            if (SelectedObject != null)
            {
                _bufferUsed = Physics2D.OverlapPoint(WorldTapPosition,_filter,_colliderBuffer);
                var pointerOnObject = false;
                for (var i = 0; i < _bufferUsed; i++)
                {
                    if (_colliderBuffer[i] != SelectedObject.Collider) continue;
                    pointerOnObject = true;
                    break;
                }
                SelectedObject.Hold(tapPosition,pointerOnObject);
            }
        }
        private static void ReleaseTap()
        {
            CurrentTapState = TapState.Released;
            if (QuickTapped) QuickTap?.Invoke(lastTappedPos);
            if (SelectedObject != null)
            {
                SelectedObject.Release();
                SelectedObject = null;
            }
        }
        public static bool TryNewTouch(out Touch newTouch)
        {
            foreach (var touch in Touch.activeTouches)
            {
                if(touch.phase != TouchPhase.Began) continue;
                newTouch = touch;
                return true;
            }
            newTouch = default;
            return false;
        }
        public static bool TryGetTouchWithID(int id, out Touch fetchedTouch)
        {
            foreach (var touch in Touch.activeTouches)
            {
                if(touch.touchId != id) continue;
                fetchedTouch = touch;
                return true;
            }
            fetchedTouch = default;
            return false;
        }
        public static bool IsTouchingUI() =>
            CurrentTapState != TapState.NotTapped && (IsMobile
                ? EventSystem.current.IsPointerOverGameObject(_fingerId)
                : EventSystem.current.IsPointerOverGameObject());
        public static bool IsMobile => !Input.mousePresent && Input.touchSupported;
    }
}