namespace Castle
{
	using UnityEngine;

	[System.Serializable]
	public class YModifier : TextModifier
	{
		public virtual void Apply(TextAnimator.CharacterData characterData, TextAnimator animator)
		{
			vertices = new Vector3[4];
			int index = 4 * characterData.Order;
			//vertices[4 * i] = animator.castleText.vertices[4 * i] + (Vector3.up * value);
			//		vertices[4 * i + 1] = animator.castleText.vertices[4 * i + 1] + (Vector3.up * value);
			//		vertices[4 * i + 2] = animator.castleText.vertices[4 * i + 2] + (Vector3.up * value);
			//		vertices[4 * i + 3] = animator.castleText.vertices[4 * i + 3] + (Vector3.up * value);
			for(int i = 0; i < 4; i++)
			{
				vertices[index + i] = animator.castleText.vertices[index + i] + (Vector3.up * curve.Evaluate(characterData.Progress));
			}
			
		}
		//public override void AffectMesh(TextAnimator animator)
		//{
		//	vertices = new Vector3[animator.castleText.vertices.Length];
		//	for(int i = 0; i < animator.charData.Length; i++)
		//	{
		//		animator.charData[i].UpdateTime(animator.internalTime);
		//		vertices[4 * i] = animator.castleText.vertices[4 * i] + (Vector3.up * value);
		//		vertices[4 * i + 1] = animator.castleText.vertices[4 * i + 1] + (Vector3.up * value);
		//		vertices[4 * i + 2] = animator.castleText.vertices[4 * i + 2] + (Vector3.up * value);
		//		vertices[4 * i + 3] = animator.castleText.vertices[4 * i + 3] + (Vector3.up * value);
		//	}
		//	animator.castleText.SetVertices(vertices);
		//}
	}
}
