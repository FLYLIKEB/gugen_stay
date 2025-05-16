using UnityEngine;

public class ItemObject : MonoBehaviour
{
    public Item item;  // 이 오브젝트가 나타내는 아이템
    
    private void Start()
    {
        // 아이템 태그 설정
        if (gameObject.tag != "Item")
        {
            gameObject.tag = "Item";
        }
        
        // 스프라이트 설정
        UpdateSprite();
    }
    
    // 스프라이트 업데이트
    private void UpdateSprite()
    {
        if (item != null && item.icon != null)
        {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = item.icon;
                spriteRenderer.sortingOrder = 10; // 다른 오브젝트보다 위에 표시
            }
            else
            {
                // SpriteRenderer가 없으면 추가
                spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = item.icon;
                spriteRenderer.sortingOrder = 10;
            }
        }
        else
        {
            Debug.LogWarning("아이템 또는 아이콘이 없습니다.", this);
        }
    }
    
    // 아이템 설정 메서드
    public void SetItem(Item newItem)
    {
        if (newItem == null)
        {
            Debug.LogError("null 아이템으로 설정 시도", this);
            return;
        }
        
        item = newItem.Clone(); // 복제본 사용
        Debug.Log($"아이템 설정: {item.itemName}", this);
        
        // 스프라이트 업데이트
        UpdateSprite();
    }
    
    // 아이템 드롭 메서드 (위치 지정)
    public static ItemObject DropItem(Item item, Vector3 position)
    {
        if (item == null)
        {
            Debug.LogError("null 아이템을 드롭할 수 없습니다.");
            return null;
        }
        
        Debug.Log($"아이템 드롭 시도: {item.itemName} (위치: {position})");
        
        try
        {
            // 아이템 오브젝트 생성
            GameObject itemObject = new GameObject($"Item_{item.itemName}");
            itemObject.transform.position = position;
            itemObject.tag = "Item";
            
            // 스프라이트 렌더러 추가
            SpriteRenderer spriteRenderer = itemObject.AddComponent<SpriteRenderer>();
            if (item.icon != null)
            {
                spriteRenderer.sprite = item.icon;
                spriteRenderer.sortingOrder = 10; // 다른 오브젝트보다 위에 표시
            }
            else
            {
                Debug.LogWarning($"아이템의 아이콘이 없습니다: {item.itemName}");
                // 기본 스프라이트 (흰색 사각형)
                Texture2D tempTexture = new Texture2D(32, 32);
                Color color = new Color(1, 1, 1);
                for (int y = 0; y < tempTexture.height; y++)
                {
                    for (int x = 0; x < tempTexture.width; x++)
                    {
                        tempTexture.SetPixel(x, y, color);
                    }
                }
                tempTexture.Apply();
                
                Sprite defaultSprite = Sprite.Create(
                    tempTexture, 
                    new Rect(0, 0, tempTexture.width, tempTexture.height),
                    Vector2.one * 0.5f
                );
                
                spriteRenderer.sprite = defaultSprite;
            }
            
            // 콜라이더 추가
            BoxCollider2D collider = itemObject.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
            collider.size = new Vector2(0.5f, 0.5f); // 적절한 크기 설정
            
            // 리지드바디 추가 (물리적 상호작용 가능)
            Rigidbody2D rb = itemObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0.5f; // 가벼운 중력 효과
            rb.mass = 0.1f;
            rb.drag = 1f;
            rb.angularDrag = 0.5f;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            
            // 약간의 랜덤 힘 적용 (튀어나가는 효과)
            Vector2 randomForce = new Vector2(Random.Range(-1f, 1f), Random.Range(1f, 2f));
            rb.AddForce(randomForce, ForceMode2D.Impulse);
            
            // 아이템 오브젝트 컴포넌트 추가 및 설정
            ItemObject itemObjectComponent = itemObject.AddComponent<ItemObject>();
            itemObjectComponent.item = item.Clone(); // 복제본 사용
            
            Debug.Log($"아이템 드롭 성공: {item.itemName}");
            return itemObjectComponent;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"아이템 드롭 중 오류 발생: {e.Message}\n{e.StackTrace}");
            return null;
        }
    }
    
    // 아이템 습득 로직
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"플레이어가 아이템과 접촉: {item?.itemName}");
            // 여기서 자동 습득을 원한다면 해당 로직 추가
        }
    }
} 