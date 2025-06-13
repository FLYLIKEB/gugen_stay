using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;  // 기본 이동 속도
    public float jumpForce = 10f; // 점프 힘
    public bool isGrounded;       // 땅에 있는지 체크
    public float groundCheckRadius = 0.2f; // 지면 체크 반경
    public Transform groundCheck; // 지면 체크 위치
    public LayerMask groundLayer; // 지면 레이어 마스크

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sprite;
    private bool isMovingToTarget = false;
    private Transform targetPosition;
    private float moveSpeedToTarget = 5f;
    private Inventory inventory;  // 인벤토리 참조 추가
    
    // 애니메이션 파라미터 이름 - PlayerAnimation 스크립트와 일치시킴
    private readonly string IS_WALKING = "IsWalking"; 
    private readonly string IS_JUMPING = "IsJumping";
    private readonly string JUMP_TRIGGER = "Jump"; // 점프 트리거(있다면)

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        
        // 인벤토리 컴포넌트 추가
        inventory = GetComponent<Inventory>();
        if (inventory == null)
        {
            // 인벤토리 컴포넌트가 없으면 추가
            inventory = gameObject.AddComponent<Inventory>();
        }
        
        // 플레이어 태그 설정
        if (gameObject.tag != "Player")
        {
            gameObject.tag = "Player";
        }
        
        // 콜라이더 확인 및 수정
        Collider2D collider = GetComponent<Collider2D>();
        if (collider == null || (collider.bounds.size.x <= 0.01f && collider.bounds.size.y <= 0.01f))
        {
            // 콜라이더가 없거나 크기가 0이면 추가
            if (collider != null)
            {
                Debug.LogWarning("플레이어의 콜라이더 크기가 0입니다. 콜라이더를 수정합니다.");
                DestroyImmediate(collider);
            }
            
            // 스프라이트 크기 기반으로 BoxCollider2D 생성
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null && spriteRenderer.sprite != null)
            {
                BoxCollider2D boxCollider = gameObject.AddComponent<BoxCollider2D>();
                // 스프라이트 크기의 90%로 콜라이더 설정
                boxCollider.size = new Vector2(
                    spriteRenderer.bounds.size.x * 0.9f,
                    spriteRenderer.bounds.size.y * 0.9f
                );
                // 콜라이더 위치 조정 (발 위치 고려)
                boxCollider.offset = new Vector2(0, 0);
                
                collider = boxCollider;
                Debug.Log($"플레이어에 BoxCollider2D 추가됨: 크기={boxCollider.size}, 오프셋={boxCollider.offset}");
            }
            else
            {
                // 스프라이트 렌더러가 없으면 기본 크기로 설정
                BoxCollider2D boxCollider = gameObject.AddComponent<BoxCollider2D>();
                boxCollider.size = new Vector2(1f, 1f);
                collider = boxCollider;
                Debug.Log("플레이어에 기본 크기(1x1)의 BoxCollider2D 추가됨");
            }
        }
        
        // 지면 확인 위치 설정
        if (groundCheck == null)
        {
            // GroundCheck 오브젝트 생성
            GameObject checkObj = new GameObject("GroundCheck");
            checkObj.transform.parent = transform;
            
            // 콜라이더를 다시 가져옴 (이 시점에는 확실히 존재함)
            collider = GetComponent<Collider2D>();
            
            // 플레이어 발 아래에 위치하도록 설정
            if (collider != null && collider.bounds.size.y > 0.1f)
            {
                // 플레이어 발 아래에 위치하도록 설정 (콜라이더 하단에서 약간 아래로)
                float yOffset = -collider.bounds.extents.y - 0.05f;
                checkObj.transform.localPosition = new Vector3(0, yOffset, 0);
                Debug.Log($"콜라이더 정보: 중심={collider.bounds.center}, 크기={collider.bounds.size}, 높이={collider.bounds.size.y}");
            }
            else
            {
                // 기본값 (스프라이트 크기 추정해서 아래쪽에 배치)
                SpriteRenderer sprite = GetComponent<SpriteRenderer>();
                float yOffset = -0.5f;  // 기본값
                
                if (sprite != null && sprite.bounds.size.y > 0.1f)
                {
                    yOffset = -sprite.bounds.extents.y - 0.05f;
                    Debug.Log($"스프라이트 정보: 크기={sprite.bounds.size}, 높이={sprite.bounds.size.y}");
                }
                else
                {
                    Debug.LogWarning("유효한 콜라이더나 스프라이트를 찾을 수 없어 기본 위치를 사용합니다.");
                }
                
                checkObj.transform.localPosition = new Vector3(0, yOffset, 0);
            }
            
            groundCheck = checkObj.transform;
            Debug.Log("GroundCheck 오브젝트가 생성되었습니다: " + groundCheck.localPosition + " (로컬좌표)");
            Debug.Log("GroundCheck 월드좌표: " + groundCheck.position);
        }
        
        // 레이어 마스크 설정
        if (groundLayer == 0)
        {
            groundLayer = LayerMask.GetMask("Ground");
            Debug.Log("Ground 레이어 마스크 자동 설정: " + groundLayer.value);
            
            // 레이어가 올바르게 설정되었는지 확인
            if (groundLayer.value == 0)
            {
                Debug.LogError("Ground 레이어가 없습니다! 프로젝트 설정에서 Layer를 추가해주세요.");
                // Ground 레이어가 없으면 일단 Default 레이어 사용
                groundLayer = LayerMask.GetMask("Default");
            }
        }
        
        // groundCheckRadius 설정 (더 넓게 설정)
        if (groundCheckRadius < 0.1f)
        {
            groundCheckRadius = 0.3f; // 더 큰 값으로 증가
            Debug.Log("groundCheckRadius가 0.3로 설정되었습니다.");
        }
        
        // 리지드바디 설정 확인
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            Debug.Log("Rigidbody2D 설정 완료 - 회전 제한, 연속 충돌 감지");
        }
        
        // 애니메이터 파라미터 존재 여부 확인
        if (anim != null) 
        {
            CheckAnimatorParameters();
        }
        
        Debug.Log("플레이어가 사용하는 태그: " + gameObject.tag);
        Debug.Log("지면 감지에 사용하는 태그: Ground");
        
        // 시작 시 강제로 첫 FixedUpdate에서 지면 체크 실행
        Invoke("ForceGroundCheck", 0.2f);
    }

    private void ForceGroundCheck()
    {
        Debug.Log("강제 지면 체크 실행");
        CheckGround();
    }

    // 애니메이터 파라미터 존재 확인 및 로그 출력
    private void CheckAnimatorParameters()
    {
        bool hasWalkingParam = false;
        bool hasJumpingParam = false;
        bool hasJumpTrigger = false;
        
        foreach (AnimatorControllerParameter param in anim.parameters)
        {
            if (param.name == IS_WALKING) hasWalkingParam = true;
            if (param.name == IS_JUMPING) hasJumpingParam = true;
            if (param.name == JUMP_TRIGGER) hasJumpTrigger = true;
        }
        
        if (!hasWalkingParam) 
            Debug.LogWarning($"애니메이터에 '{IS_WALKING}' 파라미터가 없습니다. Animator Controller에 Bool 타입 파라미터를 추가하세요.");
        if (!hasJumpingParam) 
            Debug.LogWarning($"애니메이터에 '{IS_JUMPING}' 파라미터가 없습니다. Animator Controller에 Bool 타입 파라미터를 추가하세요.");
        if (!hasJumpTrigger) 
            Debug.LogWarning($"애니메이터에 '{JUMP_TRIGGER}' 트리거가 없습니다. 점프 애니메이션을 사용한다면 추가하세요.");
    }

    void FixedUpdate()
    {
        // 지면 체크 로직 추가 (원형 캐스트 방식)
        CheckGround();
        
        if (isMovingToTarget && targetPosition != null)
        {
            // 목표 위치로 이동 (Rigidbody2D의 velocity 사용)
            Vector2 direction = (targetPosition.position - transform.position).normalized;
            rb.velocity = direction * moveSpeedToTarget;

            // 목표 위치에 도달하면 이동 멈춤
            if (Vector2.Distance(transform.position, targetPosition.position) < 0.1f)
            {
                isMovingToTarget = false;
                targetPosition = null;
                rb.velocity = Vector2.zero; // 이동 종료 후 속도 초기화
            }
        }
        else
        {
            // ⭐ 기본 이동 (좌우 방향 이동 활성화)
            float moveInput = Input.GetAxis("Horizontal");
            rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

            // 캐릭터 방향 전환
            if (moveInput > 0)
                sprite.flipX = false;
            else if (moveInput < 0)
                sprite.flipX = true;

            // 애니메이션 설정 - 수정된 파라미터 이름 사용
            if (anim != null)
            {
                anim.SetBool(IS_WALKING, moveInput != 0);
                anim.SetBool(IS_JUMPING, !isGrounded);
            }
        }
    }

    void Update()
    {
        // 점프 (Space 키)
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            
            // 점프 트리거 실행 (파라미터가 있다면)
            if (anim != null)
            {
                // Animator에 'Jump' 트리거가 있는지 먼저 확인
                bool hasTrigger = false;
                foreach (AnimatorControllerParameter param in anim.parameters)
                {
                    if (param.name == JUMP_TRIGGER && param.type == AnimatorControllerParameterType.Trigger)
                    {
                        hasTrigger = true;
                        break;
                    }
                }
                
                if (hasTrigger)
                {
                    anim.SetTrigger(JUMP_TRIGGER);
                }
                // 어떤 경우든 IS_JUMPING 파라미터 설정
                anim.SetBool(IS_JUMPING, true);
            }
        }
        
        // E 키를 눌러 아이템 상호작용
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("E키 입력 감지");
            // 아이템 상호작용 범위 내의 오브젝트 검사
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1.5f);
            Debug.Log($"감지된 콜라이더 수: {colliders.Length}");
            
            foreach (Collider2D collider in colliders)
            {
                Debug.Log($"감지된 오브젝트: {collider.gameObject.name}, 태그: {collider.gameObject.tag}");
                
                // NPC와 상호작용
                if (collider.CompareTag("NPC"))
                {
                    NPCController npc = collider.GetComponent<NPCController>();
                    if (npc != null)
                    {
                        npc.StartDialogue();
                    }
                }
                // 아이템 상호작용
                else if (collider.CompareTag("Item"))
                {
                    // 아이템 정보 가져오기
                    ItemObject itemObject = collider.GetComponent<ItemObject>();
                    if (itemObject != null && itemObject.item != null)
                    {
                        Debug.Log($"아이템 발견: {itemObject.item.itemName}");
                        // 인벤토리에 아이템 추가
                        if (inventory.AddItem(itemObject.item))
                        {
                            Debug.Log($"{itemObject.item.itemName} 획득!");
                            Destroy(collider.gameObject);
                        }
                    }
                    else
                    {
                        Debug.Log("ItemObject 컴포넌트 또는 아이템 데이터가 없습니다.");
                    }
                }
            }
        }
    }

    // 지면 체크 메서드
    private void CheckGround()
    {
        if (groundCheck == null) return;
        
        // 이전 상태 저장
        bool wasGrounded = isGrounded;
        
        // 원래 값 무효화
        isGrounded = false;
        
        // 모든 콜라이더를 확인 (레이어 필터링 없이)
        Collider2D[] allColliders = Physics2D.OverlapCircleAll(groundCheck.position, groundCheckRadius);
        
        if (allColliders.Length > 0)
        {
            foreach (Collider2D col in allColliders)
            {
                // "Ground"로 시작하는 오브젝트 이름만 지면으로 인식
                if (col.gameObject.name.StartsWith("Ground") || col.gameObject.CompareTag("Ground"))
                {
                    isGrounded = true;
                    break;
                }
            }
        }
        
        // 애니메이션 업데이트
        if (anim != null)
        {
            anim.SetBool(IS_JUMPING, !isGrounded);
        }
    }

    private System.Collections.IEnumerator ShowLogOnScreen(string message, Color color)
    {
        // 로깅 비활성화로 빈 코루틴 유지
        yield break;
    }

    // 충돌 감지 이벤트 - 보조적인 지면 확인 방식으로 사용
    void OnCollisionEnter2D(Collision2D collision)
    {
        // "Ground"로 시작하는 오브젝트 이름만 지면으로 인식
        if (collision.gameObject.name.StartsWith("Ground") || collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            
            // 착지 시 IS_JUMPING 파라미터 false로 설정
            if (anim != null)
            {
                anim.SetBool(IS_JUMPING, false);
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        // "Ground"로 시작하는 오브젝트 이름만 지면으로 인식
        if (collision.gameObject.name.StartsWith("Ground") || collision.gameObject.CompareTag("Ground"))
        {
            // 즉시 false로 설정하지 않고 CheckGround에서 처리하도록 함
            // isGrounded = false;
        }
    }

    // 🎯 플레이어를 특정 위치로 이동하는 함수 (이동 후 다시 움직일 수 있도록 수정) - 현재 안쓰임
    public void MoveToTarget(Transform newTarget, float speed)
    {
        targetPosition = newTarget;
        moveSpeedToTarget = speed;
        isMovingToTarget = true;
        rb.velocity = Vector2.zero; // 이동 시작 전에 속도 초기화하여 버그 방지
    }

    // 🎯 특정 위치로 순간이동하는 함수
    public void TeleportTo(Transform newTarget)
    {
        transform.position = newTarget.position; // ⭐ 즉시 이동
        rb.velocity = Vector2.zero; // 이동 후 속도 초기화
    }

    // 디버깅용 시각화
    private void OnDrawGizmosSelected()
    {
        // 에디터에서만 그리고, 디버그 모드일 때만 표시
        #if UNITY_EDITOR
        if (Application.isPlaying && Debug.isDebugBuild)
        {
            if (groundCheck != null)
            {
                // 지면 체크 영역 표시
                Gizmos.color = isGrounded ? Color.green : Color.red;
                Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
            }
        }
        #endif
    }
}