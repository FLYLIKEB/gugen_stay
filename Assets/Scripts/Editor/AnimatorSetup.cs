using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

public class AnimatorSetup : EditorWindow
{
    private AnimatorController controller;
    
    [MenuItem("Tools/애니메이터 설정")]
    public static void ShowWindow()
    {
        GetWindow<AnimatorSetup>("애니메이터 설정");
    }
    
    private void OnGUI()
    {
        GUILayout.Label("플레이어 애니메이터 설정", EditorStyles.boldLabel);
        
        controller = EditorGUILayout.ObjectField("애니메이터 컨트롤러", controller, typeof(AnimatorController), false) as AnimatorController;
        
        if (GUILayout.Button("필수 파라미터 추가"))
        {
            AddRequiredParameters();
        }
        
        if (GUILayout.Button("기본 트랜지션 생성"))
        {
            CreateDefaultTransitions();
        }
    }
    
    private void AddRequiredParameters()
    {
        if (controller == null)
        {
            Debug.LogError("애니메이터 컨트롤러가 선택되지 않았습니다.");
            return;
        }
        
        bool hasIsWalking = false;
        bool hasIsJumping = false;
        bool hasJump = false;
        
        // 기존 파라미터 확인
        foreach (AnimatorControllerParameter param in controller.parameters)
        {
            if (param.name == "IsWalking" && param.type == AnimatorControllerParameterType.Bool)
                hasIsWalking = true;
            if (param.name == "IsJumping" && param.type == AnimatorControllerParameterType.Bool)
                hasIsJumping = true;
            if (param.name == "Jump" && param.type == AnimatorControllerParameterType.Trigger)
                hasJump = true;
        }
        
        // 필요한 파라미터 추가
        if (!hasIsWalking)
        {
            controller.AddParameter("IsWalking", AnimatorControllerParameterType.Bool);
            Debug.Log("'IsWalking' 파라미터가 추가되었습니다.");
        }
        
        if (!hasIsJumping)
        {
            controller.AddParameter("IsJumping", AnimatorControllerParameterType.Bool);
            Debug.Log("'IsJumping' 파라미터가 추가되었습니다.");
        }
        
        if (!hasJump)
        {
            controller.AddParameter("Jump", AnimatorControllerParameterType.Trigger);
            Debug.Log("'Jump' 트리거가 추가되었습니다.");
        }
        
        EditorUtility.SetDirty(controller);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    
    private void CreateDefaultTransitions()
    {
        if (controller == null)
        {
            Debug.LogError("애니메이터 컨트롤러가 선택되지 않았습니다.");
            return;
        }
        
        // 레이어 확인
        if (controller.layers.Length == 0)
        {
            Debug.LogError("애니메이터 컨트롤러에 레이어가 없습니다.");
            return;
        }
        
        AnimatorStateMachine rootStateMachine = controller.layers[0].stateMachine;
        
        // 기본 상태 확인/생성
        AnimatorState idleState = null;
        AnimatorState walkingState = null;
        AnimatorState jumpingState = null;
        
        // 기존 상태 확인
        foreach (var state in rootStateMachine.states)
        {
            if (state.state.name == "Idle" || state.state.name == "Standing")
                idleState = state.state;
            else if (state.state.name == "Walking" || state.state.name == "Running")
                walkingState = state.state;
            else if (state.state.name == "Jumping")
                jumpingState = state.state;
        }
        
        // 필요한 상태 생성
        if (idleState == null)
        {
            idleState = rootStateMachine.AddState("Idle");
            Debug.Log("'Idle' 상태가 생성되었습니다.");
        }
        
        if (walkingState == null)
        {
            walkingState = rootStateMachine.AddState("Walking");
            Debug.Log("'Walking' 상태가 생성되었습니다.");
        }
        
        if (jumpingState == null)
        {
            jumpingState = rootStateMachine.AddState("Jumping");
            Debug.Log("'Jumping' 상태가 생성되었습니다.");
        }
        
        // 기본 상태 설정
        rootStateMachine.defaultState = idleState;
        
        // 트랜지션 생성 (중복 방지)
        bool hasIdleToWalking = false;
        bool hasWalkingToIdle = false;
        bool hasToJumping = false;
        bool hasJumpingToIdle = false;
        
        // 기존 트랜지션 확인
        foreach (var transition in idleState.transitions)
        {
            if (transition.destinationState == walkingState)
                hasIdleToWalking = true;
            else if (transition.destinationState == jumpingState)
                hasToJumping = true;
        }
        
        foreach (var transition in walkingState.transitions)
        {
            if (transition.destinationState == idleState)
                hasWalkingToIdle = true;
            else if (transition.destinationState == jumpingState)
                hasToJumping = true;
        }
        
        foreach (var transition in jumpingState.transitions)
        {
            if (transition.destinationState == idleState)
                hasJumpingToIdle = true;
        }
        
        // 필요한 트랜지션 추가
        if (!hasIdleToWalking)
        {
            var transition = idleState.AddTransition(walkingState);
            transition.hasExitTime = false;
            transition.duration = 0.1f;
            transition.AddCondition(AnimatorConditionMode.If, 0, "IsWalking");
            Debug.Log("Idle -> Walking 트랜지션이 추가되었습니다.");
        }
        
        if (!hasWalkingToIdle)
        {
            var transition = walkingState.AddTransition(idleState);
            transition.hasExitTime = false;
            transition.duration = 0.1f;
            transition.AddCondition(AnimatorConditionMode.IfNot, 0, "IsWalking");
            Debug.Log("Walking -> Idle 트랜지션이 추가되었습니다.");
        }
        
        if (!hasToJumping)
        {
            // Idle -> Jumping
            var transitionIdle = idleState.AddTransition(jumpingState);
            transitionIdle.hasExitTime = false;
            transitionIdle.duration = 0.1f;
            transitionIdle.AddCondition(AnimatorConditionMode.If, 0, "IsJumping");
            Debug.Log("Idle -> Jumping 트랜지션이 추가되었습니다.");
            
            // Walking -> Jumping
            var transitionWalking = walkingState.AddTransition(jumpingState);
            transitionWalking.hasExitTime = false;
            transitionWalking.duration = 0.1f;
            transitionWalking.AddCondition(AnimatorConditionMode.If, 0, "IsJumping");
            Debug.Log("Walking -> Jumping 트랜지션이 추가되었습니다.");
        }
        
        if (!hasJumpingToIdle)
        {
            var transition = jumpingState.AddTransition(idleState);
            transition.hasExitTime = false;
            transition.duration = 0.1f;
            transition.AddCondition(AnimatorConditionMode.IfNot, 0, "IsJumping");
            Debug.Log("Jumping -> Idle 트랜지션이 추가되었습니다.");
        }
        
        EditorUtility.SetDirty(controller);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
} 