using UnityEngine;
using UnityEditor;
using System.IO;

public class TextureGenerator
{
    [MenuItem("도구/기본 아이템 아이콘 생성")]
    public static void GenerateDefaultItemIcon()
    {
        // 폴더 확인 및 생성
        if (!Directory.Exists("Assets/Resources"))
        {
            Directory.CreateDirectory("Assets/Resources");
        }
        
        // 기본 아이콘 텍스처 생성
        Texture2D texture = new Texture2D(64, 64, TextureFormat.RGBA32, false);
        
        // 텍스처 배경 색 채우기 (반투명 회색)
        Color bgColor = new Color(0.7f, 0.7f, 0.7f, 1.0f);
        
        // 모든 픽셀을 배경색으로 초기화
        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                texture.SetPixel(x, y, bgColor);
            }
        }
        
        // 테두리 그리기 (진한 회색)
        Color borderColor = new Color(0.3f, 0.3f, 0.3f, 1.0f);
        int borderWidth = 2;
        
        // 상하 테두리
        for (int x = 0; x < texture.width; x++)
        {
            for (int b = 0; b < borderWidth; b++)
            {
                texture.SetPixel(x, b, borderColor);
                texture.SetPixel(x, texture.height - 1 - b, borderColor);
            }
        }
        
        // 좌우 테두리
        for (int y = 0; y < texture.height; y++)
        {
            for (int b = 0; b < borderWidth; b++)
            {
                texture.SetPixel(b, y, borderColor);
                texture.SetPixel(texture.width - 1 - b, y, borderColor);
            }
        }
        
        // 밝은 부분 추가 (그라데이션)
        Color lightColor = new Color(1.0f, 1.0f, 1.0f, 0.5f);
        for (int y = borderWidth + 2; y < texture.height / 2; y++)
        {
            float t = (float)y / (texture.height / 2);
            Color gradientColor = Color.Lerp(lightColor, bgColor, t);
            
            for (int x = borderWidth + 2; x < texture.width - borderWidth - 2; x++)
            {
                texture.SetPixel(x, y, gradientColor);
            }
        }
        
        // 변경사항 적용
        texture.Apply();
        
        // 텍스처를 png 파일로 저장
        byte[] bytes = texture.EncodeToPNG();
        string filePath = "Assets/Resources/DefaultItemIcon.png";
        File.WriteAllBytes(filePath, bytes);
        
        // 에셋 데이터베이스 리프레시
        AssetDatabase.Refresh();
        
        // 스프라이트 설정
        TextureImporter importer = AssetImporter.GetAtPath(filePath) as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spritePixelsPerUnit = 100;
            importer.filterMode = FilterMode.Point;
            importer.SaveAndReimport();
        }
        
        Debug.Log("기본 아이템 아이콘이 생성되었습니다: " + filePath);
    }
} 