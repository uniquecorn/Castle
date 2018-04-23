namespace Castle
{
	using UnityEngine;

	[System.Serializable]
	public class ScaleModifier : TextModifier
	{
		public override void Apply(CharacterData characterData)
		{
			for(int i = 0; i < 4; i++)
			{
				characterData.vertexPos.modifiedPositions[i] *= curve.Evaluate(characterData.Progress);
			}
		}
	}
}
