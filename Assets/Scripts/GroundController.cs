using UnityEngine;

/// <summary>
/// 지면 오브젝트를 관리하는 컨트롤러 스크립트
/// </summary>
public class GroundController : MonoBehaviour
{
    private void Awake()
    {
        // Rigidbody2D 컴포넌트 확인
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        
        // Rigidbody2D가 있다면 설정 변경
        if (rb != null)
        {
            // 1. 정적 Rigidbody로 설정
            rb.bodyType = RigidbodyType2D.Static;
            rb.gravityScale = 0f;
            Debug.Log($"지면 '{gameObject.name}'의 Rigidbody2D가 정적으로 설정되었습니다.");
        }
        else
        {
            // 2. Collider만 확인
            Collider2D collider = GetComponent<Collider2D>();
            if (collider == null)
            {
                // Collider가 없으면 BoxCollider2D 추가
                BoxCollider2D boxCollider = gameObject.AddComponent<BoxCollider2D>();
                Debug.Log($"지면 '{gameObject.name}'에 BoxCollider2D가 추가되었습니다.");
            }
        }
        
        // 태그 설정
        if (gameObject.tag != "Ground")
        {
            gameObject.tag = "Ground";
            Debug.Log($"지면 '{gameObject.name}'의 태그가 'Ground'로 설정되었습니다.");
        }
        
        // 레이어 설정
        if (gameObject.layer != LayerMask.NameToLayer("Ground"))
        {
            // Ground 레이어가 없으면 기본 레이어 유지
            int groundLayer = LayerMask.NameToLayer("Ground");
            if (groundLayer != -1)
            {
                gameObject.layer = groundLayer;
                Debug.Log($"지면 '{gameObject.name}'의 레이어가 'Ground'로 설정되었습니다.");
            }
            else
            {
                Debug.LogWarning($"'Ground' 레이어가 프로젝트에 존재하지 않습니다. 레이어를 Unity 편집기에서 추가하세요.");
            }
        }
    }
    
    // 지면 데이터 시각화
    private void OnDrawGizmosSelected()
    {
        // 콜라이더 시각화
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            Gizmos.color = new Color(0, 1, 0, 0.3f); // 반투명 녹색
            
            // 콜라이더 유형에 따라 다르게 시각화
            if (collider is BoxCollider2D)
            {
                BoxCollider2D boxCollider = collider as BoxCollider2D;
                Vector3 center = transform.TransformPoint(boxCollider.offset);
                Vector3 size = new Vector3(
                    boxCollider.size.x * transform.lossyScale.x,
                    boxCollider.size.y * transform.lossyScale.y,
                    0.1f
                );
                Gizmos.DrawCube(center, size);
            }
            else if (collider is CircleCollider2D)
            {
                CircleCollider2D circleCollider = collider as CircleCollider2D;
                Vector3 center = transform.TransformPoint(circleCollider.offset);
                float radius = circleCollider.radius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.y);
                Gizmos.DrawSphere(center, radius);
            }
        }
    }
} 