using System;
using Castle.Core.Range;
using Sirenix.OdinInspector;

namespace Castle.Core.TimeTools
{
    [Serializable,InlineProperty]
    public struct DateRange : IConditionalCastleRange<DateTime>
    {
        [HorizontalGroup,HideLabel]
        public SimpleDate from, to;
        
        public bool Check(DateTime variable)
        {
            return false;
        }

        public string Label { get; }
    }
}