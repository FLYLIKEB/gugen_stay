using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

public class JumpAnimationFixer : EditorWindow
{
    private AnimatorController animatorController;
    private bool hasJumpTrigger = false;
    private bool hasJumpingState = false;
    
    [MenuItem("Tools/애니메이션/점프 트리거 추가")]
    public static void ShowWindow()
    {
        GetWindow<JumpAnimationFixer>("점프 트리거 추가");
    }
    
    private void OnEnable()
    {
        // 자동으로 PlayerAnimator.controller 찾기
        string[] guids = AssetDatabase.FindAssets("t:AnimatorController PlayerAnimator");
        if (guids.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            animatorController = AssetDatabase.LoadAssetAtPath<AnimatorController>(path);
            
            if (animatorController != null)
            {
                CheckAnimatorParameters();
            }
        }
    }
    
    private void OnGUI()
    {
        GUILayout.Label("애니메이터에 Jump 트리거 추가", EditorStyles.boldLabel);
        
        EditorGUILayout.Space();
        
        // 애니메이터 컨트롤러 필드
        AnimatorController newController = (AnimatorController)EditorGUILayout.ObjectField(
            "애니메이터 컨트롤러", animatorController, typeof(AnimatorController), false);
        
        if (newController != animatorController)
        {
            animatorController = newController;
            if (animatorController != null)
            {
                CheckAnimatorParameters();
            }
        }
        
        EditorGUILayout.Space();
        
        // 현재 상태 표시
        if (animatorController != null)
        {
            EditorGUILayout.LabelField("Jump 트리거:", hasJumpTrigger ? "있음 ✓" : "없음 ✗");
            EditorGUILayout.LabelField("Jumping 상태:", hasJumpingState ? "있음 ✓" : "없음 ✗");
        }
        
        EditorGUILayout.Space();
        
        // 버튼 활성화 여부
        GUI.enabled = animatorController != null && (!hasJumpTrigger || !hasJumpingState);
        
        if (GUILayout.Button("Jump 트리거 및 상태 추가"))
        {
            AddJumpAnimation();
        }
        
        GUI.enabled = true;
    }
    
    private void CheckAnimatorParameters()
    {
        hasJumpTrigger = false;
        hasJumpingState = false;
        
        // 트리거 확인
        foreach (AnimatorControllerParameter param in animatorController.parameters)
        {
            if (param.name == "Jump" && param.type == AnimatorControllerParameterType.Trigger)
            {
                hasJumpTrigger = true;
                break;
            }
        }
        
        // 상태 확인
        AnimatorStateMachine rootStateMachine = animatorController.layers[0].stateMachine;
        foreach (ChildAnimatorState state in rootStateMachine.states)
        {
            if (state.state.name == "Jumping")
            {
                hasJumpingState = true;
                break;
            }
        }
    }
    
