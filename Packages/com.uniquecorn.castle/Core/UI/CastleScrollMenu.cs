using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Castle.Core.UI
{
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
        protected virtual bool UseLayoutElement => (horizontal && layoutGroup.childControlWidth) ||
                                                   (!horizontal && layoutGroup.childControlHeight);
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
}