using UnityEngine;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace Castle.Core
{
    public abstract class CastleObject : MonoBehaviour
    {
        [HideInInspector,SerializeField]
        private new Transform transform;
        public Transform Transform => transform ? transform : transform = base.transform;
        [HideInInspector,SerializeField]
        private new Collider2D collider;
#if ODIN_INSPECTOR
        [ShowInInspector,InfoBox("No Collider on Object",InfoMessageType.Error,VisibleIf = "NoColliderAttached")]
#endif
        public Collider2D Collider
        {
            get
            {
                if (collider != null) return collider;
                TryGetComponent(out collider);
                return collider;
            }
        }
        protected float holdTimer,holdFloored;
        protected float hoverTimer,hoverFloored;
        protected Vector2 holdOffset,holdDelta;
        protected bool NoColliderAttached => collider == null;
        protected bool NoTransformAttached => transform == null;
        public virtual float ZPos => Transform.position.z;
        public virtual void Tap(Vector2 tapPosition)
        {
            holdTimer =
                holdFloored = 0;
            holdDelta = Vector2.zero;
            holdOffset = tapPosition;
        }
        public virtual void Hold(Vector2 tapPosition, bool pointerOnObject)
        {
            if (holdFloored < Mathf.FloorToInt(holdTimer))
            {
                holdFloored = Mathf.FloorToInt(holdTimer);
            }
            holdTimer += Time.unscaledDeltaTime;
            holdDelta = holdOffset - tapPosition;
        }
        public virtual void Release()
        {
            holdTimer = holdFloored = 0;
            holdDelta = holdOffset = Vector2.zero;
        }
        public void Drag(Vector3 offset = default) =>
            Transform.Move2D(CastleManager.WorldTapPosition + offset);
        public void Drag(float snappiness, Vector3 offset = default) =>
            Transform.Move2DLerp(CastleManager.WorldTapPosition + offset, snappiness);
#if UNITY_EDITOR
        protected void GetCollider() => TryGetComponent(out collider);
        protected virtual void Reset()
        {
            if (NoColliderAttached) GetCollider();
            if (NoTransformAttached) transform = base.transform;
        }
#endif
    }
}
