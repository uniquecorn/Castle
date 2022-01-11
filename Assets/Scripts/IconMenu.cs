using Castle.Core.UI.Menu;
using UnityEngine;
using UnityEngine.UI;
#if ODIN_INSPECTOR
[System.Serializable]
public class IconMenu : CastleSimpleMenu<IconMenu.SimpleIcon,Sprite>
{
    public class SimpleIcon : MenuOption
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