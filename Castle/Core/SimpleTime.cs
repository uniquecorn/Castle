using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Castle.Core
{
    [Serializable,InlineProperty,HideLabel]
    public struct SimpleTime : IComparable<SimpleTime>, IEquatable<SimpleTime>
    {
        private const string AMFormat = "{0}:{1}AM";
        private const string PMFormat = "{0}:{1}PM";
        [HideLabel,HorizontalGroup,LabelText("$ToString")]
        [Range(0,1439)]
        public int minutes;
        [ShowInInspector,HideLabel,HorizontalGroup(0.05f)]
        public int Hour
        {
            get => Mathf.FloorToInt((float) minutes / 60);
            set => minutes = Mathf.Clamp(Mathf.Clamp(value,0,23) * 60 + Minute,0,1439);
        }
        [ShowInInspector,HideLabel,HorizontalGroup(0.05f)]
        public int Minute
        {
            get => minutes % 60;
            set => minutes = Hour * 60 + Mathf.Clamp(value,0,59);
        }
        public SimpleTime(int hour, int minute) => minutes = hour * 60 + minute;
        public SimpleTime(DateTime dateTime) => minutes = Mathf.FloorToInt((float)dateTime.TimeOfDay.TotalMinutes);
        public int CompareTo(SimpleTime other) => minutes.CompareTo(other.minutes);
        public static bool operator >  (SimpleTime operand1, SimpleTime operand2) => operand1.CompareTo(operand2) == 1;
        public static bool operator <  (SimpleTime operand1, SimpleTime operand2) => operand1.CompareTo(operand2) == -1;
        public static bool operator >= (SimpleTime operand1, SimpleTime operand2) => operand1.CompareTo(operand2) >= 0;
        public static bool operator <=  (SimpleTime operand1, SimpleTime operand2) => operand1.CompareTo(operand2) <= 0;
        public static bool operator == (SimpleTime operand1, SimpleTime operand2) => operand1.Equals(operand2);
        public static bool operator !=(SimpleTime operand1, SimpleTime operand2) => !operand1.Equals(operand2);
        public bool Equals(SimpleTime other) => minutes == other.minutes;
        public override bool Equals(object obj) =>obj is SimpleTime other && Equals(other);
        public override int GetHashCode() => minutes;
        public static implicit operator string(SimpleTime s) => s.ToString();
        public override string ToString()
        {
            switch (minutes)
            {
                case < 60:
                case 1440:
                    return string.Format(AMFormat, 12, Minute.ToString("00"));
                case >= 720 and < 780:
                    return string.Format(PMFormat, 12, Minute.ToString("00"));
                case < 720:
                    return string.Format(AMFormat, Hour.ToString("00"), Minute.ToString("00"));
                default:
                    return string.Format(PMFormat, (Hour - 12).ToString("00"), Minute.ToString("00"));
            }
        }
    }
}