namespace Castle
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public class CastleText : CastleObject
	{
		public string text;
		public Font font;
		private CastleFont castleFont;
		public int fontSize;
		List<SpriteRenderer> internalSprites;
		public GameObject glyph;
		// Update is called once per frame
		void Awake()
		{
			if(castleFont == null)
			{
				castleFont = new CastleFont();
				castleFont.LoadFont(font);
				for(int i = 0; i < castleFont.glyphs.Length;i++)
				{
					CreateGlyph(i).sprite = castleFont.glyphs[i].sprite;
				}
				//testRender.sprite = castleFont.glyphs[5].sprite;
			}
		}

		SpriteRenderer CreateGlyph(int x)
		{
			return Instantiate(glyph, transform.position + Vector3.right * x, Quaternion.identity).GetComponent<SpriteRenderer>();
		}
		void Update()
		{
			//font.RequestCharactersInTexture()
			//tm.
		}
	}
}