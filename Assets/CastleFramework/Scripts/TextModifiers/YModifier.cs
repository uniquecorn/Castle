namespace Castle
{
	using UnityEngine;

	[System.Serializable]
	public class YModifier : TextModifier
	{
		public override void Apply(CharacterData characterData)
		{
			for(int i = 0; i < 4; i++)
			{
				characterData.vertexPos.modifiedPositions[i] += (Vector3.up * curve.Evaluate(characterData.Progress));
			}
		}
	}
}
