using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Collections.Generic;

public class InventoryUIGenerator : EditorWindow
{
    private GameObject itemSlotPrefab;
    private Sprite defaultItemIcon;
    
    [MenuItem("도구/인벤토리 UI 생성")]
    public static void ShowWindow()
    {
        GetWindow<InventoryUIGenerator>("인벤토리 UI 생성기");
    }
    
    private void OnGUI()
    {
        GUILayout.Label("인벤토리 UI 생성기", EditorStyles.boldLabel);
        
        EditorGUILayout.Space();
        
        itemSlotPrefab = (GameObject)EditorGUILayout.ObjectField("아이템 슬롯 프리팹", itemSlotPrefab, typeof(GameObject), false);
        defaultItemIcon = (Sprite)EditorGUILayout.ObjectField("기본 아이템 아이콘", defaultItemIcon, typeof(Sprite), false);
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("인벤토리 UI 생성"))
        {
            CreateInventoryUI();
        }
        
        if (GUILayout.Button("아이템 슬롯 프리팹 생성"))
        {
            CreateItemSlotPrefab();
        }
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("아이템 매니저 생성"))
        {
            CreateItemManager();
        }
        
        if (GUILayout.Button("테스트 아이템 생성"))
        {
            CreateTestItem();
        }
    }
    
    private void CreateInventoryUI()
    {
        // 캔버스 생성
        GameObject canvasObj = new GameObject("인벤토리UI");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();
        
        // 인벤토리 패널 생성
        GameObject inventoryPanel = CreateUIElement("인벤토리패널", canvasObj);
        RectTransform inventoryPanelRT = inventoryPanel.GetComponent<RectTransform>();
        inventoryPanelRT.anchorMin = new Vector2(0.5f, 0.5f);
        inventoryPanelRT.anchorMax = new Vector2(0.5f, 0.5f);
        inventoryPanelRT.pivot = new Vector2(0.5f, 0.5f);
        inventoryPanelRT.sizeDelta = new Vector2(600, 400);
        
        Image inventoryPanelImage = inventoryPanel.GetComponent<Image>();
        inventoryPanelImage.color = new Color(0, 0, 0, 0.8f);
        
        // 슬롯 컨테이너 생성
        GameObject slotContainer = CreateUIElement("슬롯컨테이너", inventoryPanel);
        RectTransform slotContainerRT = slotContainer.GetComponent<RectTransform>();
        slotContainerRT.anchorMin = new Vector2(0, 0);
        slotContainerRT.anchorMax = new Vector2(1, 1);
        slotContainerRT.offsetMin = new Vector2(20, 100);
        slotContainerRT.offsetMax = new Vector2(-20, -20);
        
        GridLayoutGroup gridLayout = slotContainer.AddComponent<GridLayoutGroup>();
        gridLayout.cellSize = new Vector2(80, 80);
        gridLayout.spacing = new Vector2(10, 10);
        gridLayout.startCorner = GridLayoutGroup.Corner.UpperLeft;
        gridLayout.startAxis = GridLayoutGroup.Axis.Horizontal;
        gridLayout.childAlignment = TextAnchor.UpperLeft;
        gridLayout.constraint = GridLayoutGroup.Constraint.Flexible;
        
        // 아이템 정보 패널 생성
        GameObject itemInfoPanel = CreateUIElement("아이템정보패널", inventoryPanel);
        RectTransform itemInfoPanelRT = itemInfoPanel.GetComponent<RectTransform>();
        itemInfoPanelRT.anchorMin = new Vector2(0, 0);
        itemInfoPanelRT.anchorMax = new Vector2(1, 0);
        itemInfoPanelRT.pivot = new Vector2(0.5f, 0);
        itemInfoPanelRT.sizeDelta = new Vector2(0, 80);
        itemInfoPanelRT.offsetMin = new Vector2(10, 10);
        itemInfoPanelRT.offsetMax = new Vector2(-10, 90);
        
        Image itemInfoPanelImage = itemInfoPanel.GetComponent<Image>();
        itemInfoPanelImage.color = new Color(0.2f, 0.2f, 0.2f, 1f);
        
        // 아이템 이미지 추가
        GameObject itemImage = CreateUIElement("아이템이미지", itemInfoPanel);
        RectTransform itemImageRT = itemImage.GetComponent<RectTransform>();
        itemImageRT.anchorMin = new Vector2(0, 0.5f);
        itemImageRT.anchorMax = new Vector2(0, 0.5f);
        itemImageRT.pivot = new Vector2(0, 0.5f);
        itemImageRT.sizeDelta = new Vector2(60, 60);
        itemImageRT.anchoredPosition = new Vector2(10, 0);
        
        Image itemImageComponent = itemImage.GetComponent<Image>();
        itemImageComponent.preserveAspect = true;
        
        // 아이템 이름 텍스트 추가
        GameObject itemNameText = new GameObject("아이템이름텍스트");
        itemNameText.transform.SetParent(itemInfoPanel.transform, false);
        RectTransform itemNameTextRT = itemNameText.AddComponent<RectTransform>();
        itemNameTextRT.anchorMin = new Vector2(0, 1);
        itemNameTextRT.anchorMax = new Vector2(1, 1);
        itemNameTextRT.pivot = new Vector2(0.5f, 1);
        itemNameTextRT.offsetMin = new Vector2(80, -35);
        itemNameTextRT.offsetMax = new Vector2(-150, -5);
        
        TextMeshProUGUI nameText = itemNameText.AddComponent<TextMeshProUGUI>();
        nameText.fontSize = 16;
        nameText.color = Color.white;
        nameText.alignment = TextAlignmentOptions.Left;
        nameText.enableWordWrapping = false;
        
        // 아이템 설명 텍스트 추가
        GameObject itemDescText = new GameObject("아이템설명텍스트");
        itemDescText.transform.SetParent(itemInfoPanel.transform, false);
        RectTransform itemDescTextRT = itemDescText.AddComponent<RectTransform>();
        itemDescTextRT.anchorMin = new Vector2(0, 0);
        itemDescTextRT.anchorMax = new Vector2(0.7f, 1);
        itemDescTextRT.pivot = new Vector2(0.5f, 0.5f);
        itemDescTextRT.offsetMin = new Vector2(80, 5);
        itemDescTextRT.offsetMax = new Vector2(0, -35);
        
        TextMeshProUGUI descText = itemDescText.AddComponent<TextMeshProUGUI>();
        descText.fontSize = 12;
        descText.color = new Color(0.8f, 0.8f, 0.8f);
        descText.alignment = TextAlignmentOptions.TopLeft;
        descText.enableWordWrapping = true;
        
        // 사용 버튼 추가
        GameObject useButton = CreateButton("사용버튼", itemInfoPanel, "사용", new Color(0, 0.5f, 1f, 0.8f));
        RectTransform useButtonRT = useButton.GetComponent<RectTransform>();
        useButtonRT.anchorMin = new Vector2(1, 1);
        useButtonRT.anchorMax = new Vector2(1, 1);
        useButtonRT.pivot = new Vector2(1, 1);
        useButtonRT.sizeDelta = new Vector2(80, 30);
        useButtonRT.anchoredPosition = new Vector2(-10, -10);
        
        // 버리기 버튼 추가
        GameObject dropButton = CreateButton("버리기버튼", itemInfoPanel, "버리기", new Color(1f, 0.3f, 0.3f, 0.8f));
        RectTransform dropButtonRT = dropButton.GetComponent<RectTransform>();
        dropButtonRT.anchorMin = new Vector2(1, 0);
        dropButtonRT.anchorMax = new Vector2(1, 0);
        dropButtonRT.pivot = new Vector2(1, 0);
        dropButtonRT.sizeDelta = new Vector2(80, 30);
        dropButtonRT.anchoredPosition = new Vector2(-10, 10);
        
        // Inventory UI 스크립트 추가 및 설정
        InventoryUI inventoryUI = canvasObj.AddComponent<InventoryUI>();
        inventoryUI.inventoryPanel = inventoryPanel;
        inventoryUI.slotContainer = slotContainer.transform;
        inventoryUI.itemInfoPanel = itemInfoPanel;
        inventoryUI.itemNameText = nameText;
        inventoryUI.itemDescriptionText = descText;
        inventoryUI.itemImage = itemImageComponent;
        inventoryUI.useButton = useButton.GetComponent<Button>();
        inventoryUI.dropButton = dropButton.GetComponent<Button>();
        
        if (itemSlotPrefab != null)
        {
            inventoryUI.slotPrefab = itemSlotPrefab;
        }
        else
        {
            Debug.LogError("아이템 슬롯 프리팹이 설정되지 않았습니다. '아이템 슬롯 프리팹 생성' 버튼을 먼저 클릭하세요.");
        }
        
        // 초기 상태 설정
        inventoryPanel.SetActive(false);
        itemInfoPanel.SetActive(false);
        
        Debug.Log("인벤토리 UI가 성공적으로 생성되었습니다!");
    }
    
    private GameObject CreateUIElement(string name, GameObject parent)
    {
        GameObject element = new GameObject(name);
        element.transform.SetParent(parent.transform, false);
        element.AddComponent<RectTransform>();
        element.AddComponent<Image>();
        return element;
    }
    
    private GameObject CreateButton(string name, GameObject parent, string text, Color color)
    {
        GameObject button = new GameObject(name);
        button.transform.SetParent(parent.transform, false);
        RectTransform rt = button.AddComponent<RectTransform>();
        
        Image image = button.AddComponent<Image>();
        image.color = color;
        
        Button btn = button.AddComponent<Button>();
        ColorBlock colors = btn.colors;
        colors.normalColor = color;
        colors.highlightedColor = new Color(color.r + 0.1f, color.g + 0.1f, color.b + 0.1f, 1f);
        colors.pressedColor = new Color(color.r - 0.1f, color.g - 0.1f, color.b - 0.1f, 1f);
        btn.colors = colors;
        
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(button.transform, false);
        RectTransform textRT = textObj.AddComponent<RectTransform>();
        textRT.anchorMin = Vector2.zero;
        textRT.anchorMax = Vector2.one;
        textRT.sizeDelta = Vector2.zero;
        
        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 14;
        tmp.color = Color.white;
        tmp.alignment = TextAlignmentOptions.Center;
        
        return button;
    }
    
    private void CreateItemSlotPrefab()
    {
        // 슬롯 프리팹 생성
        GameObject slotPrefab = new GameObject("ItemSlot");
        RectTransform slotRT = slotPrefab.AddComponent<RectTransform>();
        slotRT.sizeDelta = new Vector2(80, 80);
        
        Image slotImage = slotPrefab.AddComponent<Image>();
        slotImage.color = new Color(0.3f, 0.3f, 0.3f, 0.8f);
        
        Button slotButton = slotPrefab.AddComponent<Button>();
        ColorBlock colors = slotButton.colors;
        colors.normalColor = new Color(0.3f, 0.3f, 0.3f, 0.8f);
        colors.highlightedColor = new Color(0.4f, 0.4f, 0.4f, 0.8f);
        colors.pressedColor = new Color(0.2f, 0.2f, 0.2f, 0.8f);
        slotButton.colors = colors;
        
        // 아이템 이미지 추가
        GameObject itemImageObj = new GameObject("ItemImage");
        itemImageObj.transform.SetParent(slotPrefab.transform, false);
        RectTransform itemImageRT = itemImageObj.AddComponent<RectTransform>();
        itemImageRT.anchorMin = Vector2.zero;
        itemImageRT.anchorMax = Vector2.one;
        itemImageRT.offsetMin = new Vector2(5, 5);
        itemImageRT.offsetMax = new Vector2(-5, -5);
        
        Image itemImage = itemImageObj.AddComponent<Image>();
        itemImage.color = Color.white;
        itemImage.preserveAspect = true;
        
        // 아이템 수량 텍스트 추가
        GameObject quantityTextObj = new GameObject("QuantityText");
        quantityTextObj.transform.SetParent(slotPrefab.transform, false);
        RectTransform quantityTextRT = quantityTextObj.AddComponent<RectTransform>();
        quantityTextRT.anchorMin = new Vector2(1, 0);
        quantityTextRT.anchorMax = new Vector2(1, 0);
        quantityTextRT.pivot = new Vector2(1, 0);
        quantityTextRT.sizeDelta = new Vector2(30, 20);
        quantityTextRT.anchoredPosition = new Vector2(-5, 5);
        
        TextMeshProUGUI quantityText = quantityTextObj.AddComponent<TextMeshProUGUI>();
        quantityText.fontSize = 12;
        quantityText.color = Color.white;
        quantityText.alignment = TextAlignmentOptions.Right;
        
        // ItemSlot 스크립트 추가
        ItemSlot itemSlot = slotPrefab.AddComponent<ItemSlot>();
        itemSlot.itemImage = itemImage;
        itemSlot.quantityText = quantityText;
        itemSlot.button = slotButton;
        
        // 프리팹 저장
        if (!Directory.Exists("Assets/Prefabs"))
        {
            AssetDatabase.CreateFolder("Assets", "Prefabs");
        }
        
        PrefabUtility.SaveAsPrefabAsset(slotPrefab, "Assets/Prefabs/ItemSlot.prefab", out bool success);
        
        if (success)
        {
            Debug.Log("아이템 슬롯 프리팹이 Assets/Prefabs/ItemSlot.prefab에 저장되었습니다!");
            itemSlotPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/ItemSlot.prefab");
        }
        
        DestroyImmediate(slotPrefab);
    }
    
    private void CreateItemManager()
    {
        // 아이템 매니저 오브젝트 생성
        GameObject itemManagerObj = new GameObject("ItemManager");
        ItemManager itemManager = itemManagerObj.AddComponent<ItemManager>();
        
        // 테스트 아이템 몇 개 생성
        if (defaultItemIcon != null)
        {
            List<Item> testItems = new List<Item>
            {
                new Item("힐링 포션", "체력을 회복합니다.", defaultItemIcon, Item.ItemType.Consumable, true, 1),
                new Item("철 검", "기본적인 무기입니다.", defaultItemIcon, Item.ItemType.Equipment, false, 1),
                new Item("열쇠", "어딘가의 문을 열 수 있습니다.", defaultItemIcon, Item.ItemType.Quest, false, 1),
                new Item("금화", "게임 내 화폐입니다.", defaultItemIcon, Item.ItemType.Misc, true, 10)
            };
            
            itemManager.allItems = testItems;
        }
        else
        {
            Debug.LogWarning("기본 아이템 아이콘이 설정되지 않았습니다. 기본 아이콘을 선택하세요.");
        }
        
        Debug.Log("아이템 매니저가 생성되었습니다!");
    }
    
    private void CreateTestItem()
    {
        if (defaultItemIcon == null)
        {
            Debug.LogError("기본 아이템 아이콘이 필요합니다.");
            return;
        }
        
        // 테스트 아이템 생성
        GameObject testItemObj = new GameObject("TestItem");
        SpriteRenderer spriteRenderer = testItemObj.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = defaultItemIcon;
        
        BoxCollider2D collider = testItemObj.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        
        ItemObject itemObject = testItemObj.AddComponent<ItemObject>();
        
        // 테스트 아이템 데이터 설정
        Item testItem = new Item("테스트 아이템", "이것은 테스트 아이템입니다.", defaultItemIcon, Item.ItemType.Misc, true, 1);
        itemObject.item = testItem;
        
        testItemObj.tag = "Item";
        
        Debug.Log("테스트 아이템이 생성되었습니다!");
    }
} 