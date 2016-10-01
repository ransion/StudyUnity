//using UnityEngine;
//using UnityEditor;
//using UnityEditor.Animations;
//using System.Collections;
//using System.Collections.Generic;
//using System.IO;

//public class PTAnimatorFactrory : MonoBehaviour
//{
//	#region const
//	private const string BasicControllerPath = "Assets/Content/Character/Hero/AnimatorController/BasicController.controller";
//	private const string AnimRootPath = "Assets/Content/Character/Hero/";
//	private const string ControllerBasePath = "Assets/Content/Character/Hero/AnimatorController/";
//	private const string PrefabPath = "Prefabs/Character/Hero/";
//	private const string BasicPrefabPath = "Prefabs/Character/Hero/BasicHero";
//	#endregion const

//	#region private members
//	private static string curHeroName;
//	private static string animBasePath;
//	private static string skillAnimBasePath;
//	private static AnimatorController productController;
//	private static AnimatorStateMachine baseLayerMachine;
//	private static AnimatorStateMachine upperLayerMachine;

//	// Base Layer States
//	private static AnimatorState stateBaseIdleOrRun;
//	private static AnimatorState stateBaseDeath;
//	private static AnimatorState stateBaseKnockOut;
//	private static AnimatorState stateBaseLowerMove;
//	private static AnimatorState stateBaseTurnLeft;
//	private static AnimatorState stateBaseTurnRight;
//	// Upper Layer States
//	private static AnimatorState stateUpperIdleOrRun;
//	private static AnimatorState stateUpperAim;
//	private static AnimatorState stateUpperShoot;
//	private static AnimatorState stateUpperReload;
//	private static AnimatorState stateUpperDeath;
//	#endregion private members

//	#region public interfaces
//	/// <summary>
//	/// create hero animator controller
//	/// </summary>
//	/// <param name="characterName">hero code as name,e.g. 002</param>
//	/// <param name="skills">skill info export from skill editor</param>
//	public static void CreateController(string characterName, List<PTOneSkill> skills)
//	{
//		Debug.Log("CreateController to " + PTCharacterWindow.curCharacter);
//		// clear previous data
//		Clear();

//		// create basic controller first
//		CreateBasicController(PTCharacterWindow.curCharacter);

//		// add skill state to controller
//		AddSkillStates(skills);

//		// bind controller to prefab
//		BindControllerToPrefab();
//	}


//	#endregion public interfaces

//	#region private imp
//	/// <summary>
//	/// Create basic animator controller not include skill states
//	/// </summary>
//	/// <param name="name">hero code as name,e.g. 002</param>
//	/// <returns> controller result </returns>
//	private static void CreateBasicController(string name = null)
//	{
//		if (string.IsNullOrEmpty(name))
//		{
//			Debug.LogError("Hero name is null");
//			return;
//		}
//		CreateBaseTransitions();
//		CreateUpperTransitions();

//		return;
//	}

//	private static void CreateOneClipSkillState(string animationName, int skillDisplayNo)
//	{
//		var animClip = AssetDatabase.LoadAssetAtPath(skillAnimBasePath + animationName, typeof(AnimationClip)) as AnimationClip;
//		var animState = upperLayerMachine.AddState("Skill" + skillDisplayNo);
//		productController.SetStateEffectiveMotion(animState, animClip);
//		//contact with parent machine
//		// IdleOrRun to skill
//		var trans = stateUpperIdleOrRun.AddTransition(animState);
//		trans.AddCondition(AnimatorConditionMode.If, 0, "Skill" + skillDisplayNo);
//		trans.hasExitTime = false;
//		trans.duration = 0f;
//		trans.interruptionSource = TransitionInterruptionSource.Source;
//		// skill to IdleOrRun
//		trans = animState.AddTransition(stateUpperIdleOrRun);
//		// trans.AddCondition(AnimatorConditionMode.If, 0, "SkillFinish");
//		trans.hasExitTime = true;
//		trans.exitTime = 1f;
//		trans.duration = 0f;
//	}

//	private static void SetDefaultTransParams(AnimatorStateTransition trans)
//	{
//		trans.hasExitTime = true;
//		trans.exitTime = 1f;
//		trans.duration = 0f;
//	}

//	private static void Clear()
//	{
//		return;
//	}

