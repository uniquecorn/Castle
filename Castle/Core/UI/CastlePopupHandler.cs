using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace Castle.Core.UI
{
    public class CastlePopupHandler : MonoBehaviour
    {
        public Canvas canvas;
        public GraphicRaycaster raycaster;
        public CastlePopup[] popups;
        private void Update()
        {
            HandlerUpdate();
            CastlePopup.HandleBackButton();
        }

        public virtual void HandlerUpdate()
        {
            if (!popups.IsSafe()) return;
            for (var i = 0; i < popups.Length; i++)
            {
                popups[i].UIUpdate();
            }
        }

        public T GetPopup<T>() where T : CastlePopup
        {
            for (var i = 0; i < popups.Length; i++)
            {
                if (popups[i] is T popup)
                {
                    return popup;
                }
            }

            return null;
        }
#if UNITY_EDITOR
#if ODIN_INSPECTOR
        [Button]
#else
        [UnityEditor.MenuItem("Get Popups")]
#endif
        public virtual void GetPopups()
        {
            var uiPopups = new List<CastlePopup>();
            if (popups != null)
            {
                uiPopups.AddRange(popups.ClearNullEntries());
            }
            var p = GetComponentsInChildren<CastlePopup>();
            for (var i = 0; i < p.Length; i++)
            {
                if (uiPopups.Contains(p[i])) continue;
                if(p[i].Handler != this) continue;
                p[i].handler = this;
                uiPopups.Add(p[i]);
            }
            popups = uiPopups.ToArray();
        }
#endif
        
    }
}