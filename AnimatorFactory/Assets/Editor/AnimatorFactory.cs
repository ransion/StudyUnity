using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

public static class AnimatorFactoryUtil
{
	public static AnimationClip LoadAnimClip(string path)
	{
		return (AnimationClip)AssetDatabase.LoadAssetAtPath(path, typeof(AnimationClip));
	}

	public static AnimationClip LoadAnimClip(string fbxPath, string animPath)
	{
		var objs = AssetDatabase.LoadAllAssetsAtPath(fbxPath);
		return objs.Where(o => o is AnimationClip && o.name.Equals(animPath)).Select(o => o as AnimationClip).FirstOrDefault();
	}
}

public class AnimatorFactory : EditorWindow
{
	#region const
	private const string AnimatorSavePath = "Assets/Standard Assets/Characters/ThirdPersonCharacter/Animator/";
	private const string PrefabSavePath = "Assets/Standard Assets/Characters/ThirdPersonCharacter/Prefabs/";
	private const string ModelPath = "Assets/Standard Assets/Characters/ThirdPersonCharacter/Models/";
	private const string AnimationPath = "Assets/Standard Assets/Characters/ThirdPersonCharacter/Animation/Humanoid";
	private const string AnimatorControllerSuffix = "AnimatorController.controller";
	#endregion const

	#region private members
	private string characterName;
	private AnimatorController productController;
	private AnimatorStateMachine baseLayerMachine;
	private AnimatorStateMachine crouchLayerMachine;
	// base layer states
	private AnimatorState stateIdle;
	private AnimatorState stateMove;
	private AnimatorState stateJump;
	private AnimatorState stateDeath;
	// crouch layer states
	private AnimatorState stateWalk;
	private AnimatorState stateWalkLeft;
	private AnimatorState stateWalkRight;
	#endregion private members

	#region EditorWindow
	[MenuItem("Window/AnimatorFactory")]
	public static void OpenWindow()
	{
		EditorWindow.GetWindow(typeof(AnimatorFactory));
	}

	void OnEnable()
	{
		// set default name
		characterName = "Ethan";
	}

	void OnGUI()
	{
		GUILayout.Label("Animation Settings", EditorStyles.boldLabel);
		characterName = EditorGUILayout.TextField("Character Name： ", characterName);

		if (GUILayout.Button("Generate Controller"))
			GenerateController();
	}
	#endregion EditorWindow

	#region public interface
	public void GenerateController()
	{
		if (string.IsNullOrEmpty(characterName))
			return;

		// create animator controller
		CreateController();
		// add controller parameters
		AddParameters();
		// Create Anim States
		CreateAnimStates();
		// Bind aniamtor controller to prefab
		BindControllerToPrefab();
	}
	#endregion public interface

	#region private imp
	private void CreateAnimStates()
	{
		// Create base layer states
		CreateBaseLayerState();
		// Create crouch layer states
		CreateCrouchLayerState();
	}

	private void CreateBaseLayerState()
	{
		CreateIdle();
		CreateMove();
		CreateJump();
		CreateDeath();
		SetBaseLayerTransition();
	}

	private void CreateCrouchLayerState()
	{
		// Load Animation
		string fbxPath = AnimationPath + "Crouch.FBX";
		CreateCrouchIdle(fbxPath);
		CreateCrouchWalk(fbxPath);
	}

	private void BindControllerToPrefab()
	{
		// Generate prefab first time
		var prefab = AssetDatabase.LoadAssetAtPath(ModelPath + characterName + ".fbx", typeof(GameObject)) as GameObject;
		var go = Instantiate(prefab);
		go.name = characterName + "Prefab";

		var animator = go.GetComponent<Animator>() ?? go.AddComponent<Animator>();

		animator.runtimeAnimatorController = productController;
		PrefabUtility.CreatePrefab(PrefabSavePath + go.name + ".prefab", go);
		DestroyImmediate(go);

		// If there has been a prefab, just bind controller to it.
		//var prefab = AssetDatabase.LoadAssetAtPath(PrefabSavePath + characterName, typeof(GameObject)) as GameObject;
		//var go = Instantiate(prefab) as GameObject;

		//var animator = go.GetComponent<Animator>() ?? go.AddComponent<Animator>();

		//animator.runtimeAnimatorController = productController;
		//PrefabUtility.ReplacePrefab(go, prefab);
		//DestroyImmediate(go);
	}

