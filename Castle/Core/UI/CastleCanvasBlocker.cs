using UnityEngine;
using UnityEngine.UI;

namespace Castle.Core.UI
{
    [RequireComponent(typeof(Image))]
    public class CastleCanvasBlocker : MonoBehaviour
    {
        public Image image;
        [SerializeField,HideInInspector]
        private new RectTransform transform;
        public RectTransform Transform => transform ? transform : transform = (RectTransform)base.transform;
        private void Reset()
        {
            Transform.FillRect();
            TryGetComponent(out image);
            image.color = Color.black.Clear();
        }
    }
}