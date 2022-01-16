using Castle.Tools;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode,RequireComponent(typeof(CanvasRenderer))]
public class UICircle<T> : MaskableGraphic // Changed to maskableGraphic so it can be masked with RectMask2D
{
    [SerializeField]
    Texture m_Texture;

    
    [Range(0, 1)]
    [SerializeField]
    private float fillAmount = 1;

    public float FillAmount
    {
        get { return fillAmount; }
        set
        {
            fillAmount = value;
            SetVerticesDirty();
        }
    }

    public bool fill = true;
    public int thickness = 5;

    [Range(0, 60)]
    public int segments = 60;

    [Range(0, 60)]
    public int step = 60;

    public override Texture mainTexture
    {
        get
        {
            return m_Texture == null ? s_WhiteTexture : m_Texture;
        }
    }

    /// <summary>
    /// Texture to be used.
    /// </summary>
    public Texture texture
    {
        get { return m_Texture; }

        set
        {
            if (m_Texture == value)
                return;
            m_Texture = value;
            SetVerticesDirty();
            SetMaterialDirty();
        }
    }
    public bool invert;
    [SerializeField]
    private float radius = 30;
    public float Radius
    {
        get
        {
            return radius;
        }
        set
        {
            radius = value;
            SetVerticesDirty();
        }
    }
    public Vector2 offset;

    // Using arrays is a bit more efficient
    UIVertex[] uiVertices = new UIVertex[4];
    Vector2[] uvs = new Vector2[4];
    Vector2[] pos = new Vector2[4];

    protected override void Start()
    {
        uvs[0] = new Vector2(0, 1);
        uvs[1] = new Vector2(1, 1);
        uvs[2] = new Vector2(1, 0);
        uvs[3] = new Vector2(0, 0);
    }

    // Updated OnPopulateMesh to user VertexHelper instead of mesh
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        var pivot = rectTransform.pivot;
        var outer = -pivot.x * radius;
        var inner = -pivot.x * radius - thickness;
        var corners = new Vector2[4];
        var rect = rectTransform.rect;
        corners[0] = new Vector2(rect.xMin, rect.yMin);
        corners[1] = new Vector2(rect.xMax, rect.yMin);
        corners[2] = new Vector2(rect.xMin, rect.yMax);
        corners[3] = new Vector2(rect.xMax, rect.yMax);

        var degrees = 360f / segments;
        var fa = (int)((segments + 1) * this.fillAmount);

        // Updated to new vertexhelper
        vh.Clear();
        if (radius <= 0)
        {
            return;
        }
        // Changed initial values so the first polygon is correct when circle isn't filled
        var x = outer * Mathf.Cos(0);
        var y = outer * Mathf.Sin(0);
        var prevX = new Vector2(x, y) + offset;
        // Changed initial values so the first polygon is correct when circle isn't filled
        x = inner * Mathf.Cos(0);
        y = inner * Mathf.Sin(0);
        var prevY = new Vector2(x, y) + offset;

