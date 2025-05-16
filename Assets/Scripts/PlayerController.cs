using UnityEngine;

/// <summary>
/// 플레이어 이동 및 점프 동작을 제어하는 컨트롤러
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("이동 설정")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    
    private Rigidbody2D rb;
    private bool isGrounded = false;
    private float horizontalInput = 0f;
    private bool jumpRequest = false;
    
    // 다른 스크립트에서 접근할 속성
    public bool IsGrounded => isGrounded;
    public float HorizontalInput => horizontalInput;
    
    private void Awake()
    {
        // 컴포넌트 참조 가져오기
        rb = GetComponent<Rigidbody2D>();
        
        // Rigidbody2D가 없으면 추가
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            Debug.Log("Rigidbody2D 컴포넌트가 추가되었습니다.");
        }
        
        // GroundCheck 참조 확인
        if (groundCheck == null)
        {
            // GroundCheck 생성
            GameObject checkObj = new GameObject("GroundCheck");
            checkObj.transform.parent = transform;
            checkObj.transform.localPosition = new Vector3(0, -0.5f, 0); // 플레이어 바닥
            groundCheck = checkObj.transform;
            Debug.Log("GroundCheck 오브젝트가 생성되었습니다.");
        }
        
        // 태그 설정
        if (gameObject.tag != "Player")
        {
            gameObject.tag = "Player";
        }
        
        // 레이어 마스크 설정
        if (groundLayer == 0)
        {
            groundLayer = LayerMask.GetMask("Ground");
            Debug.Log("groundLayer가 자동 설정되었습니다. 레이어 설정을 확인하세요.");
        }
    }
    
    private void Update()
    {
        // 입력 처리
        horizontalInput = Input.GetAxis("Horizontal");
        
        // 점프 요청 확인
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            jumpRequest = true;
        }
    }
    
    private void FixedUpdate()
    {
        // 지면 체크
        CheckGrounded();
        
        // 이동 처리
        Move();
        
        // 점프 처리
        if (jumpRequest)
        {
            Jump();
            jumpRequest = false;
        }
    }
    
    private void CheckGrounded()
    {
        // 원형 캐스트로 지면 감지
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }
    
    private void Move()
    {
        // 현재 수직 속도 유지하며 수평 이동만 적용
        Vector2 targetVelocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);
        rb.velocity = targetVelocity;
        
        // 방향에 따라 플레이어 회전 (좌우 반전)
        if (horizontalInput != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(horizontalInput), 1, 1);
        }
    }
    
    private void Jump()
    {
        // 수직 방향으로 힘 적용
        rb.velocity = new Vector2(rb.velocity.x, 0); // 이전 점프 중첩 방지
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        Debug.Log("점프!");
    }
    
    private void OnDrawGizmosSelected()
    {
        // 지면 체크 영역 시각화
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
} 