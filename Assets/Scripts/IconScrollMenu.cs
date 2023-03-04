using System;
using Castle.Core.UI.Menu;
using UnityEngine;
using UnityEngine.UI;

#if ODIN_INSPECTOR
[Serializable]
public class IconScrollMenu : CastleScrollMenu<IconScrollMenu.SimpleScrollIcon, Sprite>
{
    public class SimpleScrollIcon : MenuOption
    {
        public Image icon;
        public override void Load(Sprite sprite, int i, bool initialLoad = false)
        {
            base.Load(sprite, i,initialLoad);
            icon.sprite = sprite;
        }
    }
}
#endif