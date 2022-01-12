using UnityEngine;

namespace Castle.Core.UI
{
    public class ScalePopup : CastlePopup
    {
        protected override bool UseVisibleTimer => true;
        protected override void OpenAnim(float progress = 1)
        {
            base.OpenAnim(progress);
            ScaleAnim(progress);
        }
        protected override void CloseAnim(float progress = 0)
        {
            base.CloseAnim(progress);
            ScaleAnim(progress);
        }
        protected virtual void ScaleAnim(float progress) => transform.localScale = Vector3.one*Mathf.SmoothStep(0, 1, progress);
    }
}
