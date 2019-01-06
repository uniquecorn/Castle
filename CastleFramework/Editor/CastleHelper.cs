namespace Castle
{
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using UnityEngine;
	using UnityEditor;
	using UnityEditor.Animations;

	public class CastleHelper : Editor
	{
		[MenuItem("Castle/Create/CastleButton")]
		static void CreateButton()
		{
			Object prefabObject = AssetDatabase.LoadAssetAtPath<Object>("Assets/CastleFramework/Primitives/CastleButton.prefab");
			CastleButton castleButton = ((GameObject)PrefabUtility.InstantiatePrefab(prefabObject)).GetComponent<CastleButton>();
			string basePath = "Assets/Animations/Buttons/Button";
			string _path = CastleTools.FindNonConflictingDir(basePath, ".controller");

			AnimatorController control = AnimatorController.CreateAnimatorControllerAtPath(_path);
			AnimatorState spawn = AddState(control, "Spawn", false, false, true);
			AnimatorState normal = AddState(control, "Normal", true);
			AddExit(spawn, normal);
			AnimatorState enterHover = AddState(control, "EnterHover", false);
			AddParam(control, "EnterHover");
			AddTransition(normal, enterHover, "ExitHover");
			AnimatorState hover = AddState(control, "Hover");
			AddExit(enterHover, hover);
			AnimatorState exitHover = AddState(control, "ExitHover", false);
			AddParam(control,"ExitHover");
			AddTransition(hover, exitHover, "ExitHover");
			AddExit(exitHover, normal);
			AnimatorState tap = AddState(control, "Tap", false, true);
			AnimatorState hold = AddState(control, "Hold");
			AnimatorState release = AddState(control, "Release", false, true);
			AddExit(tap, hold);
			AddExit(release, normal);
			castleButton.anim.runtimeAnimatorController = control;
			Selection.activeGameObject = castleButton.gameObject;
		}

		[MenuItem("Castle/Create/CastleText")]
		static void CreateText()
		{
			Object prefabObject = AssetDatabase.LoadAssetAtPath<Object>("Assets/CastleFramework/Primitives/CastleText.prefab");
			CastleText castleText = ((GameObject)PrefabUtility.InstantiatePrefab(prefabObject)).GetComponent<CastleText>();
			Selection.activeGameObject = castleText.gameObject;
		}

		static void AddExit(AnimatorState start, AnimatorState end, float _exitTime = 1, float _duration = 0)
		{
			AnimatorStateTransition tempTransition = start.AddExitTransition();
			tempTransition.destinationState = end;
			tempTransition.hasExitTime = true;
			tempTransition.exitTime = _exitTime;
			tempTransition.duration = _duration;
		}

		static void AddTransition(AnimatorState start, AnimatorState end, string condition, float _duration = 0)
		{
			AnimatorStateTransition tempTransition = start.AddExitTransition();
			tempTransition.destinationState = end;
			tempTransition.hasExitTime = false;
			tempTransition.AddCondition(AnimatorConditionMode.If, 0, condition);
			tempTransition.duration = _duration;
		}
		
		static AnimatorState AddState(AnimatorController control, string clipName, bool loop = false, bool addParam = false, bool defaultState = false)
		{
			AnimatorStateMachine rootStateMachine = control.layers[0].stateMachine;
			AnimatorState tempState = rootStateMachine.AddState(clipName);
			AnimationClip tempClip = new AnimationClip()
			{
				name = clipName
			};
			tempState.motion = tempClip;
			if (defaultState)
			{
				rootStateMachine.defaultState = tempState;
			}
			if (addParam)
			{
				AddParam(control, clipName);
				AnimatorStateTransition tempTransition = rootStateMachine.AddAnyStateTransition(tempState);
				tempTransition.AddCondition(AnimatorConditionMode.If, 0, clipName);
				tempTransition.duration = 0;
			}
			AssetDatabase.AddObjectToAsset(tempClip, control);
			AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(tempClip));
			return tempState;
		}

		static void AddParam(AnimatorController control, string clipName)
		{
			control.AddParameter(clipName, AnimatorControllerParameterType.Trigger);
		}
		//[MenuItem("CONTEXT/Font/Create CastleFont")]
		//static CastleFont CreateCastleFont(MenuCommand command)
		//{
		//	Font font = (Font)command.context;
		//	CastleFont castleFont = CreateInstance<CastleFont>();
		//	AssetDatabase.CreateAsset(castleFont, "Assets/Fonts/" + font.name + ".asset");
		//	int size = font.characterInfo.Length;
		//	castleFont.glyphs = new CastleFont.Glyph[size];
		//	for(int i = 0; i < size; i++)
		//	{
		//		Rect _uv = new Rect(font.characterInfo[i].uvTopLeft.x, font.characterInfo[i].uvTopLeft.y, font.characterInfo[i].glyphWidth, font.characterInfo[i].glyphHeight);
		//		CastleFont.Glyph g = new CastleFont.Glyph();
		//		//g.uv = font.characterInfo[i]
		//		g.index = font.characterInfo[i].index;
		//	}
		//	string path = AssetDatabase.GetAssetPath(font);
		//	Texture2D tex = AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D)) as Texture2D;
		//	castleFont.sprite_texture = tex;
		//	EditorUtility.SetDirty(castleFont);
		//	return castleFont;
		//}
	}
}