//	private static void BindControllerToPrefab()
//	{
//		//GameObject prefab;
//		//GameObject go;
//		//Animator animator;
//		//prefab = Resources.Load(BasicPrefabPath, typeof(GameObject)) as GameObject;
//		//go = Instantiate(prefab);
//		//go.name = "PrefabHero" + curHeroName;

//		//animator = go.GetComponent<Animator>();
//		//if (null == animator)
//		//	animator = go.AddComponent<Animator>();

//		//animator.runtimeAnimatorController = productController;
//		//PrefabUtility.CreatePrefab("Assets/Resources/" + PrefabPath + go.name + ".prefab", go);
//		//DestroyImmediate(go);

//		string fileName = GetHeroPrefabName(curHeroName);
//		if (string.IsNullOrEmpty(fileName))
//		{
//			Debug.LogError("Can not fine hero:" + curHeroName);
//			return;
//		}

//		Object prefab;
//		GameObject go;
//		Animator animator;

//		prefab = Resources.Load(PrefabPath + fileName);
//		go = Instantiate(prefab) as GameObject;

//		animator = go.GetComponent<Animator>();
//		if (null == animator)
//			animator = go.AddComponent<Animator>();

//		animator.runtimeAnimatorController = productController;
//		animator.SetLayerWeight(1, 1f);
//		PrefabUtility.ReplacePrefab(go, prefab);
//		DestroyImmediate(go);
//	}

//	private static void CreateUpperTransitions()
//	{
//		// any state to death
//		AnimatorStateTransition tempTrans = upperLayerMachine.AddAnyStateTransition(stateUpperDeath);
//		tempTrans.AddCondition(AnimatorConditionMode.If, 0, "Dead");
//		tempTrans.duration = 0f;

//		// any state to knockout
//		tempTrans = upperLayerMachine.AddAnyStateTransition(stateUpperDeath);
//		tempTrans.AddCondition(AnimatorConditionMode.If, 0, "KnockOut");
//		tempTrans.duration = 0f;

//		// to exit state
//		tempTrans = stateUpperDeath.AddExitTransition();
//		tempTrans.AddCondition(AnimatorConditionMode.If, 0, "Reborn");
//		tempTrans.duration = 0f;

//		// IdleOrRun to other
//		tempTrans = stateUpperIdleOrRun.AddTransition(stateUpperAim);
//		tempTrans.AddCondition(AnimatorConditionMode.If, 0, "Aim");
//		tempTrans.interruptionSource = TransitionInterruptionSource.Source;
//		tempTrans.duration = 0f;

//		tempTrans = stateUpperIdleOrRun.AddTransition(stateUpperReload);
//		tempTrans.AddCondition(AnimatorConditionMode.If, 0, "Reload");
//		tempTrans.interruptionSource = TransitionInterruptionSource.Source;
//		tempTrans.duration = 0f;

//		// Aim to other
//		tempTrans = stateUpperAim.AddTransition(stateUpperIdleOrRun);
//		tempTrans.AddCondition(AnimatorConditionMode.If, 0, "ArmDown");
//		tempTrans.interruptionSource = TransitionInterruptionSource.Source;
//		tempTrans.duration = 0f;

//		tempTrans = stateUpperAim.AddTransition(stateUpperShoot);
//		tempTrans.AddCondition(AnimatorConditionMode.If, 0, "Shoot");
//		tempTrans.interruptionSource = TransitionInterruptionSource.Source;
//		tempTrans.duration = 0f;

//		tempTrans = stateUpperAim.AddTransition(stateUpperReload);
//		tempTrans.AddCondition(AnimatorConditionMode.If, 0, "Reload");
//		tempTrans.interruptionSource = TransitionInterruptionSource.Source;
//		tempTrans.duration = 0f;

//		// Reload to other
//		tempTrans = stateUpperReload.AddTransition(stateUpperIdleOrRun);
//		tempTrans.hasExitTime = true;
//		tempTrans.exitTime = 1f;
//		tempTrans.interruptionSource = TransitionInterruptionSource.Source;
//		tempTrans.duration = 0f;

//		tempTrans = stateUpperReload.AddTransition(stateUpperAim);
//		tempTrans.AddCondition(AnimatorConditionMode.If, 0, "Aim");
//		tempTrans.hasExitTime = true;
//		tempTrans.exitTime = 1f;
//		tempTrans.interruptionSource = TransitionInterruptionSource.Source;
//		tempTrans.duration = 0f;

