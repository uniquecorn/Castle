namespace Castle
{
    using TMPro;
	using System.Collections.Generic;
	using UnityEngine;
    using NaughtyAttributes;
    [ExecuteInEditMode]
    [RequireComponent(typeof(TextMeshProUGUI))]
	public class CastleText : MonoBehaviour
	{
        private TextMeshProUGUI textComponent;
        public TextMeshProUGUI TextComponent
        {
            get
            {
                if(!textComponent)
                {
                    textComponent = GetComponent<TextMeshProUGUI>();
                }
                return textComponent;
            }
        }
		private CharacterData[] charData;
		[OnValueChanged("SyncToTextMesh"),Range(0, 1)]
		public float progress;
		public float delay; 
		public float duration = 1;
		private float realAnimationTime;
		private float internalTime;
		public bool playOnAwake;
        public LoopMode loop;
        [ShowIf("IsPlayForever")]
        public bool prewarm;
		private bool isPlaying;
		public bool unscaledTime;

        public enum LoopMode
        {
            STOP,
            LOOP,
            PLAYFOREVER
        }

		public TextModifier[] modifiers;
        private void Reset()
        {
            SyncToTextMesh();
        }
        // Update is called once per frame
        public void SyncToTextMesh()
        {
            TextComponent.ForceMeshUpdate();
            charData = new CharacterData[TextComponent.text.Length];
            realAnimationTime = duration + (charData.Length * delay);
            int charCount = 0;
            for (int meshNum = 0; meshNum < TextComponent.textInfo.meshInfo.Length; meshNum++)
            {
                int numOfMeshCharacters = 0;
                for (int characterNum = 0; characterNum < charData.Length * 4; characterNum += 4, numOfMeshCharacters++,++charCount)
                {
                    VertexPos vertPos = new VertexPos(TextComponent.textInfo.meshInfo[meshNum].vertices[characterNum], TextComponent.textInfo.meshInfo[meshNum].vertices[characterNum + 1], TextComponent.textInfo.meshInfo[meshNum].vertices[characterNum + 2], TextComponent.textInfo.meshInfo[meshNum].vertices[characterNum + 3]);
                    charData[charCount] = new CharacterData(delay * charCount, duration, charCount, vertPos)
                    {
                        charUsed = TextComponent.text[charCount].ToString()
                    };
                }
            }
        }
        bool IsPlayForever()
        {
            if(loop == LoopMode.PLAYFOREVER)
            {
                return true;
            }
            return false;
        }
        void Start()
		{
            SyncToTextMesh();
			if(playOnAwake)
			{
                Play();
			}
		}
        [Button]
        public void Play()
        {
            isPlaying = true;
            internalTime = progress = 0;
            if(prewarm && IsPlayForever())
            {
                progress = 1;
                internalTime = realAnimationTime;
            }
        }
        [Button]
        public void Pause()
        {
            isPlaying = false;
        }
        [Button]
        public void Stop()
        {
            isPlaying = false;
            internalTime = progress = 0;
        }
		void UpdateTime()
		{
			if (!isPlaying)
			{
				internalTime = progress * realAnimationTime;
			}
			else
			{
				progress = internalTime / realAnimationTime;
				if(unscaledTime)
				{
					internalTime += Time.unscaledDeltaTime;
				}
				else
				{
					internalTime += Time.deltaTime;
				}
				if(internalTime >= realAnimationTime)
				{
                    switch(loop)
                    {
                        case LoopMode.STOP:
                            isPlaying = false;
                            internalTime = realAnimationTime;
                            progress = 1;
                            break;
                        case LoopMode.LOOP:
                            internalTime = 0;
                            progress = 0;
                            break;
                    }
				}
			}
			for (int i = 0; i < charData.Length; i++)
			{
				charData[i].UpdateTime(internalTime);
			}
		}

        void Animate()
        {
            for (int j = 0; j < charData.Length; j++)
            {
                charData[j].vertexPos.ResetModified();
            }
            if (modifiers != null)
            {
                for (int i = 0; i < modifiers.Length; i++)
                {
                    for(int j = 0; j < charData.Length; j++)
                    {
                        modifiers[i].Apply(charData[j]);
                    }
                }
            }
            int charCount = 0;
            for (int meshNum = 0; meshNum < TextComponent.textInfo.meshInfo.Length; meshNum++)
            {
                int numOfMeshCharacters = 0;
                for (int characterNum = 0; characterNum < charData.Length * 4; characterNum += 4, numOfMeshCharacters++, ++charCount)
                {
                    for(int vertIndex = 0; vertIndex < 4; vertIndex++)
                    {
                        TextComponent.textInfo.meshInfo[meshNum].vertices[characterNum + vertIndex] = charData[charCount].vertexPos.modifiedPositions[vertIndex];
                    }
                }
            }
            TextComponent.UpdateVertexData();
        }
		void Update()
		{
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying)
            {
                modifiers = GetComponents<TextModifier>();
                if ((progress * realAnimationTime) != internalTime)
                {
                    internalTime = progress * realAnimationTime;
                    for (int j = 0; j < charData.Length; j++)
                    {
                        charData[j].UpdateTime(internalTime);
                    }
                    Animate();
                }
            }
#endif
            if(Application.isPlaying)
            {
                UpdateTime();
                if (modifiers.Length > 0)
                {
                    Animate();
                }
            }
		}
	}
}