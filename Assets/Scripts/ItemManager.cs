using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static ItemManager Instance { get; private set; }
    
    // 모든 아이템 목록
    public List<Item> allItems = new List<Item>();
    
    // 아이템 데이터 저장용 딕셔너리
    private Dictionary<string, Item> itemDictionary = new Dictionary<string, Item>();
    
    private void Awake()
    {
        // 싱글톤 패턴 구현
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeItems();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    // 아이템 초기화
    private void InitializeItems()
    {
        // 딕셔너리 초기화
        itemDictionary.Clear();
        
        // 모든 아이템을 딕셔너리에 추가
        foreach (Item item in allItems)
        {
            if (item != null && !itemDictionary.ContainsKey(item.itemName))
            {
                itemDictionary.Add(item.itemName, item);
            }
        }
    }
    
    // 이름으로 아이템 가져오기
    public Item GetItemByName(string itemName)
    {
        if (itemDictionary.TryGetValue(itemName, out Item item))
        {
            return item.Clone(); // 원본 아이템의 복제본 반환
        }
        
        Debug.LogWarning($"아이템을 찾을 수 없습니다: {itemName}");
        return null;
    }
    
    // 월드에 아이템 생성
    public ItemObject SpawnItem(string itemName, Vector3 position)
    {
        Item item = GetItemByName(itemName);
        if (item == null) return null;
        
        return ItemObject.DropItem(item, position);
    }
    
    // 아이템 생성 (랜덤)
    public ItemObject SpawnRandomItem(Vector3 position)
    {
        if (allItems.Count == 0) return null;
        
        // 랜덤 아이템 선택
        int randomIndex = Random.Range(0, allItems.Count);
        Item randomItem = allItems[randomIndex];
        
        return ItemObject.DropItem(randomItem, position);
    }
} 