using System.Collections.Generic;
using UnityEngine;

namespace Castle
{
    public static class RendererExtensions
    {
        public static void SetSorting<T>(this T renderer, int sortingLayerID, int sortingOrder) where T : Renderer
        {
            renderer.sortingLayerID = sortingLayerID;
            renderer.sortingOrder = sortingOrder;
        }

        public static void SetSorting<T>(this IList<T> renderers, int sortingLayerID, int sortingOrder) where T : Renderer
        {
            if (!renderers.IsSafe()) return;
            foreach (var r in renderers)
            {
                SetSorting(r,sortingLayerID,sortingOrder);
            }
        }
        public static void SetSorting<T, T2>(this T controller, System.Func<T, T2> getRenderer, int sortingLayerID,
            int sortingOrder) where T2 : Renderer =>
            SetSorting(getRenderer(controller),sortingLayerID,sortingOrder);
        public static void SetSorting<T, T2>(this IList<T> controllers, System.Func<T, T2> getRenderer, int sortingLayerID,
            int sortingOrder) where T2 : Renderer
        {
            if(!controllers.IsSafe())return;
            foreach (var controller in controllers)
            {
                SetSorting(getRenderer(controller), sortingLayerID, sortingOrder);
            }
        }
        public static void SetProperty<T>(this T renderer, System.Action<MaterialPropertyBlock> action) where T : Renderer
        {
            Tools.Block.Clear();
            renderer.GetPropertyBlock(Tools.Block);
            action(Tools.Block);
            renderer.SetPropertyBlock(Tools.Block);
            Tools.Block.Clear();
        }
        public static void SetProperty<T>(this IList<T> renderers, System.Action<MaterialPropertyBlock> action) where T : Renderer
        {
            if (!renderers.IsSafe()) return;
            Tools.Block.Clear();
            renderers[0].GetPropertyBlock(Tools.Block);
            action(Tools.Block);
            foreach (var r in renderers)
            {
                r.SetPropertyBlock(Tools.Block);
            }
            Tools.Block.Clear();
        }
        public static void SetProperty<T,T2>(this IList<T> controllers,System.Func<T,T2> getRenderer, System.Action<MaterialPropertyBlock> action) where T2 : Renderer
        {
            if (!controllers.IsSafe()) return;
            Tools.Block.Clear();
            getRenderer(controllers[0]).GetPropertyBlock(Tools.Block);
            action(Tools.Block);
            foreach (var c in controllers)
            {
                getRenderer(c).SetPropertyBlock(Tools.Block);
            }
            Tools.Block.Clear();
        }
    }
}