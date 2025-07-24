using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Castle.Core.UI
{
    [Serializable]
    public abstract class CastleMenu<T0, T1> where T0 : CastleMenu<T0, T1>.MenuOption
    {
        public abstract class MenuOption : MonoBehaviour
        {
            [HideInInspector] 
            public int index;
            [ReadOnly]
            public int trueIndex;
            [HideIf("TransformAttached")]
            public new RectTransform transform;
            public bool TransformAttached => transform != null;
            [NonSerialized]
            public CastleMenu<T0, T1> menu;
            public T1 LoadedItem => menu.Arr[trueIndex];
            public virtual void Hide() => gameObject.SetActive(false);
            public virtual void Init(int i, CastleMenu<T0, T1> menu)
            {
                this.menu = menu;
                index = i;
            }
            public virtual void Load(T1 item, int i, bool initialLoad = false)
            {
                trueIndex = i;
                if (!gameObject.activeSelf)
                {
                    gameObject.SetActive(true);
                }
            }
#if UNITY_EDITOR
            public void Reset() => transform = base.transform as RectTransform;
#endif
        }
        public virtual IList<T1> Arr => CachedArray;
        public IList<T1> CachedArray;
        [HideInInspector]
        public bool horizontal;
        [HideInInspector]
        public HorizontalOrVerticalLayoutGroup layoutGroup;
        public RectTransform OptionTransform => options is {Length: > 0}
            ? options[0].transform
            : prefab.transform;
        public abstract RectTransform Content { get; }
        [NonSerialized] public T0[] options;
        public T0 prefab;
        public virtual int OptionsNum(IList<T1> array) => array.Count;
        public int OptionsNum() => OptionsNum(Arr);
        public virtual void CreateMenu(IList<T1> array)
        {
            CachedArray = array;
            if (Content.TryGetComponent(out layoutGroup))
            {
                horizontal = layoutGroup is HorizontalLayoutGroup;
            }
            if (prefab.transform.parent == Content)
            {
                prefab.gameObject.SetActive(false);
            }
            var optionsToCreate = OptionsNum(array);
            if (options == null)
            {
                options = new T0[optionsToCreate];
                for (var i = 0; i < options.Length; i++)
                {
                    options[i] = CreateOption(i);
                }
            }
            else
            {
                if (options.Length < optionsToCreate)
                {
                    var menuOptions = new T0[optionsToCreate];
                    for (var i = 0; i < optionsToCreate; i++)
                    {
                        if (i < options.Length)
                        {
                            menuOptions[i] = options[i];
                        }
                        else
                        {
                            menuOptions[i] = CreateOption(i);
                        }
                    }

                    options = menuOptions;
                }
                else if (options.Length >= optionsToCreate)
                {
                    var menuOptions = new T0[optionsToCreate];
                    for (var i = 0; i < options.Length; i++)
                    {
                        if (i < menuOptions.Length)
                        {
                            menuOptions[i] = options[i];
                        }
                        else
                        {
                            Object.Destroy(options[i].gameObject);
                        }
                    }
                    options = menuOptions;
                }
            }
            InitialLoad();
        }
        protected virtual void InitialLoad()
        {
            for (var i = 0; i < options.Length; i++)
            {
                options[i].Load(Arr[i], i,true);
            }
        }
        public virtual T0 CreateOption(int i)
        {
            var option = Object.Instantiate(prefab, Content);
            option.Init(i, this);
            return option;
        }
#if UNITY_EDITOR
        protected bool CanCreateOption() => prefab == null && Content != null;
        [Button, ShowIf("CanCreateOption")]
        protected void CreateOptionPrefab()
        {
            var go = new GameObject(typeof(T0).Name, typeof(RectTransform));
            go.transform.SetParent(Content);
            prefab = go.AddComponent<T0>();
            prefab.Reset();
        }
#endif
    }
}