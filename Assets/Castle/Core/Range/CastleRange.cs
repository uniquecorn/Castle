using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Castle.Core.Range
{
    [Serializable]
    public abstract class CastleRange : ISimpleCastleRange
    {
        public abstract string Label { get; }
        [Title("$Label"), ShowInInspector, ShowIf("UseRangeEnum"), PropertyOrder(-3), HideReferenceObjectPicker,
         InlineButton("DebugCheck", ShowIf = "InPlayMode")]
        protected virtual Enum RangeType
        {
            get => null;
            set => _ = value;
        }
        public bool InPlayMode => Application.isPlaying;
        private bool UseRangeEnum => RangeType != null;
        public virtual bool Check() => false;
        
    }

    public abstract class CastleValueRange : CastleRange
    {
        public enum ConditionCheck
        {
            MoreOrEqual,
            Equal,
            Less
        }

        public enum ValueTypeCheck
        {
            None,
            Simple,
            SimpleValue,
            Value
        }
        [ShowIf("UseValue",ValueTypeCheck.Value)]
        public ConditionCheck checkType;
        public int value;
        protected virtual ValueTypeCheck UseValue => ValueTypeCheck.None;
    }
    public interface ISimpleCastleRange : ICastleRange
    {
        bool Check();
        void DebugCheck() => Debug.Log(Label +" is "+Check());
    }

    public interface IConditionalCastleRange<in T> : ICastleRange
    {
        bool Check(T variable);
    }

    public interface ICastleRange
    {
        string Label { get; }
        ConditionOp ArrayCheck => ConditionOp.And;
    }
    public enum ConditionOp
    {
        And,
        Or
    }
}