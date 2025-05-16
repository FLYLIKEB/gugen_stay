using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

// 씬이 로드될 때 자동으로 DebugManager를 추가하는 스크립트
[DefaultExecutionOrder(-100)]
public class DebugManagerSetup : MonoBehaviour
{
    private static DebugManagerSetup instance;
    
    private void Awake()
    {
        // 싱글톤 패턴
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        instance = this;
        DontDestroyOnLoad(gameObject);
        
        // 로그 테스트 메시지
        Debug.Log("DebugManagerSetup 초기화 완료");
        Debug.Log("현재 씬: " + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        Debug.Log("지면 인식 문제 디버깅 시작 - 현재 시간 " + System.DateTime.Now.ToString("HH:mm:ss"));
        
        // DebugManager가 존재하지 않으면 생성
        if (FindObjectOfType<DebugManager>() == null)
        {
            GameObject debugManagerObj = new GameObject("DebugManager");
            debugManagerObj.AddComponent<DebugManager>();
            DontDestroyOnLoad(debugManagerObj);
            Debug.Log("DebugManager 객체가 자동으로 생성되었습니다.");
        }
    }
    
    private void Start()
    {
        // 추가적인 디버그 정보 로깅
        Debug.Log("게임 시작 - 시스템 정보:");
        Debug.Log("플랫폼: " + Application.platform);
        Debug.Log("Unity 버전: " + Application.unityVersion);
        Debug.Log("프레임레이트: " + Application.targetFrameRate);
        
        // 카메라 정보 로깅
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            Debug.Log("메인 카메라: " + mainCamera.name);
            Debug.Log("카메라 설정 - 직교: " + mainCamera.orthographic + ", 크기: " + mainCamera.orthographicSize);
        }
        else
        {
            Debug.LogWarning("메인 카메라를 찾을 수 없습니다.");
        }
        
        // 플레이어 정보 로깅
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Debug.Log("플레이어 오브젝트: " + player.name);
            Debug.Log("플레이어 위치: " + player.transform.position);
            
            // 컴포넌트 확인
            Collider2D collider = player.GetComponent<Collider2D>();
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            
            if (collider != null)
                Debug.Log("플레이어 콜라이더: " + collider.GetType().Name + ", 활성화: " + collider.enabled);
            
            if (rb != null)
                Debug.Log("플레이어 Rigidbody: 질량=" + rb.mass + ", 중력=" + rb.gravityScale + ", 제약=" + rb.constraints);
        }
        else
        {
            Debug.LogWarning("Player 태그를 가진 오브젝트를 찾을 수 없습니다.");
        }
        
        // 지면 오브젝트 확인
        GameObject[] grounds = GameObject.FindGameObjectsWithTag("Ground");
        Debug.Log("지면 오브젝트 수: " + grounds.Length);
        
        for (int i = 0; i < grounds.Length; i++)
        {
            GameObject ground = grounds[i];
            Debug.Log($"지면 #{i+1}: {ground.name}, 위치: {ground.transform.position}, 레이어: {LayerMask.LayerToName(ground.layer)}");
            
            Collider2D groundCollider = ground.GetComponent<Collider2D>();
            if (groundCollider != null)
                Debug.Log($"지면 #{i+1} 콜라이더: {groundCollider.GetType().Name}, 활성화: {groundCollider.enabled}");
            else
                Debug.LogWarning($"지면 #{i+1}에 콜라이더가 없습니다!");
        }
    }

#if UNITY_EDITOR
    // 에디터 메뉴 항목 추가
    [MenuItem("Tools/디버그/디버그 매니저 생성")]
    private static void CreateDebugManager()
    {
        // 이미 존재하는지 확인
        DebugManager existingManager = FindObjectOfType<DebugManager>();
        
        if (existingManager != null)
        {
            Debug.Log("DebugManager가 이미 존재합니다: " + existingManager.gameObject.name);
            Selection.activeGameObject = existingManager.gameObject;
            return;
        }
        
        // DebugManager 생성
        GameObject debugManagerObj = new GameObject("DebugManager");
        debugManagerObj.AddComponent<DebugManager>();
        
        // 선택
        Selection.activeGameObject = debugManagerObj;
        
        Debug.Log("DebugManager가 생성되었습니다.");
        
        // 현재 씬 저장
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
    }
    
    // 에디터 메뉴 항목 추가 
    [MenuItem("Tools/디버그/로그 확인")]
    private static void OpenLogFile()
    {
        string logPath = Application.persistentDataPath + "/unity_debug_log.txt";
        
        if (System.IO.File.Exists(logPath))
        {
            System.Diagnostics.Process.Start(logPath);
        }
        else
        {
            Debug.LogWarning("로그 파일이 존재하지 않습니다: " + logPath);
        }
    }
#endif
} 