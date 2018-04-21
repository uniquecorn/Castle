namespace Castle
{
	using UnityEngine;

	[System.Serializable]
	public class TextModifier : MonoBehaviour
	{
		public AnimationCurve curve;
		[HideInInspector]
		public Vector3[] vertices;

		public virtual void Apply(TextAnimator.CharacterData characterData, ref Mesh characterMesh)
		{

		}
	}
}