//		// Shoot to other
//		tempTrans = stateUpperShoot.AddTransition(stateUpperReload);
//		tempTrans.AddCondition(AnimatorConditionMode.If, 0, "Reload");
//		tempTrans.interruptionSource = TransitionInterruptionSource.Source;
//		tempTrans.duration = 0f;

//		tempTrans = stateUpperShoot.AddTransition(stateUpperIdleOrRun);
//		tempTrans.AddCondition(AnimatorConditionMode.If, 0, "ArmDown");
//		tempTrans.interruptionSource = TransitionInterruptionSource.Source;
//		tempTrans.duration = 0f;

//		tempTrans = stateUpperShoot.AddTransition(stateUpperShoot);
//		tempTrans.AddCondition(AnimatorConditionMode.If, 0, "Shoot");
//		tempTrans.hasExitTime = true;
//		tempTrans.exitTime = 1f;
//		tempTrans.interruptionSource = TransitionInterruptionSource.Source;
//		tempTrans.duration = 0f;
//	}

//	private static void CreateBaseTransitions()
//	{
//		// Idle To Other
//		var trans = stateBaseIdleOrRun.AddTransition(stateBaseLowerMove);
//		trans.AddCondition(AnimatorConditionMode.If, 0, "Aim");
//		trans.interruptionSource = TransitionInterruptionSource.Source;
//		trans.duration = 0;

//		trans = stateBaseIdleOrRun.AddTransition(stateBaseLowerMove);
//		trans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, "Shoot");
//		trans.interruptionSource = TransitionInterruptionSource.Source;
//		trans.duration = 0;

//		// Lower To Other
//		trans = stateBaseLowerMove.AddTransition(stateBaseIdleOrRun);
//		trans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, "ArmDown");
//		trans.interruptionSource = TransitionInterruptionSource.Source;
//		trans.duration = 0;

//		trans = stateBaseLowerMove.AddTransition(stateBaseIdleOrRun);
//		trans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, "SkillFinish");
//		trans.interruptionSource = TransitionInterruptionSource.Source;
//		trans.duration = 0;

//		trans = stateBaseLowerMove.AddTransition(stateBaseTurnLeft);
//		trans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, "TurnLeft");
//		trans.interruptionSource = TransitionInterruptionSource.Source;
//		trans.duration = 0;

//		trans = stateBaseLowerMove.AddTransition(stateBaseTurnRight);
//		trans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, "TurnRight");
//		trans.interruptionSource = TransitionInterruptionSource.Source;
//		trans.duration = 0;

//		// TurnLeft To Other
//		trans = stateBaseTurnLeft.AddTransition(stateBaseIdleOrRun);
//		trans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, "ArmDown");
//		trans.interruptionSource = TransitionInterruptionSource.Source;
//		trans.duration = 0;

//		trans = stateBaseTurnLeft.AddTransition(stateBaseIdleOrRun);
//		trans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.Greater, 0, "RunSpeed");
//		trans.interruptionSource = TransitionInterruptionSource.Source;
//		trans.duration = 0;

//		trans = stateBaseTurnLeft.AddTransition(stateBaseIdleOrRun);
//		trans.hasExitTime = true;
//		trans.exitTime = 0.9f;
//		trans.interruptionSource = TransitionInterruptionSource.Source;
//		trans.duration = 0;

//		// TurnRight To Other
//		trans = stateBaseTurnRight.AddTransition(stateBaseIdleOrRun);
//		trans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, "ArmDown");
//		trans.interruptionSource = TransitionInterruptionSource.Source;
//		trans.duration = 0;

//		trans = stateBaseTurnRight.AddTransition(stateBaseIdleOrRun);
//		trans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.Greater, 0, "RunSpeed");
//		trans.interruptionSource = TransitionInterruptionSource.Source;
//		trans.duration = 0;

//		trans = stateBaseTurnRight.AddTransition(stateBaseIdleOrRun);
//		trans.hasExitTime = true;
//		trans.exitTime = 0.9f;
//		trans.interruptionSource = TransitionInterruptionSource.Source;
//		trans.duration = 0;
//	}
//	#endregion private imp

//}