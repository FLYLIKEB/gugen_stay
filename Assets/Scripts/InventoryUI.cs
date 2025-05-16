using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    public GameObject inventoryPanel;           // 인벤토리 패널
    public GameObject slotPrefab;               // 슬롯 프리팹
    public Transform slotContainer;             // 슬롯 컨테이너
    
    // 아이템 정보 패널
    public GameObject itemInfoPanel;            // 아이템 정보 패널
    public TextMeshProUGUI itemNameText;        // 아이템 이름 텍스트
    public TextMeshProUGUI itemDescriptionText; // 아이템 설명 텍스트
    public Image itemImage;                     // 아이템 이미지
    public Button useButton;                    // 사용 버튼
    public Button dropButton;                   // 버리기 버튼
    
    private Inventory playerInventory;          // 플레이어 인벤토리
    private List<ItemSlot> itemSlots = new List<ItemSlot>(); // 아이템 슬롯 목록
    private int selectedSlotIndex = -1;         // 선택된 슬롯 인덱스
    
    private void Awake()
    {
        // 레퍼런스 확인
        CheckReferences();
    }
    
    private void OnEnable()
    {
        // UI가 활성화될 때마다 레퍼런스 확인
        CheckReferences();
    }
    
    private void CheckReferences()
    {
        // 아이템 정보 패널 레퍼런스 찾기
        if (itemInfoPanel == null)
        {
            itemInfoPanel = transform.Find("인벤토리패널/아이템정보패널")?.gameObject;
            if (itemInfoPanel == null)
            {
                Debug.LogError("아이템 정보 패널을 찾을 수 없습니다.");
            }
        }
        
        // 아이템 이름 텍스트 레퍼런스 찾기
        if (itemNameText == null && itemInfoPanel != null)
        {
            itemNameText = itemInfoPanel.transform.Find("아이템이름텍스트")?.GetComponent<TextMeshProUGUI>();
            if (itemNameText == null)
            {
                Debug.LogError("아이템 이름 텍스트를 찾을 수 없습니다.");
            }
        }
        
        // 아이템 설명 텍스트 레퍼런스 찾기
        if (itemDescriptionText == null && itemInfoPanel != null)
        {
            itemDescriptionText = itemInfoPanel.transform.Find("아이템설명텍스트")?.GetComponent<TextMeshProUGUI>();
            if (itemDescriptionText == null)
            {
                Debug.LogError("아이템 설명 텍스트를 찾을 수 없습니다.");
            }
        }
        
        // 아이템 이미지 레퍼런스 찾기
        if (itemImage == null && itemInfoPanel != null)
        {
            itemImage = itemInfoPanel.transform.Find("아이템이미지")?.GetComponent<Image>();
            if (itemImage == null)
            {
                Debug.LogError("아이템 이미지를 찾을 수 없습니다.");
            }
        }
        
        // 사용 버튼 레퍼런스 찾기
        if (useButton == null && itemInfoPanel != null)
        {
            useButton = itemInfoPanel.transform.Find("사용버튼")?.GetComponent<Button>();
            if (useButton == null)
            {
                Debug.LogError("사용 버튼을 찾을 수 없습니다.");
            }
        }
        
        // 버리기 버튼 레퍼런스 찾기
        if (dropButton == null && itemInfoPanel != null)
        {
            dropButton = itemInfoPanel.transform.Find("버리기버튼")?.GetComponent<Button>();
            if (dropButton == null)
            {
                Debug.LogError("버리기 버튼을 찾을 수 없습니다.");
            }
        }
    }
    
    private void Start()
    {
        Debug.Log("인벤토리 UI 시작");
        
        // 플레이어 인벤토리 참조 가져오기
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerInventory = player.GetComponent<Inventory>();
            if (playerInventory != null)
            {
                // 인벤토리 변경 이벤트 등록
                playerInventory.OnInventoryChanged += UpdateUI;
                Debug.Log("플레이어 인벤토리 찾음: " + player.name);
            }
            else
            {
                Debug.LogError("플레이어에 Inventory 컴포넌트가 없습니다.");
            }
        }
        else
        {
            Debug.LogError("Player 태그를 가진 게임 오브젝트를 찾을 수 없습니다.");
        }
        
        // 초기 UI 생성
        CreateSlots();
        
        // 초기 상태 설정
        if (inventoryPanel != null)
            inventoryPanel.SetActive(false);
            
        if (itemInfoPanel != null)
            itemInfoPanel.SetActive(false);
        
        // 버튼 이벤트 등록
        if (useButton != null)
        {
            useButton.onClick.RemoveAllListeners();
            useButton.onClick.AddListener(UseSelectedItem);
        }
        
        if (dropButton != null)
        {
            dropButton.onClick.RemoveAllListeners();
            dropButton.onClick.AddListener(DropSelectedItem);
        }
        
        // 레퍼런스 확인
        Debug.Log("슬롯 프리팹: " + (slotPrefab != null ? "있음" : "없음"));
        Debug.Log("슬롯 컨테이너: " + (slotContainer != null ? "있음" : "없음"));
        Debug.Log("아이템 정보 패널: " + (itemInfoPanel != null ? "있음" : "없음"));
    }
    
    private void Update()
    {
        // I 키를 눌러 인벤토리 열기/닫기
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }
        
        // ESC 키를 눌러 인벤토리 닫기
        if (Input.GetKeyDown(KeyCode.Escape) && inventoryPanel != null && inventoryPanel.activeSelf)
        {
            CloseInventory();
        }
    }
    
    // 인벤토리 열기/닫기 토글
    public void ToggleInventory()
    {
        Debug.Log("인벤토리 토글");
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(!inventoryPanel.activeSelf);
            
            if (!inventoryPanel.activeSelf && itemInfoPanel != null)
            {
                itemInfoPanel.SetActive(false);
            }
            
            // 열 때 인벤토리 UI 업데이트
            if (inventoryPanel.activeSelf && playerInventory != null)
            {
                UpdateUI(playerInventory.items);
            }
        }
        else
        {
            Debug.LogError("인벤토리 패널이 없습니다.");
        }
    }
    
    // 인벤토리 닫기
    public void CloseInventory()
    {
        if (inventoryPanel != null)
            inventoryPanel.SetActive(false);
            
        if (itemInfoPanel != null)
            itemInfoPanel.SetActive(false);
    }
    
    // 슬롯 생성
    private void CreateSlots()
    {
        if (slotContainer == null || slotPrefab == null || playerInventory == null)
        {
            Debug.LogError("슬롯 생성에 필요한 참조가 없습니다.");
            return;
        }
        
        // 기존 슬롯 모두 제거
        foreach (Transform child in slotContainer)
        {
            Destroy(child.gameObject);
        }
        
        itemSlots.Clear();
        
        // 인벤토리 크기에 맞게 슬롯 생성
        for (int i = 0; i < playerInventory.items.Count; i++)
        {
            GameObject slotGO = Instantiate(slotPrefab, slotContainer);
            ItemSlot slot = slotGO.GetComponent<ItemSlot>();
            
            if (slot != null)
            {
                // 슬롯 인덱스 설정
                int index = i;
                
                // 버튼 참조 확인
                if (slot.button == null)
                {
                    slot.button = slotGO.GetComponent<Button>();
                    
                    if (slot.button == null)
                    {
                        Debug.LogError($"슬롯 {i}에 Button 컴포넌트가 없습니다.");
                        continue;
                    }
                }
                
                // 클릭 이벤트 등록
                slot.button.onClick.RemoveAllListeners(); // 기존 리스너 제거
                int slotIndex = i; // 클로저 문제 해결을 위해 복사
                slot.button.onClick.AddListener(() => {
                    Debug.Log($"슬롯 {slotIndex} 클릭됨");
                    SelectSlot(slotIndex);
                });
                
                itemSlots.Add(slot);
            }
            else
            {
                Debug.LogError($"슬롯 {i}에 ItemSlot 컴포넌트가 없습니다.");
            }
        }
        
        Debug.Log($"슬롯 {itemSlots.Count}개 생성 완료");
    }
    
    // UI 업데이트
    public void UpdateUI(List<Item> items)
    {
        Debug.Log($"인벤토리 UI 업데이트: 아이템 {items.Count}개, 슬롯 {itemSlots.Count}개");
        
        if (itemSlots.Count == 0 && items.Count > 0)
        {
            Debug.Log("슬롯이 없어서 다시 생성합니다.");
            CreateSlots();
        }
        
        for (int i = 0; i < itemSlots.Count; i++)
        {
            if (i < items.Count)
            {
                Item item = items[i];
                if (itemSlots[i] != null)
                {
                    itemSlots[i].UpdateSlot(item);
                    
                    if (item != null)
                    {
                        Debug.Log($"슬롯 {i}: {item.itemName} ({item.quantity})");
                    }
                }
            }
        }
        
        // 선택된 슬롯이 있으면 정보 패널 업데이트
        if (selectedSlotIndex >= 0 && selectedSlotIndex < items.Count && items[selectedSlotIndex] != null)
        {
            UpdateItemInfoPanel(items[selectedSlotIndex]);
        }
        else if (itemInfoPanel != null)
        {
            itemInfoPanel.SetActive(false);
        }
    }
    
    // 아이템 정보 패널 업데이트
    private void UpdateItemInfoPanel(Item item)
    {
        if (item == null || itemInfoPanel == null) return;
        
        itemInfoPanel.SetActive(true);
        
        if (itemNameText != null)
            itemNameText.text = item.itemName;
            
        if (itemDescriptionText != null)
            itemDescriptionText.text = item.description;
            
        if (itemImage != null)
        {
            itemImage.sprite = item.icon;
            itemImage.enabled = true;
        }
        
        if (useButton != null)
        {
            useButton.interactable = (item.itemType == Item.ItemType.Consumable || 
                                      item.itemType == Item.ItemType.Equipment);
        }
    }
    
    // 슬롯 선택
    private void SelectSlot(int index)
    {
        Debug.Log($"슬롯 {index} 선택됨");
        
        if (index < 0 || index >= playerInventory.items.Count)
        {
            Debug.LogError("잘못된 슬롯 인덱스입니다.");
            return;
        }
        
        selectedSlotIndex = index;
        Item selectedItem = playerInventory.items[index];
        
        // 선택한 슬롯에 아이템이 있으면 정보 패널 표시
        if (selectedItem != null)
        {
            Debug.Log($"선택된 아이템: {selectedItem.itemName}");
            UpdateItemInfoPanel(selectedItem);
        }
        else
        {
            Debug.Log("빈 슬롯 선택됨");
            if (itemInfoPanel != null)
                itemInfoPanel.SetActive(false);
        }
    }
    
    // 선택한 아이템 사용
    private void UseSelectedItem()
    {
        if (playerInventory != null && selectedSlotIndex >= 0 && 
            selectedSlotIndex < playerInventory.items.Count && 
            playerInventory.items[selectedSlotIndex] != null)
        {
            Debug.Log($"UI에서 아이템 사용: {playerInventory.items[selectedSlotIndex].itemName}");
            
            // 사용 전 아이템 정보 백업
            string itemName = playerInventory.items[selectedSlotIndex].itemName;
            int prevQuantity = playerInventory.items[selectedSlotIndex].quantity;
            
            // 아이템 사용 실행
            playerInventory.UseItem(selectedSlotIndex);
            
            // 사용 후 UI 업데이트
            if (playerInventory.items[selectedSlotIndex] == null)
            {
                Debug.Log($"{itemName} 아이템이 모두 소진되었습니다.");
                
                if (itemInfoPanel != null)
                    itemInfoPanel.SetActive(false);
                
                // 인벤토리 내용 확인 (디버깅)
                playerInventory.PrintInventoryContents();
            }
            else
            {
                Debug.Log($"{itemName} 아이템이 사용되었습니다. 남은 수량: {playerInventory.items[selectedSlotIndex].quantity} (이전: {prevQuantity})");
                
                // 정보 패널 업데이트
                UpdateItemInfoPanel(playerInventory.items[selectedSlotIndex]);
            }
        }
        else
        {
            Debug.LogWarning("사용할 아이템이 없습니다.");
        }
    }
    
    // 선택한 아이템 버리기
    private void DropSelectedItem()
    {
        if (playerInventory != null && selectedSlotIndex >= 0 && 
            selectedSlotIndex < playerInventory.items.Count && 
            playerInventory.items[selectedSlotIndex] != null)
        {
            Debug.Log($"UI에서 아이템 버리기: {playerInventory.items[selectedSlotIndex].itemName}");
            
            // 아이템 정보 백업
            string itemName = playerInventory.items[selectedSlotIndex].itemName;
            
            // RemoveItem 대신 DropItem 사용
            bool dropped = playerInventory.DropItem(selectedSlotIndex);
            
            if (dropped)
            {
                Debug.Log($"{itemName} 아이템이 버려졌습니다.");
                
                if (itemInfoPanel != null)
                    itemInfoPanel.SetActive(false);
                
                // 인벤토리 내용 확인 (디버깅)
                playerInventory.PrintInventoryContents();
            }
            else
            {
                Debug.LogWarning($"{itemName} 아이템을 버리는데 실패했습니다.");
            }
        }
        else
        {
            Debug.LogWarning("버릴 아이템이 없습니다.");
        }
    }
} 