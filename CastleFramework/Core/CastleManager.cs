using Castle.Tools;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Castle.Core
{
    public static class CastleManager
    {
        public static TapState CurrentTapState;
        public static CastleObject SelectedObject;
        public static Vector2 firstTappedPos,lastTappedPos;
        private static float _tapTimer;
        public static bool NotTapped => CurrentTapState is TapState.Released or TapState.NotTapped;
        private static int _fingerId,_bufferUsed;
        private static Collider2D[] _colliderBuffer;
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
        public static void FUpdate()
        {
            UpdateTapState();
        }
        public static bool CheckPoint(Vector2 position,out CastleObject hoveredObject)
        {
            _bufferUsed = Physics2D.OverlapPointNonAlloc(position,_colliderBuffer,_colliderLayerMask);
            return TryGetClosestObject(out hoveredObject);
        }
        private static bool TryGetClosestObject(out CastleObject hoveredObject,bool excludeSelected = true)
        {
            hoveredObject = null;
            if (!_colliderBuffer.IsSafe()) return false;
            var closestDist = 999999999f;
            for (var i = 0; i < _bufferUsed; i++)
            {
                if (!_colliderBuffer[i].TryGetComponent<CastleObject>(out var b)) continue;
                if (excludeSelected && b == SelectedObject) continue;
                if (b.ZPos < closestDist)
                {
                    closestDist = b.ZPos;
                    hoveredObject = b;
                }
            }
            return hoveredObject != null;
        }
        public static void UpdateTapState()
        {
            switch (CurrentTapState)
            {
                case TapState.NotTapped:
                    _tapTimer = 0;
                    if (IsInputTapped(out var tapPos))
                    {
                        StartTap(tapPos);
                    }
                    break;
                case TapState.Tapped:
                    _tapTimer = 0;
                    if (IsInputTapped(out var tapPos2))
                    {
                        HoldTap(tapPos2);
                    }
                    break;
                case TapState.Held:
                    _tapTimer += Time.deltaTime;
                    if (IsInputTapped(out var tapPos3))
                    {
                        HoldTap(tapPos3);
                    }
                    else
                    {
                        ReleaseTap(lastTappedPos);
                    }
                    break;
                case TapState.Released:
                    if (IsInputTapped(out var tapPos4))
                    {
                        StartTap(tapPos4);
                    }
                    else
                    {
                        CurrentTapState = TapState.NotTapped;
                    }
                    break;
            }
        }
        public static bool IsInputTapped(out Vector2 tapPosition)
        {
            tapPosition = default;
            if (IsMobile)
            {
                if (NotTapped)
                {
                    if (!TryNewTouch(out var newTouch)) return false;
                    tapPosition = newTouch.position;
                    _fingerId = newTouch.fingerId;
                    return true;
                }
                var currTouch = CurrentTouch;
                tapPosition = currTouch.position;
                return currTouch.phase is TouchPhase.Began or TouchPhase.Moved or TouchPhase.Stationary;
            }
            else
            {
                if (!Input.GetMouseButtonDown(0)) return false;
                tapPosition = Input.mousePosition;
                return true;
            }
        }
        private static void StartTap(Vector2 tapPosition)
        {
            firstTappedPos = lastTappedPos = tapPosition;
            CurrentTapState = TapState.Tapped;
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
            var worldTapPos = WorldTapPosition;
            if (SelectedObject != null)
            {
                _bufferUsed = Physics2D.OverlapPointNonAlloc(worldTapPos,_colliderBuffer,_colliderLayerMask);
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
        private static void ReleaseTap(Vector2 tapPosition)
        {
            CurrentTapState = TapState.Released;
            if (SelectedObject != null)
            {
                SelectedObject.Release();
                SelectedObject = null;
            }
        }

        public static void TapObject(CastleObject castleObject)
        {
            if (SelectedObject != null)
            {
                SelectedObject.Release();
            }
            SelectedObject = castleObject;
            SelectedObject.Tap(SelectedObject.Transform.position);
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

            newTouch = new Touch()
            {
                fingerId = 999,
                phase = TouchPhase.Ended,
                position = lastTappedPos
            };
            return false;
        }
        public static Touch CurrentTouch => GetTouchWithID(_fingerId);
        public static Touch GetTouchWithID(int id)
        {
            if (Input.touchCount > 0)
            {
                for (int i = 0; i < Input.touchCount; i++)
                {
                    if (Input.GetTouch(i).fingerId == id)
                    {
                        return Input.GetTouch(i);
                    }
                }
            }
            return new Touch()
            {
                fingerId = 999,
                phase = TouchPhase.Ended,
                position = lastTappedPos
            };
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
