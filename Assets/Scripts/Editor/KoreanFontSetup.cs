using UnityEngine;
using UnityEditor;
using TMPro;

public class KoreanFontSetup : EditorWindow
{
    [MenuItem("Tools/한글 폰트 설정")]
    public static void ShowWindow()
    {
        GetWindow<KoreanFontSetup>("한글 폰트 설정");
    }

    void OnGUI()
    {
        GUILayout.Label("한글 TextMeshPro 폰트 설정", EditorStyles.boldLabel);
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("기본 TextMeshPro 리소스 임포트"))
        {
            // TMP Essential Resources 임포트
            string packagePath = "Packages/com.unity.textmeshpro/Package Resources/TMP Essential Resources.unitypackage";
            AssetDatabase.ImportPackage(packagePath, false);
            Debug.Log("TextMeshPro Essential Resources를 임포트했습니다.");
        }
        
        GUILayout.Space(5);
        
        if (GUILayout.Button("한글 폰트 에셋 생성"))
        {
            CreateKoreanFont();
        }
        
        GUILayout.Space(10);
        
        EditorGUILayout.HelpBox(
            "1. 먼저 '기본 TextMeshPro 리소스 임포트'를 클릭하세요.\n" +
            "2. 프로젝트에 .ttf 한글 폰트 파일이 있다면 '한글 폰트 에셋 생성'을 클릭하세요.\n" +
            "3. Window > TextMeshPro > Font Asset Creator에서 수동으로 폰트를 생성할 수도 있습니다.",
            MessageType.Info
        );
    }
    
    void CreateKoreanFont()
    {
        // 프로젝트에서 한글 폰트 파일 찾기
        string[] fontGuids = AssetDatabase.FindAssets("t:Font");
        
        foreach (string guid in fontGuids)
        {
            string fontPath = AssetDatabase.GUIDToAssetPath(guid);
            Font font = AssetDatabase.LoadAssetAtPath<Font>(fontPath);
            
            if (font != null && (fontPath.Contains("스타더스트") || fontPath.Contains("Korean")))
            {
                Debug.Log($"한글 폰트 발견: {font.name}");
                
                // Font Asset Creator 열기
                EditorApplication.ExecuteMenuItem("Window/TextMeshPro/Font Asset Creator");
                
                break;
            }
        }
        
        if (fontGuids.Length == 0)
        {
            Debug.LogWarning("한글 폰트 파일을 찾을 수 없습니다. 프로젝트에 .ttf 한글 폰트 파일을 추가해주세요.");
        }
    }
} 