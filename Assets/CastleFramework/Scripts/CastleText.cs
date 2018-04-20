namespace Castle
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	[ExecuteInEditMode]
	public class CastleText : CastleObject
	{
		[TextArea(3, 10)]
		public string text;
		private string internalText;
		public int fontSize;
		private int internalFontSize;
		public float scale;
		private float internalScale;

		public Font font;

		public Color textColor;
		private Color internalColor;
		
		public enum Alignment
		{
			LEFT,
			CENTER,
			RIGHT
		}
		public Alignment alignment;
		private Alignment internalAlignment;

		[Range(0,1)]
		public float progress;
		Mesh mesh;
		MeshFilter meshFilter;
		MeshRenderer meshRenderer;

		Vector3[] vertices;
		int[] triangles;
		Vector2[] uv;
		Color[] colors;
		Vector3 caretPos;
		int caretLine;
		List<float> lineLengths;
		// Update is called once per frame
		protected override void Start()
		{
			mesh = new Mesh();
			meshFilter = GetComponent<MeshFilter>();
			meshFilter.mesh = mesh;
			meshRenderer = GetComponent<MeshRenderer>();
			meshRenderer.material = font.material;
			RebuildMesh();
		}

		void OnFontTextureRebuilt(Font changedFont)
		{
			if (changedFont != font)
				return;

			RebuildMesh();
		}

		void RebuildMesh()
		{
			internalText = text;
			internalScale = scale;
			internalAlignment = alignment;
			internalFontSize = fontSize;
			internalColor = textColor;
			font.RequestCharactersInTexture(text, internalFontSize);
			
			mesh.MarkDynamic();
			lineLengths = new List<float>()
			{
				0
			};
			vertices = new Vector3[internalText.Length * 4];
			triangles = new int[internalText.Length * 6];
			uv = new Vector2[internalText.Length * 4];
			colors = new Color[internalText.Length * 4];
			caretPos = Vector3.zero;
			caretLine = 0;
			for (int i = 0; i < internalText.Length; i++)
			{
				AddChar(internalText[i],i);
			}
			Align();
			
			mesh.Clear();
			mesh.vertices = vertices;
			mesh.triangles = triangles;
			mesh.uv = uv;
			mesh.colors = colors;
		}

		void AddChar(char _char, int characterPosition)
		{
			CharacterInfo ch;
			font.GetCharacterInfo(_char, out ch, internalFontSize);

			if (_char == '\n')
			{
				NewLine();
			}
			else
			{
				int vertexPos = 4 * characterPosition;
				int triPos = 6 * characterPosition;
				vertices[vertexPos] = (caretPos + new Vector3(ch.minX, ch.maxY, 0)) * internalScale;
				vertices[vertexPos + 1] = (caretPos + new Vector3(ch.maxX, ch.maxY, 0)) * internalScale;
				vertices[vertexPos + 2] = (caretPos + new Vector3(ch.maxX, ch.minY, 0)) * internalScale;
				vertices[vertexPos + 3] = (caretPos + new Vector3(ch.minX, ch.minY, 0)) * internalScale;

				colors[vertexPos] =
					colors[vertexPos + 1] =
					colors[vertexPos + 2] =
					colors[vertexPos + 3] = internalColor;

				uv[vertexPos] = ch.uvTopLeft;
				uv[vertexPos + 1] = ch.uvTopRight;
				uv[vertexPos + 2] = ch.uvBottomRight;
				uv[vertexPos + 3] = ch.uvBottomLeft;

				triangles[triPos] = vertexPos;
				triangles[triPos + 1] = vertexPos + 1;
				triangles[triPos + 2] = vertexPos + 2;

				triangles[triPos + 3] = vertexPos;
				triangles[triPos + 4] = vertexPos + 2;
				triangles[triPos + 5] = vertexPos + 3;
			}
			lineLengths[caretLine] += ch.advance;
			caretPos += (Vector3.right * ch.advance);
		}

		void NewLine()
		{
			caretLine++;
			lineLengths.Add(0);
			caretPos = new Vector3(0, -fontSize * caretLine, 0);
		}

		void SetColors()
		{
			internalColor = textColor;
			for(int i = 0; i < colors.Length; i++)
			{
				colors[i] = internalColor;
			}
			mesh.colors = colors;
		}

		void Align()
		{
			int currentLine = 0;
			for (int i = 0; i < internalText.Length; i++)
			{
				if (internalText[i] == '\n')
				{
					currentLine++;
				}
				else
				{
					Vector3 alignedVec = Vector3.zero;
					switch (internalAlignment)
					{
						case Alignment.CENTER:
							alignedVec = Vector3.right * ((lineLengths[currentLine] * internalScale) / 2);
							break;
						case Alignment.RIGHT:
							alignedVec = Vector3.right * (lineLengths[currentLine] * internalScale);
							break;
					}
					vertices[4 * i] -= alignedVec;
					vertices[(4 * i) + 1] -= alignedVec;
					vertices[(4 * i) + 2] -= alignedVec;
					vertices[(4 * i) + 3] -= alignedVec;
				}
			}
		}

		void Update()
		{
			if(text != internalText || alignment != internalAlignment || scale != internalScale || fontSize != internalFontSize)
			{
				RebuildMesh();
			}
			if(textColor != internalColor)
			{
				SetColors();
			}
		}
	}
}