	/// <summary>
	/// Show how to create a controller, and set layer parameters
	/// </summary>
	private void CreateController()
	{
		productController = AnimatorController.CreateAnimatorControllerAtPath(AnimatorSavePath + characterName + AnimatorControllerSuffix);
		// get base machine in base layer
		baseLayerMachine = productController.layers[0].stateMachine;
		// set base machine parameters
		baseLayerMachine.entryPosition = Vector3.zero;
		baseLayerMachine.exitPosition = new Vector3(400f, 200f);
		baseLayerMachine.anyStatePosition = new Vector3(0f, 200f);

		// add crouch layer to controller
		productController.AddLayer("CrouchLayer");
		// get a copy from controller's layer
		AnimatorControllerLayer[] layers = productController.layers;
		// set layer parameters
		layers[1].defaultWeight = 1f;
		layers[1].blendingMode = AnimatorLayerBlendingMode.Override;
		// save layer setting to controller
		productController.layers = layers;
		// get state machine in crouch layer
		crouchLayerMachine = productController.layers[1].stateMachine;
		// set crouch machine parameters
		crouchLayerMachine.entryPosition = Vector3.zero;
		crouchLayerMachine.exitPosition = new Vector3(600f, 200f);
		crouchLayerMachine.anyStatePosition = new Vector3(0f, 200f);
	}

	/// <summary>
	/// Show how to add parameters to the controller
	/// </summary>
	private void AddParameters()
	{
		// use AddParameter interface
		productController.AddParameter("FloatA", AnimatorControllerParameterType.Float);
		productController.AddParameter("FloatB", AnimatorControllerParameterType.Float);
		productController.AddParameter("TriggerA", AnimatorControllerParameterType.Trigger);
		productController.AddParameter("TriggerB", AnimatorControllerParameterType.Trigger);
		productController.AddParameter("TriggerC", AnimatorControllerParameterType.Trigger);
		productController.AddParameter("BooleanA", AnimatorControllerParameterType.Bool);
		// if you want to set default value
		AnimatorControllerParameter playSpeed = new AnimatorControllerParameter();
		playSpeed.name = "PlaySpeed";
		playSpeed.type = AnimatorControllerParameterType.Float;
		playSpeed.defaultFloat = 1.0f;
		productController.AddParameter(playSpeed);
	}

	/// <summary>
	/// Show how to add a child machine
	/// </summary>
	/// <param name="fbxPath"></param>
	private void CreateCrouchWalk(string fbxPath)
	{
		AnimatorStateMachine walkMachine = crouchLayerMachine.AddStateMachine("Walk", new Vector3(0f, 100f));
		walkMachine.entryPosition = Vector3.zero;
		walkMachine.anyStatePosition = new Vector3(0f, -200f);
		walkMachine.exitPosition = new Vector3(0f, -400f);

		AnimationClip walkClip = AnimatorFactoryUtil.LoadAnimClip(fbxPath, "HumanoidCrouchWalk");
		AnimationClip walkLeftClip = AnimatorFactoryUtil.LoadAnimClip(fbxPath, "HumanoidCrouchWalkLeft");
		AnimationClip walkRightClip = AnimatorFactoryUtil.LoadAnimClip(fbxPath, "HumanoidCrouchWalkRight");
		stateWalk = walkMachine.AddState("Walk", new Vector3(200f, 0f));
		stateWalk.motion = walkClip;
		stateWalkLeft = walkMachine.AddState("WalkLeft", new Vector3(200f, -200f));
		stateWalkLeft.motion = walkLeftClip;
		stateWalkRight = walkMachine.AddState("WalkRight", new Vector3(200f, -400f));
		stateWalkRight.motion = walkRightClip;
	}

	/// <summary>
	/// Show how to set speed parameter, And set state motion in other way
	/// </summary>
	/// <param name="fbxPath"></param>
	private void CreateCrouchIdle(string fbxPath)
	{
		// Load Animation
		AnimationClip idleClip = AnimatorFactoryUtil.LoadAnimClip(fbxPath, "HumanoidCrouchIdle");

		stateIdle = crouchLayerMachine.AddState("Idle", new Vector3(300f, 0f));
		stateIdle.speedParameterActive = true;
		stateIdle.speedParameter = "FloatA";

		// Set state motion,the other way
		productController.SetStateEffectiveMotion(stateIdle, idleClip);
		// set to default state
		crouchLayerMachine.defaultState = stateIdle;
	}

	/// <summary>
	/// Show how to add transitions
	/// </summary>
	private void SetBaseLayerTransition()
	{
		var trans = stateIdle.AddTransition(stateMove);
		trans.hasExitTime = true;
		trans.exitTime = 0.9f;
		trans.interruptionSource = TransitionInterruptionSource.Source;
		trans.duration = 0;

		trans = stateMove.AddTransition(stateIdle);
		trans.interruptionSource = TransitionInterruptionSource.Destination;
		trans.duration = 0;
		trans.AddCondition(AnimatorConditionMode.If, 0, "TriggerA");
	}

