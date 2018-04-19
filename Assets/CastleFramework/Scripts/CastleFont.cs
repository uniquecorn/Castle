using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastleFont
{
	public Glyph[] glyphs;
	public Texture2D sprite_texture;

	public bool loaded;

	[System.Serializable]
	public class Glyph
	{
		public Sprite sprite;
		public Rect uv;
		public Rect vert;
		public float width;
		public int index;
	}

	public void LoadFont(Font _font)
	{
		sprite_texture = (Texture2D)_font.material.mainTexture;
		int size = _font.characterInfo.Length;
		glyphs = new Glyph[size];
		for(int i = 0; i < size; ++i)
		{
			glyphs[i] = new Glyph()
			{
				sprite = Sprite.Create(sprite_texture, new Rect(_font.characterInfo[i].minX, _font.characterInfo[i].minY, _font.characterInfo[i].glyphWidth, _font.characterInfo[i].glyphHeight),Vector2.zero),
				width = _font.characterInfo[i].glyphWidth,
				vert = new Rect(_font.characterInfo[i].minX, _font.characterInfo[i].minY, _font.characterInfo[i].maxX, _font.characterInfo[i].maxY)
			};
		}
		loaded = true;
	}
}
