using System;
using UnityEngine;
using UnityEngine.EventSystems;

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
        public enum TapState
        {
            NotTapped,
            Tapped,
            Held,
            Released
        }
        public static Camera Cam;
        private static LayerMask _colliderLayerMask;
        public static Vector3 WorldTapPosition => Cam.ScreenToWorldPoint(lastTappedPos.RepZ(-1));
        public static void Init(Camera cam, LayerMask colliderLayerMask)
        {
            Cam = cam;
            _colliderLayerMask = colliderLayerMask;
            _colliderBuffer = new Collider2D[16];
        }
        /// <summary>
        /// Call this function using your game manager to handle touch input.
        /// </summary>
        public static void FUpdate() => UpdateTapState();
        public static bool CheckPoint(Vector2 position,out CastleObject hoveredObject)
        {
            _bufferUsed = Physics2D.OverlapPointNonAlloc(position,_colliderBuffer,_colliderLayerMask);
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
                        _fingerId = newTouch.fingerId;
                        StartTap(newTouch.position);
                    }
                    else if (!IsMobile && Input.GetMouseButtonDown(0))
                    {
                        StartTap(Input.mousePosition);
                    }
                    break;
                case TapState.Tapped:
                case TapState.Held:
                    _tapTimer += Time.unscaledDeltaTime;
                    if (IsMobile && TryGetTouchWithID(_fingerId, out var currTouch))
                    {
                        if (currTouch.phase is TouchPhase.Ended or TouchPhase.Canceled)
                        {
                            lastTappedPos = currTouch.position;
                            ReleaseTap();
                        }
                        else
                        {
                            HoldTap(currTouch.position);
                        }
                    }
                    else if(!IsMobile && Input.GetMouseButton(0))
                    {
                        HoldTap(Input.mousePosition);
                    }
                    else
                    {
                        ReleaseTap();
                    }
                    break;
            }
        }
        public static bool QuickTapped => CurrentTapState is TapState.Released &&
                                          (lastTappedPos - firstTappedPos).sqrMagnitude < Settings.Instance.QuickTapDistanceThreshold &&
                                          _tapTimer < Settings.Instance.QuickTapTimerThreshold;
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
                _bufferUsed = Physics2D.OverlapPointNonAlloc(WorldTapPosition,_colliderBuffer,_colliderLayerMask);
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
            if (Input.touchCount > 0)
            {
                for (var i = 0; i < Input.touchCount; i++)
                {
                    if (Input.GetTouch(i).phase == TouchPhase.Began)
                    {
                        newTouch = Input.GetTouch(i);
                        return true;
                    }
                }
            }

            newTouch = new Touch
            {
                fingerId = 999,
                phase = TouchPhase.Ended,
                position = lastTappedPos
            };
            return false;
        }
        public static bool TryGetTouchWithID(int id, out Touch touch)
        {
            if (Input.touchCount > 0)
            {
                for (int i = 0; i < Input.touchCount; i++)
                {
                    if (Input.GetTouch(i).fingerId == id)
                    {
                        touch = Input.GetTouch(i);
                        return true;
                    }
                }
            }
            touch = default;
            return false;
        }
        public static bool IsTouchingUI() =>
            CurrentTapState != TapState.NotTapped && (IsMobile
                ? EventSystem.current.IsPointerOverGameObject(_fingerId)
                : EventSystem.current.IsPointerOverGameObject());
        public static bool IsMobile
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            get => false;
#else
            get => true;
#endif
        }
    }
}