        var ifInvert = invert ? 3 : 0;
        for (var i = 0; i < fa - 1 + ifInvert; i++)
        {
            // Changed so there isn't a stray polygon at the beginning of the arc
            var rad = Mathf.Deg2Rad * ((i + 1) * degrees);
            var c = Mathf.Cos(rad);
            var s = Mathf.Sin(rad);
            //float x = outer * c;
            //float y = inner * c;
            if (!invert)
            {
                pos[0] = prevX;
                pos[1] = new Vector2(outer * c, outer * s) + offset;

                if (fill)
                {
                    pos[2] = offset;
                    pos[3] = offset;
                }
                else
                {
                    pos[2] = new Vector2(inner * c, inner * s) + offset;
                    pos[3] = prevY;
                }

                // Set values for uiVertices
                for (var j = 0; j < 4; j++)
                {
                    uiVertices[j].color = color;
                    uiVertices[j].position = pos[j];
                    uiVertices[j].uv0 = uvs[j];
                }

                // Get the current vertex count
                var vCount = vh.currentVertCount;

                // If filled, we only need to create one triangle
                vh.AddVert(uiVertices[0]);
                vh.AddVert(uiVertices[1]);
                vh.AddVert(uiVertices[2]);

                // Create triangle from added vertices
                vh.AddTriangle(vCount, vCount + 2, vCount + 1);

                // If not filled we need to add the 4th vertex and another triangle
                if (!fill)
                {
                    vh.AddVert(uiVertices[3]);
                    vh.AddTriangle(vCount, vCount + 3, vCount + 2);
                }

                prevX = pos[1];
                prevY = pos[2];

                // Removed so we can just use a single triangle when not filled
                //vh.AddUIVertexQuad(SetVbo(new[] { pos0, pos1, pos2, pos3 }, new[] { uv0, uv1, uv2, uv3 }));
            }
            else
            {
                if (i /*SEGMENTS*/ < step)
                {
                    pos[0] = prevX;
                    pos[1] = new Vector2(outer * c, outer * s);
                    var distVec = prevX - pos[1];
                    if (pos[0].x < rectTransform.rect.xMin)
                    {
                        if (distVec.x != 0)
                        {
                            distVec = distVec / distVec.x;
                        }
                        pos[0] = pos[1] + distVec * (rectTransform.rect.xMin - pos[1].x);
                    }
                    else if (pos[0].x > rectTransform.rect.xMax)
                    {
                        if (distVec.x != 0)
                        {
                            distVec = distVec / distVec.x;
                        }
                        pos[0] = pos[1] + distVec * (rectTransform.rect.xMax - pos[1].x);
                    }
                    else if (pos[0].y < rectTransform.rect.yMin)
                    {
                        if (distVec.y != 0)
                        {
                            distVec = distVec / distVec.y;
                        }
                        pos[0] = pos[1] + distVec * (rectTransform.rect.yMin - pos[1].y);
                    }
                    else if (pos[0].y > rectTransform.rect.yMax)
                    {
                        if (distVec.y != 0)
                        {
                            distVec = distVec / distVec.y;
                        }
                        pos[0] = pos[1] + distVec * (rectTransform.rect.yMax - pos[1].y);
                    }
                    distVec = pos[1] - prevX;
                    if (pos[1].x < rectTransform.rect.xMin)
                    {
                        if (distVec.x != 0)
                        {
                            distVec = distVec / distVec.x;
                        }
                        pos[1] = pos[0] + distVec * (rectTransform.rect.xMin - pos[0].x);
                    }
                    else if (pos[1].x > rectTransform.rect.xMax)
                    {
                        if (distVec.x != 0)
                        {
                            distVec = distVec / distVec.x;
                        }
                        pos[1] = pos[0] + distVec * (rectTransform.rect.xMax - pos[0].x);
                    }
                    else if (pos[1].y < rectTransform.rect.yMin)
                    {
                        if (distVec.y != 0)
                        {
                            distVec = distVec / distVec.y;
                        }
                        pos[1] = pos[0] + distVec * (rectTransform.rect.yMin - pos[0].y);
                    }
                    else if (pos[1].y > rectTransform.rect.yMax)
                    {
                        if (distVec.y != 0)
                        {
                            distVec = distVec / distVec.y;
                        }
                        pos[1] = pos[0] + distVec * (rectTransform.rect.yMax - pos[0].y);
                    }
                    if (VectorOutOfBounds(pos[0]) || VectorOutOfBounds(pos[1]))
                    {

                    }
                    else
                    {
                        pos[2] = CastleTools.CenterOfVectors(pos[0], pos[1]).ClosestVector(corners);
                        for (var j = 0; j < 3; j++)
                        {
                            uiVertices[j].color = color;
                            uiVertices[j].position = pos[j];
                            uiVertices[j].uv0 = uvs[j];
                            if (j < 2)
                            {
                                uiVertices[j].position += (Vector3)offset;
                            }
                        }
                        var vCount = vh.currentVertCount;
                        // If filled, we only need to create one triangle
                        vh.AddVert(uiVertices[0]);
                        vh.AddVert(uiVertices[1]);
                        vh.AddVert(uiVertices[2]);

                        // Create triangle from added vertices
                        vh.AddTriangle(vCount, vCount + 2, vCount + 1);
                        if (prevY != pos[2] && !VectorOutOfBounds(prevX))
                        {
                            pos[0] = prevY;
                            pos[1] = prevX;

                            for (var j = 0; j < 3; j++)
                            {
                                uiVertices[j].color = color;
                                uiVertices[j].position = pos[j];
                                uiVertices[j].uv0 = uvs[j];
                                if (j == 1 && i < (fa - 3))
                                {
                                    uiVertices[j].position += (Vector3)offset;
                                }
                            }
                            vCount = vh.currentVertCount;

                            // If filled, we only need to create one triangle
                            vh.AddVert(uiVertices[0]);
                            vh.AddVert(uiVertices[1]);
                            vh.AddVert(uiVertices[2]);

                            // Create triangle from added vertices
                            vh.AddTriangle(vCount, vCount + 2, vCount + 1);
                        }
                    }

                    prevX = new Vector2(outer * c, outer * s);
                    prevY = pos[2];
                }
            }
        }
    }
    bool VectorOutOfBounds(Vector2 vector)
    {
        if (vector.x < rectTransform.rect.xMin || vector.x > rectTransform.rect.xMax || vector.y < rectTransform.rect.yMin || vector.y > rectTransform.rect.yMax)
        {
            return true;
        }
        return false;
    }
}