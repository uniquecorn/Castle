using Castle.Tools;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode,RequireComponent(typeof(CanvasRenderer))]
public class ShapesUI : MaskableGraphic // Changed to maskableGraphic so it can be masked with RectMask2D
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

    [Range(3, 60)]
    public int segments = 60;

    [Range(3, 60)]
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
            SetMaterialDirty();
            SetVerticesDirty();
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