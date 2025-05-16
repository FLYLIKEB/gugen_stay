using UnityEngine;
using System.Collections.Generic;
using System;

public class DebugManager : MonoBehaviour
{
    public static DebugManager Instance { get; private set; }
    
    // 로그 저장 설정
    public bool saveLogsToFile = true;
    public bool showOnScreen = true;
    
    // 화면에 표시할 최대 로그 수
    public int maxLogsOnScreen = 10;
    
    // 로그 스타일
    private GUIStyle logStyle;
    private GUIStyle warningStyle;
    private GUIStyle errorStyle;
    
    // 로그 목록
    private List<LogEntry> logs = new List<LogEntry>();
    
    private struct LogEntry
    {
        public string message;
        public LogType type;
        public DateTime time;
    }
    
    private void Awake()
    {
        // 싱글톤 패턴
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        Debug.Log("DebugManager 초기화 완료");
        
        // 로그 콜백 등록
        Application.logMessageReceived += HandleLog;
    }
    
    private void OnDestroy()
    {
        // 콜백 해제
        Application.logMessageReceived -= HandleLog;
    }
    
    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        // 새 로그 항목 생성
        LogEntry entry = new LogEntry
        {
            message = logString,
            type = type,
            time = DateTime.Now
        };
        
        // 로그 목록에 추가
        logs.Add(entry);
        
        // 최대 로그 수 제한
        if (logs.Count > maxLogsOnScreen * 3)
        {
            logs.RemoveAt(0);
        }
        
        // 파일에 저장
        if (saveLogsToFile)
        {
            SaveLogToFile(entry);
        }
    }
    
    private void SaveLogToFile(LogEntry entry)
    {
        try
        {
            string path = Application.persistentDataPath + "/unity_debug_log.txt";
            string logMessage = $"[{entry.time:HH:mm:ss}] [{entry.type}] {entry.message}\n";
            System.IO.File.AppendAllText(path, logMessage);
        }
        catch (Exception e)
        {
            // 로그 저장 실패 시 콘솔에만 출력
            Debug.LogError($"로그 저장 실패: {e.Message}");
        }
    }
    
    private void OnGUI()
    {
        if (!showOnScreen) return;
        
        // 스타일 초기화
        if (logStyle == null)
        {
            logStyle = new GUIStyle(GUI.skin.label);
            logStyle.normal.textColor = Color.white;
            logStyle.fontSize = 16;
            
            warningStyle = new GUIStyle(logStyle);
            warningStyle.normal.textColor = Color.yellow;
            
            errorStyle = new GUIStyle(logStyle);
            errorStyle.normal.textColor = Color.red;
        }
        
        // 배경 박스
        GUI.Box(new Rect(10, 10, Screen.width - 20, 30 * maxLogsOnScreen), "");
        
        // 최근 로그 표시
        int count = Mathf.Min(logs.Count, maxLogsOnScreen);
        for (int i = 0; i < count; i++)
        {
            int index = logs.Count - count + i;
            LogEntry entry = logs[index];
            
            GUIStyle style = logStyle;
            switch (entry.type)
            {
                case LogType.Warning:
                    style = warningStyle;
                    break;
                case LogType.Error:
                case LogType.Exception:
                    style = errorStyle;
                    break;
            }
            
            string timeStr = entry.time.ToString("HH:mm:ss");
            GUI.Label(new Rect(20, 20 + 30 * i, Screen.width - 40, 25), $"[{timeStr}] {entry.message}", style);
        }
    }
    
    // 수동으로 로그 남기기
    public static void LogInfo(string message)
    {
        Debug.Log(message);
    }
    
    public static void LogWarning(string message)
    {
        Debug.LogWarning(message);
    }
    
    public static void LogError(string message)
    {
        Debug.LogError(message);
    }
} 