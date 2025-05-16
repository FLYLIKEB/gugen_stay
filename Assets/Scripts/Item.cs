using UnityEngine;

[System.Serializable]
public class Item
{
    public string itemName;     // 아이템 이름
    public string description;  // 아이템 설명
    public Sprite icon;         // 아이템 아이콘
    public ItemType itemType;   // 아이템 유형
    public bool isStackable;    // 중첩 가능 여부
    public int quantity;        // 수량

    // 아이템 유형 열거형
    public enum ItemType
    {
        Equipment,  // 장비
        Consumable, // 소모품
        Quest,      // 퀘스트 아이템
        Misc        // 기타
    }

    // 생성자
    public Item(string name, string desc, Sprite itemIcon, ItemType type, bool stackable = false, int qty = 1)
    {
        itemName = name;
        description = desc;
        icon = itemIcon;
        itemType = type;
        isStackable = stackable;
        quantity = qty;
    }

    // 아이템 복제 메서드
    public Item Clone()
    {
        return new Item(itemName, description, icon, itemType, isStackable, quantity);
    }
} 