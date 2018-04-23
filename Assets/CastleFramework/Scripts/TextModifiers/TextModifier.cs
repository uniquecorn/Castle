namespace Castle
{
	using UnityEngine;

	[System.Serializable]
	public class TextModifier : MonoBehaviour
	{
		public AnimationCurve curve;

		public virtual void Apply(CharacterData characterData)
		{
			//APPLY CHANGES TO CHARACTERDATA HERE
		}
	}
}
