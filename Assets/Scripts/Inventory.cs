using System.Collections.Generic;
using UnityEngine;
using System;

public class Inventory : MonoBehaviour
{
    [SerializeField]
    private int inventorySize = 20; // 인벤토리 최대 크기
    
    public List<Item> items = new List<Item>(); // 아이템 목록
    
    // 아이템 추가/제거 이벤트 (UI 업데이트용)
    public event Action<List<Item>> OnInventoryChanged;
    
    private void Awake()
    {
        // 인벤토리 초기화
        InitializeInventory();
    }
    
    private void InitializeInventory()
    {
        // 기존 아이템 모두 제거
        items.Clear();
        
        // 인벤토리 크기에 맞게 null 항목 추가
        for (int i = 0; i < inventorySize; i++)
        {
            items.Add(null);
        }
        
        // 초기화 후 이벤트 발생
        OnInventoryChanged?.Invoke(items);
        
        Debug.Log("인벤토리 초기화 완료: " + inventorySize + "개의 슬롯");
    }
    
    // 아이템 추가 메서드
    public bool AddItem(Item item)
    {
        if (item == null) return false;
        
        Debug.Log($"아이템 추가 시도: {item.itemName} (수량: {item.quantity})");
        
        // 중첩 가능한 아이템인 경우 기존 아이템에 추가
        if (item.isStackable)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i] != null && items[i].itemName == item.itemName)
                {
                    items[i].quantity += item.quantity;
                    Debug.Log($"기존 아이템에 추가: {items[i].itemName} (총 수량: {items[i].quantity})");
                    OnInventoryChanged?.Invoke(items);
                    return true;
                }
            }
        }
        
        // 빈 슬롯 찾기
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] == null)
            {
                items[i] = item.Clone();
                Debug.Log($"새 슬롯에 추가됨: {items[i].itemName} (슬롯: {i})");
                OnInventoryChanged?.Invoke(items);
                return true;
            }
        }
        
        // 인벤토리가 꽉 찬 경우
        Debug.Log("인벤토리가 가득 찼습니다.");
        return false;
    }
    
    // 아이템 제거 메서드
    public bool RemoveItem(int index)
    {
        if (index < 0 || index >= items.Count) 
        {
            Debug.LogError($"잘못된 인벤토리 인덱스: {index}");
            return false;
        }
        
        if (items[index] != null)
        {
            Debug.Log($"아이템 제거: {items[index].itemName} (슬롯: {index})");
            items[index] = null;
            OnInventoryChanged?.Invoke(items);
            return true;
        }
        
        Debug.Log($"제거할 아이템이 없습니다 (슬롯: {index})");
        return false;
    }
    
    // 특정 이름의 아이템 제거
    public bool RemoveItem(string itemName, int quantity = 1)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] != null && items[i].itemName == itemName)
            {
                if (items[i].quantity > quantity)
                {
                    items[i].quantity -= quantity;
                    Debug.Log($"아이템 수량 감소: {itemName} (남은 수량: {items[i].quantity})");
                    OnInventoryChanged?.Invoke(items);
                    return true;
                }
                else
                {
                    Debug.Log($"아이템 완전히 제거: {itemName}");
                    items[i] = null;
                    OnInventoryChanged?.Invoke(items);
                    return true;
                }
            }
        }
        
        Debug.Log($"제거할 아이템을 찾을 수 없음: {itemName}");
        return false;
    }
    
    // 특정 아이템 사용 메서드
    public void UseItem(int index)
    {
        if (index < 0 || index >= items.Count)
        {
            Debug.LogError($"잘못된 인벤토리 인덱스: {index}");
            return;
        }
        
        Item item = items[index];
        if (item != null)
        {
            Debug.Log($"아이템 사용 시도: {item.itemName} (슬롯: {index})");
            
            bool itemUsed = false;
            
            // 아이템 유형에 따른 기능 처리
            switch (item.itemType)
            {
                case Item.ItemType.Consumable:
                    // 소모품 사용 효과 (여기서는 로그만 출력)
                    Debug.Log($"{item.itemName} 아이템을 사용했습니다. 효과가 적용됩니다!");
                    itemUsed = true;
                    break;
                    
                case Item.ItemType.Equipment:
                    // 장비 착용 처리
                    Debug.Log($"{item.itemName} 장비를 착용했습니다!");
                    itemUsed = true;
                    break;
                    
                default:
                    Debug.Log($"{item.itemName}은(는) 사용할 수 없는 아이템입니다.");
                    break;
            }
            
            // 사용된 아이템 처리
            if (itemUsed)
            {
                // 소모품인 경우 수량 감소
                if (item.itemType == Item.ItemType.Consumable)
                {
                    if (item.quantity > 1)
                    {
                        item.quantity--;
                        Debug.Log($"아이템 수량 감소: {item.itemName} (남은 수량: {item.quantity})");
                    }
                    else
                    {
                        Debug.Log($"아이템을 모두 사용했습니다: {item.itemName}");
                        items[index] = null;
                    }
                }
                
                // 인벤토리 변경 이벤트 발생
                OnInventoryChanged?.Invoke(items);
            }
        }
        else
        {
            Debug.Log($"사용할 아이템이 없습니다 (슬롯: {index})");
        }
    }
    
    // 아이템을 게임 월드에 드롭하는 메서드
    public bool DropItem(int index)
    {
        if (index < 0 || index >= items.Count)
        {
            Debug.LogError($"잘못된 인벤토리 인덱스: {index}");
            return false;
        }
        
        Item itemToDrop = items[index];
        if (itemToDrop != null)
        {
            Debug.Log($"아이템 드롭: {itemToDrop.itemName} (슬롯: {index})");
            
            try
            {
                // 플레이어 앞쪽에 아이템 생성
                Vector3 dropPosition = transform.position + transform.forward * 1.5f;
                
                // 아이템 오브젝트 생성
                ItemObject droppedItem = ItemObject.DropItem(itemToDrop, dropPosition);
                
                if (droppedItem != null)
                {
                    Debug.Log($"아이템이 성공적으로 드롭되었습니다: {itemToDrop.itemName}");
                }
                else
                {
                    Debug.LogWarning($"아이템 오브젝트 생성 실패: {itemToDrop.itemName}");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"아이템 드롭 중 오류 발생: {e.Message}");
            }
            
            // 인벤토리에서 아이템 제거 (드롭 성공 여부와 상관없이)
            items[index] = null;
            OnInventoryChanged?.Invoke(items);
            
            return true;
        }
        
        Debug.Log($"드롭할 아이템이 없습니다 (슬롯: {index})");
        return false;
    }
    
    // 디버깅용: 인벤토리 내용 출력
    public void PrintInventoryContents()
    {
        Debug.Log("=== 인벤토리 내용 ===");
        int itemCount = 0;
        
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] != null)
            {
                itemCount++;
                Debug.Log($"슬롯 {i}: {items[i].itemName} x{items[i].quantity} ({items[i].itemType})");
            }
        }
        
        Debug.Log($"총 아이템 수: {itemCount}/{items.Count}");
    }
} 