using System;
using Castle.Core.TimeTools;
using Castle.Core.UI;
using Castle.Shapes;
using Sirenix.OdinInspector;
using UnityEngine;

public class TestGame : MonoBehaviour
{
    public SimpleDate date;
    public SimpleTime time;
    public Sprite[] sprites;
    public TimeRange timeRange;
    public DateRange dateRange;
    [Range(2,60)]
    public int circleResolution = 2;
    [Range(1,200)]
    public int circleRadius;
    [Button]
    void Test()
    {
        Debug.Log(timeRange.Check(System.DateTime.Now));
        
    }

    private void OnDrawGizmos()
    {
        Circle.Draw(Vector3.zero,circleRadius ,circleResolution);
    }
}
