using System.Collections;
using System.Collections.Generic;
using Castle.Tools;
using UnityEngine;

namespace Castle.Core.UI
{
    public class CastlePopupHandler : MonoBehaviour
    {
        public Canvas canvas;
        public CastlePopup[] popups;
        private Queue<CastlePopup> openedPopups;

        public virtual void HandlerUpdate()
        {
            if (!popups.IsSafe()) return;
            for (var i = 0; i < popups.Length; i++)
            {
                //popups[i].UIUpdate();
            }

            HandleBackButton();
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

        public virtual void HandleBackButton()
        {
            if (openedPopups != null && openedPopups.Count > 0 && Input.GetKeyDown(KeyCode.Escape))
            {
                var l = openedPopups.Peek();

            }
        }
    }
}