	/// <summary>
	/// Show how to add 2D tree
	/// </summary>
	private void CreateJump()
	{
		// Load Animation
		string fbxPath = AnimationPath + "IdleJumpUp.FBX";
		AnimationClip fallClip = AnimatorFactoryUtil.LoadAnimClip(fbxPath, "HumanoidFall");
		AnimationClip idleJumpUpClip = AnimatorFactoryUtil.LoadAnimClip(fbxPath, "HumanoidIdleJumpUp");
		AnimationClip jumpUpClip = AnimatorFactoryUtil.LoadAnimClip(fbxPath, "HumanoidJumpUp");
		AnimationClip midAirClip = AnimatorFactoryUtil.LoadAnimClip(fbxPath, "HumanoidMidAir");

		// create a tree
		BlendTree tree = new BlendTree();

		// Set blendtree parameters
		tree.name = "Jump";
		tree.blendType = BlendTreeType.FreeformDirectional2D;
		tree.useAutomaticThresholds = true;
		tree.minThreshold = 0f;
		tree.maxThreshold = 1f;
		tree.blendParameter = "FloatA";
		tree.blendParameterY = "FloatB";

		// Add clip to BlendTree
		tree.AddChild(fallClip, new Vector2(0f, 0f));
		tree.AddChild(idleJumpUpClip, new Vector2(0f, 1f));
		tree.AddChild(jumpUpClip, new Vector2(1f, 0f));
		tree.AddChild(midAirClip, new Vector2(-1f, 0f));

		// Add tree to controller asset
		if (AssetDatabase.GetAssetPath(productController) != string.Empty)
		{
			AssetDatabase.AddObjectToAsset(tree, AssetDatabase.GetAssetPath(productController));
		}

		// add tree state & set state motion
		stateJump = baseLayerMachine.AddState(tree.name, new Vector3(300f, -100f));
		stateJump.motion = tree;
	}

	/// <summary>
	/// Show how to add a 1D tree
	/// </summary>
	private void CreateMove()
	{
		// Load Animation
		AnimationClip walkClip = AnimatorFactoryUtil.LoadAnimClip(AnimationPath + "Walk.FBX");
		AnimationClip runClip = AnimatorFactoryUtil.LoadAnimClip(AnimationPath + "Run.FBX");

		// new a tree
		BlendTree tree = new BlendTree();

		// Set blendtree parameters
		tree.name = "Move";
		tree.blendType = BlendTreeType.Simple1D;
		tree.useAutomaticThresholds = true;
		tree.minThreshold = 0f;
		tree.maxThreshold = 1f;
		tree.blendParameter = "FloatA";

		// Add clip to BlendTree
		tree.AddChild(walkClip, 0f);
		tree.AddChild(runClip, 1f);

		// Add tree to controller asset
		if (AssetDatabase.GetAssetPath(productController) != string.Empty)
		{
			AssetDatabase.AddObjectToAsset(tree, AssetDatabase.GetAssetPath(productController));
		}

		// add tree state & set state motion
		stateMove = baseLayerMachine.AddState(tree.name, new Vector3(600f, 0f));
		stateMove.motion = tree;
	}

	/// <summary>
	/// Show how to add a basic state, And add a behaviour
	/// </summary>
	private void CreateIdle()
	{
		// Load Animation
		AnimationClip idleClip = AnimatorFactoryUtil.LoadAnimClip(AnimationPath + "Idle.FBX");

		// add tree state & set state motion
		stateIdle = baseLayerMachine.AddState("Idle", new Vector3(300f, 0f));
		stateIdle.motion = idleClip;

		// Add behaviour to state
		stateIdle.AddStateMachineBehaviour<CharacterIdleState>();

		// set to default state
		baseLayerMachine.defaultState = stateIdle;
	}

	/// <summary>
	/// Show how to relate exitState and anyState
	/// </summary>
	private void CreateDeath()
	{
		AnimationClip deathClip = AnimatorFactoryUtil.LoadAnimClip(AnimationPath + "WalkTurn.FBX");
		stateDeath = baseLayerMachine.AddState("Death", new Vector3(200f, 100f));
		productController.SetStateEffectiveMotion(stateDeath, deathClip);

		// death to exit
		var exitTransition = stateDeath.AddExitTransition();
		exitTransition.AddCondition(AnimatorConditionMode.If, 0, "TriggerA");
		exitTransition.duration = 0;

		// anyState to death
		var anyTransition = baseLayerMachine.AddAnyStateTransition(stateDeath);
		anyTransition.AddCondition(AnimatorConditionMode.If, 0, "TriggerB");
		anyTransition.duration = 0;
	}
	#endregion private imp
	
}
