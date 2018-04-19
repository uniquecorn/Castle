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
			string _path = Tools.FindNonConflictingDir(basePath, ".controller");

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
		// Update is called once per frame
		void Update()
		{

		}
	}
}
