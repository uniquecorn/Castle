using System.Collections.Generic;
using UnityEngine;

namespace Castle.Core.UI
{
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
}