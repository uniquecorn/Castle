namespace Castle
{
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

		Mesh mesh;
		MeshFilter meshFilter;
		MeshRenderer meshRenderer;

		[HideInInspector]
		public Vector3[] vertices;
		[HideInInspector]
		public int[] triangles;
		[HideInInspector]
		public Vector2[] uv;
		[HideInInspector]
		public Color[] colors;
		
		public CharacterData[] charData;
		Vector3 caretPos;
		Vector3 alignedVec;
		int caretLine;
		List<float> lineLengths;

		[Range(0, 1)]
		public float progress;
		private float internalProgress;
		public float delay; 
		public float duration = 1;
		public float realAnimationTime;
		public float internalTime;
		public bool playOnAwake;
		public bool loop;
		public bool isPlaying;
		public bool unscaledTime;

		public TextModifier[] modifiers;
		// Update is called once per frame
		void Awake()
		{
			mesh = new Mesh();
			meshFilter = GetComponent<MeshFilter>();
			meshFilter.mesh = mesh;
			meshRenderer = GetComponent<MeshRenderer>();
			meshRenderer.material = font.material;
			lineLengths = new List<float>()
			{
				0
			};
			charData = new CharacterData[1];

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
			realAnimationTime = duration + (internalText.Length * delay);
			internalScale = scale;
			internalAlignment = alignment;
			internalFontSize = fontSize;
			internalColor = textColor;
			font.RequestCharactersInTexture(text, internalFontSize);
			//realAnimationTime = duration + (internalText.Length * delay);
			mesh.MarkDynamic();
			lineLengths.Clear();
			lineLengths.Add(0);
			charData = new CharacterData[internalText.Length];
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

			SetMesh();
		}

		void SetMesh()
		{
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
				charData[characterPosition] = new CharacterData(delay * characterPosition, duration, characterPosition, new VertexPos(Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero));
				charData[characterPosition].charType = CharacterData.CharacterType.NEWLINE;
				NewLine();
			}
			else
			{
				Vector3 pos1 = (caretPos + new Vector3(ch.minX, ch.maxY, 0)) * internalScale;
				Vector3 pos2 = (caretPos + new Vector3(ch.maxX, ch.maxY, 0)) * internalScale;
				Vector3 pos3 = (caretPos + new Vector3(ch.maxX, ch.minY, 0)) * internalScale;
				Vector3 pos4 = (caretPos + new Vector3(ch.minX, ch.minY, 0)) * internalScale;
				VertexPos vecPos = new VertexPos(pos1,pos2,pos3,pos4);
				charData[characterPosition] = new CharacterData(delay * characterPosition, duration, characterPosition,vecPos);
				int charPos = 4 * characterPosition;
				int triPos = 6 * characterPosition;
				SetVertices(charData[characterPosition]);

				colors[charPos] =
					colors[charPos + 1] =
					colors[charPos + 2] =
					colors[charPos + 3] = internalColor;

				uv[charPos] = ch.uvTopLeft;
				uv[charPos + 1] = ch.uvTopRight;
				uv[charPos + 2] = ch.uvBottomRight;
				uv[charPos + 3] = ch.uvBottomLeft;

				triangles[triPos] = charPos;
				triangles[triPos + 1] = charPos + 1;
				triangles[triPos + 2] = charPos + 2;

				triangles[triPos + 3] = charPos;
				triangles[triPos + 4] = charPos + 2;
				triangles[triPos + 5] = charPos + 3;
				
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
					switch (internalAlignment)
					{
						case Alignment.LEFT:
							alignedVec = Vector3.zero;
							break;
						case Alignment.CENTER:
							alignedVec = Vector3.right * ((lineLengths[currentLine] * internalScale) / 2);
							break;
						case Alignment.RIGHT:
							alignedVec = Vector3.right * (lineLengths[currentLine] * internalScale);
							break;
					}
					charData[i].vertexPos.MoveBaseVertex(-alignedVec);
					SetVertices(charData[i]);
				}
			}
		}

		public void SetVertices(CharacterData charData)
		{
			for(int i = 0; i < 4; i++)
			{
				vertices[(4 * charData.Order) + i] = charData.vertexPos.basePositions[i];
			}
		}

		public void AnimateVertices(CharacterData charData)
		{
			for (int i = 0; i < 4; i++)
			{
				vertices[(4 * charData.Order) + i] = charData.vertexPos.modifiedPositions[i];
			}
		}

		public void SetVertice(Vector3 vert, int index)
		{
			vertices[index] = vert;
			mesh.vertices = vertices;
		}

		void UpdateTime()
		{
			if (!isPlaying)
			{
				internalTime = progress * realAnimationTime;
			}
			else
			{
				progress = internalTime / realAnimationTime;
				if(unscaledTime)
				{
					internalTime += Time.unscaledDeltaTime;
				}
				else
				{
					internalTime += Time.deltaTime;
				}
				
			}
			for (int i = 0; i < charData.Length; i++)
			{
				charData[i].UpdateTime(internalTime);
			}
		}

		void Animate()
		{
			for(int i = 0; i < charData.Length; i++)
			{
				charData[i].vertexPos.ResetModified();
				for(int j = 0; j < modifiers.Length;j++)
				{
					modifiers[j].Apply(charData[i]);
				}
				AnimateVertices(charData[i]);
			}
			SetMesh();
		}

		void Update()
		{
#if UNITY_EDITOR
			if(!UnityEditor.EditorApplication.isPlaying)
			{
				modifiers = GetComponents<TextModifier>();
			}
#endif
			if (text != internalText || alignment != internalAlignment || scale != internalScale || fontSize != internalFontSize)
			{
				RebuildMesh();
			}
			if(textColor != internalColor)
			{
				SetColors();
			}
			UpdateTime();
			if(modifiers.Length > 0)
			{
				Animate();
			}
		}
	}
}