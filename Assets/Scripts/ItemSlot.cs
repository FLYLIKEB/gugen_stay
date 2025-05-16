using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IPointerClickHandler
{
    public Image itemImage;          // 아이템 이미지
    public TextMeshProUGUI quantityText; // 수량 텍스트
    public Button button;            // 슬롯 버튼
    
    private Item item;               // 슬롯에 있는 아이템
    
    private void Awake()
    {
        // 참조가 없는 경우 자동으로 가져오기
        if (button == null)
            button = GetComponent<Button>();
            
        if (itemImage == null)
            itemImage = transform.Find("ItemImage")?.GetComponent<Image>();
            
        if (itemImage == null)
        {
            // 이미지가 없으면 새로 생성
            GameObject imageObj = new GameObject("ItemImage");
            imageObj.transform.SetParent(transform, false);
            RectTransform imageRT = imageObj.AddComponent<RectTransform>();
            imageRT.anchorMin = Vector2.zero;
            imageRT.anchorMax = Vector2.one;
            imageRT.offsetMin = new Vector2(5, 5);
            imageRT.offsetMax = new Vector2(-5, -5);
            itemImage = imageObj.AddComponent<Image>();
        }
            
        if (quantityText == null)
            quantityText = transform.Find("QuantityText")?.GetComponent<TextMeshProUGUI>();
            
        if (quantityText == null)
        {
            // 수량 텍스트가 없으면 새로 생성
            GameObject textObj = new GameObject("QuantityText");
            textObj.transform.SetParent(transform, false);
            RectTransform textRT = textObj.AddComponent<RectTransform>();
            textRT.anchorMin = new Vector2(1, 0);
            textRT.anchorMax = new Vector2(1, 0);
            textRT.pivot = new Vector2(1, 0);
            textRT.sizeDelta = new Vector2(30, 20);
            textRT.anchoredPosition = new Vector2(-5, 5);
            quantityText = textObj.AddComponent<TextMeshProUGUI>();
            quantityText.fontSize = 12;
            quantityText.color = Color.white;
            quantityText.alignment = TextAlignmentOptions.Right;
        }
            
        // 초기 UI 상태 설정
        ClearSlot();
    }
    
    // 슬롯 업데이트
    public void UpdateSlot(Item newItem)
    {
        item = newItem;
        
        if (item != null)
        {
            if (itemImage != null)
            {
                itemImage.sprite = item.icon;
                itemImage.enabled = true;
                itemImage.preserveAspect = true;
            }
            
            // 스택 가능 아이템인 경우 수량 표시
            if (item.isStackable && item.quantity > 1 && quantityText != null)
            {
                quantityText.text = item.quantity.ToString();
                quantityText.gameObject.SetActive(true);
            }
            else if (quantityText != null)
            {
                quantityText.gameObject.SetActive(false);
            }
        }
        else
        {
            ClearSlot();
        }
    }
    
    // 슬롯 비우기
    public void ClearSlot()
    {
        item = null;
        
        if (itemImage != null)
        {
            itemImage.sprite = null;
            itemImage.enabled = false;
        }
        
        if (quantityText != null)
        {
            quantityText.gameObject.SetActive(false);
        }
    }
    
    // 포인터 클릭 핸들러 (이벤트 트리거를 위해)
    public void OnPointerClick(PointerEventData eventData)
    {
        // 버튼 클릭 이벤트가 있으면 수동으로 호출
        if (button != null)
        {
            button.onClick.Invoke();
        }
    }
} 