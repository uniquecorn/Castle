using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Castle.Core
{
    [Serializable,InlineProperty,HideLabel]
    public struct TimeRange : IConditionalCastleRange<DateTime>
    {
        [FoldoutGroup("$Label")]
        public SimpleTime from, to;
        [ShowInInspector,MinMaxSlider(0, 1439),HideIf("ReverseRange"),HideLabel]
        public Vector2Int Range
        {
            get => new(from.minutes, to.minutes);
            set => (from.minutes, to.minutes) = (value.x, value.y);
        }
        [ShowInInspector, MinMaxSlider(0, 1439), ShowIf("ReverseRange"), HideLabel]
        public Vector2Int OverlappedRange
        {
           get => new(from.minutes, 1439);
           set => from.minutes = value.x;
        }
        [ShowInInspector,MinMaxSlider(0, 1439),ShowIf("ReverseRange"),HideLabel]
        public Vector2Int UnderlappedRange 
        { 
            get => new (0, to.minutes);
            set => to.minutes = value.y;
        }
        public bool Check(DateTime variable)
        {
            var dateTime = new SimpleTime(variable);
            if (from < to)
            {
                return dateTime >= from && dateTime < to;
            }
            if (from == to)
            {
                return dateTime == from;
            }
            return dateTime < to || dateTime >= from;
        }
        public bool ReverseRange => to < from;
        public string Label => from + " - " + to;
    }
}