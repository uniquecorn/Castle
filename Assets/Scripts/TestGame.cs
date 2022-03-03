using System;
using Castle.CastleShapes;
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
    [Vector3Range(-5,5,-5,5,-5,5)]
    public Vector3 offset;
    [Range(2,60)]
    public int circleResolution = 3;
    [Range(1,200)]
    public int circleRadius;
    [Range(1,200)]
    public int circleRadiusY;

    [Button]
    
    void Test()
    {
        Debug.Log(timeRange.Check(System.DateTime.Now));
        
    }

    private void OnDrawGizmos()
    {
        // Ellipse.Draw(Vector3.zero + offset,circleRadius ,circleRadiusY, circleResolution);
        
        // Circle.Draw(offset,100,100,5,750);
        

    }
}

