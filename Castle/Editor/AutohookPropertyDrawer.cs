using System;
using UnityEditor;
using UnityEngine;

namespace Castle.Editor
{
    [CustomPropertyDrawer(typeof(AutohookAttribute))]
    public class AutohookPropertyDrawer : HookPropertyDrawer
    {
        protected override bool TryFindTarget(Component main, Type type, out Component component) =>
            main.TryGetComponent(type, out component);
    }
}