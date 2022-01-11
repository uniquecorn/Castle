using Castle.Core.TimeTools;
using Castle.Core.UI;
using Sirenix.OdinInspector;
using UnityEngine;

public class TestGame : MonoBehaviour
{
    public SimpleDate date;
    public SimpleTime time;
    public Sprite[] sprites;
    public TimeRange timeRange;
    public DateRange dateRange;
    [Button]
    void Test()
    {
        Debug.Log(timeRange.Check(System.DateTime.Now));
    }
}
