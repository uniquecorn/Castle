namespace Castle
{
	using UnityEngine;

	[System.Serializable]
	public class RotationModifier : TextModifier
	{
		public override void Apply(CharacterData characterData)
		{
			Vector3 pivot = Tools.CenterOfVectors(characterData.vertexPos.modifiedPositions);
			for (int i = 0; i < 4; i++)
			{
				characterData.vertexPos.modifiedPositions[i] = Tools.RotatePointAroundPivot(characterData.vertexPos.modifiedPositions[i], pivot, Quaternion.Euler(0, 0, curve.Evaluate(characterData.Progress)));
			}
		}
		
	}
}
