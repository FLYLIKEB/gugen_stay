using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class LayerSetup : EditorWindow
{
    [MenuItem("Tools/레이어 설정")]
    public static void ShowWindow()
    {
        GetWindow<LayerSetup>("레이어 설정");
    }
    
    private void OnGUI()
    {
        GUILayout.Label("필수 레이어 설정", EditorStyles.boldLabel);
        
        if (GUILayout.Button("레이어 추가 및 충돌 설정"))
        {
            SetupLayers();
        }
        
        if (GUILayout.Button("모든 지면 오브젝트 태그/레이어 설정"))
        {
            SetupGroundObjects();
        }
    }
    
    private void SetupLayers()
    {
        // Ground 레이어가 있는지 확인
        int groundLayer = LayerMask.NameToLayer("Ground");
        
        if (groundLayer == -1)
        {
            Debug.LogWarning("Ground 레이어가 없습니다. Unity 에디터 메뉴의 Edit > Project Settings > Tags and Layers에서 추가해주세요.");
            
            if (EditorUtility.DisplayDialog("레이어 추가 안내", 
                "Ground 레이어가 없습니다. Project Settings를 열어서 추가하시겠습니까?", 
                "설정 열기", "취소"))
            {
                // 태그 및 레이어 설정 창 열기
                EditorApplication.ExecuteMenuItem("Edit/Project Settings...");
            }
            
            return;
        }
        
        // Player 레이어가 있는지 확인
        int playerLayer = LayerMask.NameToLayer("Player");
        
        if (playerLayer == -1)
        {
            Debug.LogWarning("Player 레이어가 없습니다. Unity 에디터 메뉴의 Edit > Project Settings > Tags and Layers에서 추가해주세요.");
            
            if (EditorUtility.DisplayDialog("레이어 추가 안내", 
                "Player 레이어가 없습니다. Project Settings를 열어서 추가하시겠습니까?", 
                "설정 열기", "취소"))
            {
                // 태그 및 레이어 설정 창 열기
                EditorApplication.ExecuteMenuItem("Edit/Project Settings...");
            }
            
            return;
        }
        
        Debug.Log("레이어 확인 완료: Ground 레이어 = " + groundLayer + ", Player 레이어 = " + playerLayer);
    }
    
    private void SetupGroundObjects()
    {
        int groundLayer = LayerMask.NameToLayer("Ground");
        
        if (groundLayer == -1)
        {
            Debug.LogError("Ground 레이어가 없습니다. 먼저 레이어를 추가해주세요.");
            return;
        }
        
        // Ground 태그를 가진 모든 오브젝트 찾기
        GameObject[] groundObjects = GameObject.FindGameObjectsWithTag("Ground");
        
        if (groundObjects.Length == 0)
        {
            // 이름으로 대신 찾기
            List<GameObject> possibleGrounds = new List<GameObject>();
            
            // 씬의 모든 루트 오브젝트 찾기
            GameObject[] rootObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
            
            foreach (GameObject root in rootObjects)
            {
                // 이름에 "ground", "floor", "platform" 등이 포함된 오브젝트 찾기
                if (root.name.ToLower().Contains("ground") || 
                    root.name.ToLower().Contains("floor") || 
                    root.name.ToLower().Contains("platform") ||
                    root.name.ToLower().Contains("땅") ||
                    root.name.ToLower().Contains("바닥") ||
                    root.name.ToLower().Contains("플랫폼"))
                {
                    possibleGrounds.Add(root);
                }
                
                // 자식 오브젝트 확인
                Transform[] children = root.GetComponentsInChildren<Transform>();
                foreach (Transform child in children)
                {
                    if (child.gameObject != root && (
                        child.name.ToLower().Contains("ground") || 
                        child.name.ToLower().Contains("floor") || 
                        child.name.ToLower().Contains("platform") ||
                        child.name.ToLower().Contains("땅") ||
                        child.name.ToLower().Contains("바닥") ||
                        child.name.ToLower().Contains("플랫폼")))
                    {
                        possibleGrounds.Add(child.gameObject);
                    }
                }
            }
            
            if (possibleGrounds.Count > 0)
            {
                Debug.Log("태그는 없지만 이름으로 " + possibleGrounds.Count + "개의 지면 추정 오브젝트를 찾았습니다.");
                
                foreach (GameObject obj in possibleGrounds)
                {
                    obj.tag = "Ground";
                    obj.layer = groundLayer;
                    
                    // Collider2D 확인
                    Collider2D collider = obj.GetComponent<Collider2D>();
                    if (collider == null)
                    {
                        // 콜라이더가 없으면 BoxCollider2D 추가
                        obj.AddComponent<BoxCollider2D>();
                    }
                    
                    // GroundController 추가
                    if (obj.GetComponent<GroundController>() == null)
                    {
                        obj.AddComponent<GroundController>();
                    }
                    
                    Debug.Log("지면 설정 완료: " + obj.name);
                }
            }
            else
            {
                Debug.LogWarning("지면으로 인식할 수 있는 오브젝트를 찾지 못했습니다.");
            }
        }
        else
        {
            Debug.Log(groundObjects.Length + "개의 Ground 태그 오브젝트를 찾았습니다.");
            
            foreach (GameObject obj in groundObjects)
            {
                obj.layer = groundLayer;
                
                // Collider2D 확인
                Collider2D collider = obj.GetComponent<Collider2D>();
                if (collider == null)
                {
                    // 콜라이더가 없으면 BoxCollider2D 추가
                    obj.AddComponent<BoxCollider2D>();
                }
                
                // GroundController 추가
                if (obj.GetComponent<GroundController>() == null)
                {
                    obj.AddComponent<GroundController>();
                }
                
                Debug.Log("지면 설정 완료: " + obj.name);
            }
        }
    }
} 