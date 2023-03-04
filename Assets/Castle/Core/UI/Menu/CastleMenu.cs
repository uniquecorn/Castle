using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Castle.Core.UI.Menu
{
    #if ODIN_INSPECTOR
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
    [Serializable]
    public abstract class CastleSimpleMenu<T0,T1> : CastleMenu<T0,T1> where T0 : CastleMenu<T0, T1>.MenuOption
    {
        public RectTransform content;
        public override RectTransform Content => content;
    }
    [Serializable]
    public abstract class CastleScrollMenu<T0, T1> : CastleMenu<T0, T1> where T0 : CastleMenu<T0, T1>.MenuOption
    {
        public override IList<T1> Arr => useSortedArray ? sortedArray : CachedArray;
        protected bool useSortedArray;
        protected T1[] sortedArray;
        [HideInInspector] public int[] sortedPositions;
        public ScrollRect scrollRect;
        public override RectTransform Content => scrollRect?.content;

        protected virtual float ViewportSize =>
            horizontal ? scrollRect.viewport.rect.width : scrollRect.viewport.rect.height;
        protected virtual float OptionSize => (horizontal ? OptionTransform.rect.width : OptionTransform.rect.height) +
                                              layoutGroup.spacing;
        protected virtual float ContentSize =>
            horizontal ? scrollRect.content.rect.width : scrollRect.content.rect.height;
        protected virtual float ContentPos => (horizontal ? Content.anchoredPosition.x : Content.anchoredPosition.y) *
                                              (ReversedContentPos ? -1 : 1);
        public virtual int MaxOptions => Mathf.Min(Mathf.FloorToInt(ViewportSize / OptionSize) + 2, Arr.Count);
        protected virtual float BufferSizeInc => OptionSize;
        protected RectTransform bufferB, bufferF;
        protected virtual bool UseLayoutElement => horizontal && layoutGroup.childControlWidth ||
                                                   !horizontal && layoutGroup.childControlHeight;
        protected LayoutElement bufferBLE, bufferFLE;
        public virtual void MoveContent(Vector2 x) => MoveContent();
        private bool ReversedContentPos => horizontal ? Content.anchorMin.x <= 0.01f : Content.anchorMin.y <= 0.01f;
        public float ContentEdgeBack => ContentSize - (ContentPos + ViewportSize);
        public virtual int GetCurrentPosition() =>
            Mathf.Clamp(Mathf.RoundToInt(ContentPos / OptionSize) - 1, 0, UnloadedDataCount);
        protected int lastCurrPos;
        public virtual void RefreshMenu() => RefreshMenu(GetCurrentPosition());
        public override int OptionsNum(IList<T1> array) => Mathf.Min(Mathf.FloorToInt(ViewportSize / OptionSize) + 2, array.Count);
        public int UnloadedDataCount => Arr.Count - OptionsNum();
        public override void CreateMenu(IList<T1> array)
        {
            useSortedArray = false;
            base.CreateMenu(array);
        }

        public virtual void RefreshMenu(int currPos)
        {
            lastCurrPos = currPos;
            for (int i = currPos; i < currPos + options.Length; i++)
            {
                if (i >= currPos + OptionsNum())
                {
                    options[i % options.Length].Hide();
                }
                else
                {
                    options[i % options.Length].Load(Arr[i], i);
                    options[i % options.Length].transform.SetSiblingIndex((i - currPos));
                }
            }
            CalculateBuffers(currPos);
        }
        public void MoveContent()
        {
            var currPos = GetCurrentPosition();
            if (lastCurrPos != currPos) RefreshMenu(currPos);
        }
        protected void CreateBuffers()
        {
            if (bufferB == null)
            {
                var b = new GameObject("BufferB");
                b.transform.parent = Content;
                bufferB = b.AddComponent<RectTransform>();
                if (UseLayoutElement)
                {
                    bufferBLE = b.AddComponent<LayoutElement>();
                }
            }

            if (bufferF == null)
            {
                var b = new GameObject("BufferF");
                b.transform.parent = Content;
                bufferF = b.AddComponent<RectTransform>();
                if (UseLayoutElement)
                {
                    bufferFLE = b.AddComponent<LayoutElement>();
                }
            }
        }
        public virtual void CalculateBuffers(int currPos) => SetBuffers(Arr.Count - (currPos + OptionsNum()), currPos);
        public virtual void SetBuffers(int front, int back)
        {
            bufferF.gameObject.SetActive(front > 0);
            bufferB.gameObject.SetActive(back > 0);
            if (UseLayoutElement)
            {
                if (horizontal)
                {
                    bufferBLE.minWidth = back * BufferSizeInc;
                    bufferFLE.minWidth = front * BufferSizeInc;
                }
                else
                {
                    bufferBLE.minHeight = back * BufferSizeInc;
                    bufferFLE.minHeight = front * BufferSizeInc;
                }
            }
            else
            {
                if (horizontal)
                {
                    bufferF.sizeDelta = Vector2.right * front * BufferSizeInc;
                    bufferB.sizeDelta = Vector2.right * back * BufferSizeInc;
                }
                else
                {
                    bufferF.sizeDelta = Vector2.up * front * BufferSizeInc;
                    bufferB.sizeDelta = Vector2.up * back * BufferSizeInc;
                }
            }

            bufferB.SetAsFirstSibling();
            bufferF.SetAsLastSibling();
        }

        protected override void InitialLoad()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(Content);
            CreateBuffers();
            RefreshMenu(0);
            scrollRect.onValueChanged.RemoveListener(MoveContent);
            scrollRect.onValueChanged.AddListener(MoveContent);
        }
        public virtual void SortArray(Func<T1, bool> keepSlot, bool refreshMenu = true)
        {
            var _sorted = new List<T1>();
            var _sortedPos = new List<int>();
            for (int i = 0; i < CachedArray.Count; i++)
            {
                if (keepSlot(CachedArray[i]))
                {
                    _sorted.Add(CachedArray[i]);
                    _sortedPos.Add(i);
                }
            }

            sortedArray = _sorted.ToArray();
            sortedPositions = _sortedPos.ToArray();
            useSortedArray = true;
            if (refreshMenu)
            {
                RefreshMenu();
            }
        }
    }

    public abstract class DynamicSpacingCastleScrollMenu<T0, T1> : CastleScrollMenu<T0, T1>
        where T0 : CastleMenu<T0, T1>.MenuOption
    {
        public float totalSpacing;
        public abstract int FixedDrawnOptions { get; }
        public abstract float GetSpacing(T1 item);
        public override int OptionsNum(IList<T1> array) => Mathf.Min(FixedDrawnOptions, array.Count);
        protected override void InitialLoad()
        {
            CalculateTotalSpacing();
            base.InitialLoad();
        }
        protected void CalculateTotalSpacing()
        {
            totalSpacing = 0f;
            for (var i = 0; i < Arr.Count; i++)
            {
                if (i > 0 && i < Arr.Count - 1)
                {
                    totalSpacing += layoutGroup.spacing;
                }
                totalSpacing += GetSpacing(Arr[i]);
            }
        }
        public override int GetCurrentPosition()
        {
            var pos = ContentPos;
            var h = 0f;
            if (pos <= 0)
            {
                return 0;
            }
            var _currPos = Arr.Count - 1;
            for (var i = 0; i < Arr.Count - 1; i++)
            {
                
                var x = h;
                if (i > 0)
                {
                    h += layoutGroup.spacing;
                }
                h += GetSpacing(Arr[i]);
                if (pos < x)
                {
                    _currPos = i;
                }
                else if (pos >= x && pos < h)
                {
                    if (pos > Mathf.Lerp(x, h, 0.5f))
                    {
                        _currPos = i+1;
                    }
                    else
                    {
                        _currPos = i;
                    }
                    break;
                }
            }
            return Mathf.Clamp(_currPos-1, 0, Arr.Count - MaxOptions);
        }

        public override void CalculateBuffers(int currPos)
        {
            var startSpacing = 0f;
            var currSpacing = 0f;
            for (var i = 0; i < currPos + MaxOptions; i++)
            {
                if (i >= currPos)
                {
                    
                    currSpacing += GetSpacing(Arr[i]);
                    if (i < currPos + MaxOptions - 1)
                    {
                        currSpacing += layoutGroup.spacing;
                    }
                }
                else
                {
                    if (i > 0)
                    {
                        startSpacing += layoutGroup.spacing;
                    }
                    startSpacing += GetSpacing(Arr[i]);
                }
            }
            var endSpacing = totalSpacing - (startSpacing + currSpacing);
            SetBuffersDirect(endSpacing,startSpacing);
        }

        public void SetBuffersDirect(float front, float back)
        {
            bufferF.gameObject.SetActive(front > 0.001f);
            bufferB.gameObject.SetActive(back > 0.001f);
            if (UseLayoutElement)
            {
                if (horizontal)
                {
                    bufferBLE.minWidth = back;
                    bufferFLE.minWidth = front;
                }
                else
                {
                    bufferBLE.minHeight = back;
                    bufferFLE.minHeight = front;
                }
            }
            else
            {
                if (horizontal)
                {
                    bufferF.sizeDelta = Vector2.right * front;
                    bufferB.sizeDelta = Vector2.right * back;
                }
                else
                {
                    bufferF.sizeDelta = Vector2.up * front;
                    bufferB.sizeDelta = Vector2.up * back;
                }
            }

            bufferB.SetAsFirstSibling();
            bufferF.SetAsLastSibling();
        }
    }
    #endif
}
