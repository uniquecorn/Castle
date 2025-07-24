using UnityEngine;
using UnityEngine.UI;

namespace Castle
{
    public static class UIExtensions
    {
        public static void FillRect(this RectTransform rectTransform)
        {
            rectTransform.anchorMax = Vector2.one;
            rectTransform.anchorMin = rectTransform.sizeDelta = Vector2.zero;
        }
        public static void SetSize(this LayoutGroup layoutGroup)
        {
            if (!layoutGroup.TryGetComponent<RectTransform>(out var rt)) return;
            rt.sizeDelta = new Vector2(layoutGroup.preferredWidth, layoutGroup.preferredHeight);
        }
        public static void SetSizeX(this LayoutGroup layoutGroup)
        {
            if (!layoutGroup.TryGetComponent<RectTransform>(out var rt)) return;
            rt.sizeDelta = new Vector2(layoutGroup.preferredWidth, rt.sizeDelta.y);
        }
        public static void SetSizeY(this LayoutGroup layoutGroup)
        {
            if (!layoutGroup.TryGetComponent<RectTransform>(out var rt)) return;
            rt.sizeDelta = new Vector2(rt.sizeDelta.x,layoutGroup.preferredHeight);
        }
        public static void SetSizeDirection(this HorizontalOrVerticalLayoutGroup layoutGroup)
        {
            switch (layoutGroup)
            {
                case VerticalLayoutGroup verticalLayoutGroup:
                    SetSizeY(verticalLayoutGroup);
                    break;
                case HorizontalLayoutGroup horizontalLayoutGroup:
                    SetSizeX(horizontalLayoutGroup);
                    break;
            }
        }
        public static int CountCornersVisible(this RectTransform rectTransform, Camera cam)
        {
            var screenBounds = Tools.ScreenBounds;
            var objectCorners = new Vector3[4];
            rectTransform.GetWorldCorners(objectCorners);
            var visibleCorners = 0;
            for (var i = 0; i < objectCorners.Length; i++) // For each corner in rectTransform
            {
                if (screenBounds.Contains(cam.WorldToScreenPoint(objectCorners[i]))) // If the corner is inside the screen
                {
                    visibleCorners++;
                }
            }
            return visibleCorners;
        }
        public static bool IsFullyVisibleFrom(this RectTransform rectTransform,Camera cam) => CountCornersVisible(rectTransform,cam) == 4;
        public static bool IsVisibleFrom(this RectTransform rectTransform,Camera cam) => CountCornersVisible(rectTransform,cam) > 0;
    }
}