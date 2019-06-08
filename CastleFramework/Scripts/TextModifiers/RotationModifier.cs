namespace Castle
{
	using UnityEngine;

	[System.Serializable]
	public class RotationModifier : TextModifier
	{
		public override void Apply(CharacterData characterData)
		{
			for (int i = 0; i < 4; i++)
			{
				characterData.vertexPos.modifiedPositions[i] = CastleTools.RotatePointAroundPivot(characterData.vertexPos.modifiedPositions[i], characterData.vertexPos.middlePos, Quaternion.Euler(0, 0, curve.Evaluate(characterData.Progress)));
			}
		}
	}
}
