namespace Castle
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	[RequireComponent(typeof(CastleText))]
	public class TextAnimator : MonoBehaviour
	{
		public CastleText castleText;
		public TextModifier[] modifiers;

		public struct CharacterData
		{
			private float progress;

			public float Progress
			{
				get { return progress; }
			}

			private float startingTime;

			private float totalAnimationTime;
			private int order;

			public int Order
			{
				get { return order; }
			}

			public CharacterData(float startTime, float targetAnimationTime, int targetOrder)
			{
				progress = 0.0f;
				startingTime = startTime;
				totalAnimationTime = (startingTime + targetAnimationTime) - startTime;
				order = targetOrder;
			}

			public void UpdateTime(float time)
			{
				if (time < startingTime)
					return;

				progress = (time - startingTime) / totalAnimationTime;
			}
		}

		public CharacterData[] charData;

		[Range(0,1)]
		public float progress;
		public float delay;
		public float duration;
		private float realAnimationTime;
		[HideInInspector]
		public float internalTime;
		public bool playOnAwake;
		public bool loop;
		public bool isPlaying;
		public bool unscaledTime;
		// Use this for initialization
		void Awake()
		{
			if(playOnAwake)
			{
				Play();
			}
		}

		public void UpdateText()
		{
			CreateCharacterData();
			realAnimationTime = duration + (delay * charData.Length);
		}

		public void CreateCharacterData()
		{
			charData = new CharacterData[castleText.text.Length];
			for(int i = 0; i < charData.Length; i++)
			{
				charData[i] = new CharacterData(delay * i, duration, i);
			}
		}

		public void Play()
		{
			isPlaying = true;
		}

		void Animate(float _progress)
		{
			progress = _progress;
			for(int i = 0; i < modifiers.Length; i++)
			{
				//Apply modifier to characterData
				//modifiers[i].Apply(this);
			}
		}

		// Update is called once per frame
		void Update()
		{
			if(isPlaying)
			{
				Animate(internalTime / realAnimationTime);
				if (unscaledTime)
				{
					internalTime += Time.unscaledDeltaTime;
				}
				else
				{
					internalTime += Time.deltaTime;
				}
			}
		}
	}
}
