using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rb;
    private PlayerController playerController;
    
    // 애니메이션 파라미터 이름
    private readonly string IS_WALKING = "IsWalking";
    private readonly string IS_JUMPING = "IsJumping";
    
    // 이동 감지를 위한 변수
    private float moveThreshold = 0.1f;
    
    // 디버그용 변수들
    private bool wasWalking = false;
    private bool wasJumping = false;
    
    private void Awake()
    {
        // 컴포넌트 참조 가져오기
        animator = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
        
        if (animator == null)
        {
            Debug.LogError("Animator 컴포넌트를 찾을 수 없습니다.");
            // 애니메이터가 없으면 추가
            animator = gameObject.AddComponent<Animator>();
            Debug.Log("Animator 컴포넌트가 자동으로 추가되었습니다.");
        }
        
        if (playerController == null)
        {
            Debug.LogWarning("PlayerController를 찾을 수 없습니다. 플레이어 컨트롤러를 먼저 추가하세요.");
            playerController = gameObject.AddComponent<PlayerController>();
        }
        
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D 컴포넌트를 찾을 수 없습니다.");
            // PlayerController가 이미 Rigidbody2D를 설정하므로 여기서는 처리하지 않음
            rb = GetComponent<Rigidbody2D>();
        }
        
        Debug.Log("PlayerAnimation 초기화 완료");
    }
    
    private void Update()
    {
        UpdateAnimationState();
    }
    
    private void UpdateAnimationState()
    {
        if (animator == null || rb == null) return;
        
        bool isWalking = false;
        bool isJumping = false;
        
        // PlayerController를 통한 상태 확인 (권장 방법)
        if (playerController != null)
        {
            // 걷기 상태 감지
            isWalking = Mathf.Abs(playerController.HorizontalInput) > moveThreshold;
            
            // 점프 상태 감지
            isJumping = !playerController.IsGrounded;
        }
        else
        {
            // 직접 입력값으로 이동 감지 (대체 방법)
            float horizontalInput = Input.GetAxis("Horizontal");
            isWalking = Mathf.Abs(horizontalInput) > 0.1f;
            
            // 수평 이동 감지 (Rigidbody 속도)
            float horizontalMove = Mathf.Abs(rb.velocity.x);
            if (horizontalMove > moveThreshold)
            {
                isWalking = true;
            }
            
            // 점프 감지 (키보드 입력 + 수직 속도)
            isJumping = Input.GetButton("Jump") || rb.velocity.y > moveThreshold;
        }
        
        // 상태 변경 로그 출력
        if (wasWalking != isWalking)
        {
            Debug.Log($"걷기 상태 변경: {isWalking}, 속도: {rb.velocity.x}");
            wasWalking = isWalking;
        }
        
        if (wasJumping != isJumping)
        {
            Debug.Log($"점프 상태 변경: {isJumping}, 수직 속도: {rb.velocity.y}");
            wasJumping = isJumping;
        }
        
        // 애니메이터 파라미터 설정
        animator.SetBool(IS_WALKING, isWalking);
        animator.SetBool(IS_JUMPING, isJumping);
    }
    
    // 애니메이터 설정이 올바른지 확인
    private void OnEnable()
    {
        if (animator != null)
        {
            AnimatorControllerParameter[] parameters = animator.parameters;
            bool hasWalkingParam = false;
            bool hasJumpingParam = false;
            
            foreach (var param in parameters)
            {
                if (param.name == IS_WALKING) hasWalkingParam = true;
                if (param.name == IS_JUMPING) hasJumpingParam = true;
            }
            
            if (!hasWalkingParam || !hasJumpingParam)
            {
                Debug.LogWarning($"애니메이터에 필요한 파라미터가 없습니다. IsWalking: {hasWalkingParam}, IsJumping: {hasJumpingParam}");
            }
            else
            {
                Debug.Log("애니메이터 파라미터 확인 완료");
            }
        }
    }
} 