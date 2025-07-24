using System;
using UnityEngine;

namespace Castle.Core.UI
{
    [Serializable]
    public abstract class CastleSimpleMenu<T0,T1> : CastleMenu<T0,T1> where T0 : CastleMenu<T0, T1>.MenuOption
    {
        public RectTransform content;
        public override RectTransform Content => content;
    }
}