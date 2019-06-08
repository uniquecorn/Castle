namespace Castle
{
	using UnityEngine;

	[System.Serializable]
	public class ScaleModifier : TextModifier
	{
        public enum ScaleMode
        {
            XY,
            X,
            Y
        }
        public ScaleMode scaleMode;
		public override void Apply(CharacterData characterData)
		{
			for(int i = 0; i < 4; i++)
			{
                Vector3 direction = characterData.vertexPos.modifiedPositions[i] - characterData.vertexPos.middlePos;
                direction.Normalize();
                Vector3 origin = characterData.vertexPos.middlePos;
                if (scaleMode == ScaleMode.X)
                {
                    direction = new Vector3(direction.x, 0, direction.z);
                    origin = new Vector3(origin.x, characterData.vertexPos.modifiedPositions[i].y, origin.z);
                }
                else if(scaleMode == ScaleMode.Y)
                {
                    direction = new Vector3(0, direction.y, direction.z);
                    origin = new Vector3(characterData.vertexPos.modifiedPositions[i].x, origin.y, origin.z);
                }
                characterData.vertexPos.modifiedPositions[i] = origin + direction * curve.Evaluate(characterData.Progress) * Vector3.Distance(characterData.vertexPos.basePositions[i], characterData.vertexPos.middlePos);
            }
		}
	}
}
