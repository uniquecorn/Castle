using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Castle.Core
{
    [Serializable,InlineProperty]
    public struct SimpleDate : IComparable<SimpleDate>, IEquatable<SimpleDate>
    {
        [Range(1, 366),HideInInspector] public int days;
[ShowInInspector,ValueDropdown("GetDays"),HorizontalGroup,HideLabel]
        public int Day
        {
            get
            {
                for (var i = 0; i < TotalDaysInMonth.Length; i++)
                {
                    if(days <= TotalDaysInMonth[i])
                    {
                        if (i == 0)
                        {
                            return days;
                        }
                        return days - TotalDaysInMonth[i-1];
                    }
                }
                return 1;
            }
            set
            {
                for (var i = 0; i < TotalDaysInMonth.Length; i++)
                {
                    if (days <= TotalDaysInMonth[i])
                    {
                        if (i == 0)
                        {
                            days = value;
                            return;
                        }
                        days = TotalDaysInMonth[i-1] + value;
                        return;
                    }
                }
            }
        }
        [ShowInInspector,ValueDropdown("GetMonths"),HorizontalGroup,HideLabel]
        public int Month
        {
            get
            {
                for (var i = 0; i < TotalDaysInMonth.Length; i++)
                {
                    if (days <= TotalDaysInMonth[i])
                    {
                        return i+1;
                    }
                }
                return 1;
            }
            set
            {
                for (var i = 0; i < TotalDaysInMonth.Length; i++)
                {
                    if (days <= TotalDaysInMonth[i])
                    {
                        if (i > 0)
                        {
                            days -= TotalDaysInMonth[i - 1];
                        }
                        break;
                    }
                }
                if (value > 1)
                {
                    days += TotalDaysInMonth[value - 2];
                }
            }
        }
        public int CompareTo(SimpleDate other) => days.CompareTo(other.days);
        public static bool operator >  (SimpleDate operand1, SimpleDate operand2) => operand1.CompareTo(operand2) == 1;
        public static bool operator <  (SimpleDate operand1, SimpleDate operand2) => operand1.CompareTo(operand2) == -1;
        public static bool operator >= (SimpleDate operand1, SimpleDate operand2) => operand1.CompareTo(operand2) >= 0;
        public static bool operator <=  (SimpleDate operand1, SimpleDate operand2) => operand1.CompareTo(operand2) <= 0;
        public bool Equals(SimpleDate other) => days == other.days;
        public override bool Equals(object obj) =>obj is SimpleDate other && Equals(other);
        public override int GetHashCode() => days;
        public override string ToString() => Day + " " + Month;

        public SimpleDate(DateTime dateTime)
        {
            if (dateTime.Month == 1)
            {
                days = dateTime.Day;
            }
            else
            {
                days = TotalDaysInMonth[dateTime.Month-1] + dateTime.Day;
            }
        }
        public ValueDropdownList<int> GetDays()
        {
            var dropdown = new ValueDropdownList<int>();
            var monthDays = DaysInMonth[Month - 1];
            for (var i = 0; i < monthDays; i++)
            {
                dropdown.Add(i+1);
            }
            return dropdown;
        }
        
        public static readonly ValueDropdownList<int> GetMonths = new() {
            {"Jan", 1},
            {"Feb", 2},
            {"Mar", 3},
            {"Apr", 4},
            {"May", 5},
            {"Jun", 6},
            {"Jul", 7},
            {"Aug", 8},
            {"Sep", 9},
            {"Oct", 10},
            {"Nov", 11},
            {"Dec", 12}
        };
        public static readonly int[] DaysInMonth =      {31, 29, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31};
        public static readonly int[] TotalDaysInMonth = {31, 60, 91, 121, 152, 182, 213, 244, 274, 305, 335, 366};
    }
}