    private void AddJumpAnimation()
    {
        // 1. Jump 트리거 추가
        if (!hasJumpTrigger)
        {
            animatorController.AddParameter("Jump", AnimatorControllerParameterType.Trigger);
            Debug.Log("Jump 트리거가 추가되었습니다.");
            hasJumpTrigger = true;
        }
        
        // 2. Jumping 상태 추가
        AnimatorStateMachine rootStateMachine = animatorController.layers[0].stateMachine;
        AnimatorState jumpingState = null;
        
        if (!hasJumpingState)
        {
            jumpingState = rootStateMachine.AddState("Jumping");
            Debug.Log("Jumping 상태가 추가되었습니다.");
            hasJumpingState = true;
        }
        else
        {
            // 기존 Jumping 상태 찾기
            foreach (ChildAnimatorState state in rootStateMachine.states)
            {
                if (state.state.name == "Jumping")
                {
                    jumpingState = state.state;
                    break;
                }
            }
        }
        
        // 3. 다른 필요한 상태 찾기 (Idle, Walking)
        AnimatorState idleState = null;
        AnimatorState walkingState = null;
        
        foreach (ChildAnimatorState state in rootStateMachine.states)
        {
            if (state.state.name == "Idle" || state.state.name == "Standing")
            {
                idleState = state.state;
            }
            else if (state.state.name == "Walking" || state.state.name == "Running")
            {
                walkingState = state.state;
            }
        }
        
        // 4. 필요한 상태가 없으면 생성
        if (idleState == null)
        {
            idleState = rootStateMachine.AddState("Idle");
            rootStateMachine.defaultState = idleState;
            Debug.Log("Idle 상태가 추가되었습니다.");
        }
        
        if (walkingState == null)
        {
            walkingState = rootStateMachine.AddState("Walking");
            Debug.Log("Walking 상태가 추가되었습니다.");
        }
        
        // 5. 트랜지션 추가
        // Idle -> Jumping (Jump 트리거)
        bool hasIdleToJumpingTrigger = false;
        foreach (AnimatorStateTransition transition in idleState.transitions)
        {
            if (transition.destinationState == jumpingState && transition.conditions.Length > 0 && transition.conditions[0].parameter == "Jump")
            {
                hasIdleToJumpingTrigger = true;
                break;
            }
        }
        
        if (!hasIdleToJumpingTrigger && idleState != null && jumpingState != null)
        {
            AnimatorStateTransition idleToJumping = idleState.AddTransition(jumpingState);
            idleToJumping.hasExitTime = false;
            idleToJumping.duration = 0.1f;
            idleToJumping.AddCondition(AnimatorConditionMode.If, 0, "Jump");
            Debug.Log("Idle -> Jumping 트랜지션(Jump 트리거)이 추가되었습니다.");
        }
        
        // Walking -> Jumping (Jump 트리거)
        bool hasWalkingToJumpingTrigger = false;
        if (walkingState != null)
        {
            foreach (AnimatorStateTransition transition in walkingState.transitions)
            {
                if (transition.destinationState == jumpingState && transition.conditions.Length > 0 && transition.conditions[0].parameter == "Jump")
                {
                    hasWalkingToJumpingTrigger = true;
                    break;
                }
            }
            
            if (!hasWalkingToJumpingTrigger && jumpingState != null)
            {
                AnimatorStateTransition walkingToJumping = walkingState.AddTransition(jumpingState);
                walkingToJumping.hasExitTime = false;
                walkingToJumping.duration = 0.1f;
                walkingToJumping.AddCondition(AnimatorConditionMode.If, 0, "Jump");
                Debug.Log("Walking -> Jumping 트랜지션(Jump 트리거)이 추가되었습니다.");
            }
        }
        
        // 6. Jumping -> Idle (IsJumping = false)
        bool hasJumpingToIdleTransition = false;
        if (jumpingState != null)
        {
            foreach (AnimatorStateTransition transition in jumpingState.transitions)
            {
                if (transition.destinationState == idleState)
                {
                    hasJumpingToIdleTransition = true;
                    break;
                }
            }
            
            if (!hasJumpingToIdleTransition && idleState != null)
            {
                AnimatorStateTransition jumpingToIdle = jumpingState.AddTransition(idleState);
                jumpingToIdle.hasExitTime = false;
                jumpingToIdle.duration = 0.2f;
                jumpingToIdle.AddCondition(AnimatorConditionMode.IfNot, 0, "IsJumping");
                Debug.Log("Jumping -> Idle 트랜지션이 추가되었습니다.");
            }
        }
        
        // 7. 변경 사항 저장
        EditorUtility.SetDirty(animatorController);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        Debug.Log("애니메이션 설정이 완료되었습니다.");
    }
    
    // 자동 실행 기능 - 로드 시 점프 트리거 자동 추가
    [InitializeOnLoadMethod]
    private static void AutoAddJumpTrigger()
    {
        // 유니티 에디터 시작 시 PlayerAnimator.controller 찾기
        string[] guids = AssetDatabase.FindAssets("t:AnimatorController PlayerAnimator");
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(path);
            
            if (controller != null)
            {
                // 파라미터 확인
                bool hasJumpTrigger = false;
                foreach (AnimatorControllerParameter param in controller.parameters)
                {
                    if (param.name == "Jump" && param.type == AnimatorControllerParameterType.Trigger)
                    {
                        hasJumpTrigger = true;
                        break;
                    }
                }
                
                // Jump 트리거 없으면 추가
                if (!hasJumpTrigger)
                {
                    controller.AddParameter("Jump", AnimatorControllerParameterType.Trigger);
                    EditorUtility.SetDirty(controller);
                    AssetDatabase.SaveAssets();
                    Debug.Log($"Jump 트리거가 '{path}' 컨트롤러에 자동으로 추가되었습니다.");
                }
            }
        }
    }
} 