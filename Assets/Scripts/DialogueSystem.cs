using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UGS; // Unity Google Sheets 네임스페이스
using GoogleSheet.Protocol.v2.Res;
using GoogleSheet.Protocol.v2.Req;
using GoogleSheet;
using System.IO;
using GoogleSheet.Type;
using System.Reflection;


public class DialogueSystem : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI dialogueText; // 대화 내용을 표시하는 TextMeshPro
    public GameObject dialoguePanel; // 대화창 패널
    public Button nextButton; // 다음 대화로 넘어가는 버튼
    public Button choice1Button; // 선택지 1 버튼
    public Button choice2Button; // 선택지 2 버튼

    [Header("Configuration")]
    public int currentDialogueIndex = 1000; // 현재 대화 인덱스
    public ImageLoader imageLoader; // 캐릭터 이미지 로더

    private Queue<DefaultTable.ScriptData> dialogues; // 대화 내용을 저장할 큐
    private List<DefaultTable.ScriptData> scriptDatas; // 스크립트 데이터 리스트
    private bool isDialogueActive; // 대화창 활성 상태

    private void Awake()
    {
        StartDialogue(); // 대화 시작
    }

    private void StartDialogue()
    {
        Initialize(); // 초기화
        LoadDialogues(); // 대화 데이터 로드
        RegisterButtonEvents(); // 버튼 이벤트 등록
        Debug.Log("대화 시스템 초기화 완료, 대화 시작 : " + dialogues.Count);
    }

    private void Update()
    {
        // 스페이스바 입력 시 DisplayNextDialogue 실행
        if (isDialogueActive && Input.GetKeyDown(KeyCode.Space))
        {
            DisplayNextDialogue();
        }

        // ESC 키 입력 시 대화 종료
        if (isDialogueActive && Input.GetKeyDown(KeyCode.Escape))
        {
            EndDialogue();
            closeChoice();
        }
    }

    private void Initialize()
    {
        dialogues = new Queue<DefaultTable.ScriptData>(); // 대화 큐 초기화
        dialoguePanel.SetActive(true); // 대화창 활성화
        isDialogueActive = false; // 대화 활성 상태 초기화
    }

    private void LoadDialogues()
    {
        DefaultTable.ScriptData.Load(); // Google Sheets에서 데이터 로드
        scriptDatas = new List<DefaultTable.ScriptData>(DefaultTable.ScriptData.ScriptDataList); // 데이터 리스트 초기화

        Debug.Log($"총 로드된 대화 개수: {scriptDatas.Count}");

        var filteredDatas = scriptDatas.FindAll(data => data.sceneIndex == currentDialogueIndex); // 현재 대화 인덱스에 맞는 데이터 필터링
        if (filteredDatas.Count == 0)
        {
            Debug.LogError("현재 대화 인덱스에 해당하는 대화가 없습니다.");
            return;
        }

        Debug.Log($"필터링된 대화 개수: {filteredDatas.Count}");
        InitiateDialogue(filteredDatas); // 대화 시작
    }

    private void RegisterButtonEvents()
    {
        if (nextButton != null)
        {
            nextButton.onClick.AddListener(DisplayNextDialogue); // 다음 대화 버튼 클릭 이벤트 등록
            Debug.Log("다음 버튼 이벤트 등록 완료.");
        }
        else
        {
            Debug.LogError("다음 버튼이 설정되지 않았습니다.");
        }
    }

    private void InitiateDialogue(List<DefaultTable.ScriptData> newDialogues)
    {
        if (newDialogues == null || newDialogues.Count == 0)
        {
            Debug.LogError("빈 리스트로 대화를 시작할 수 없습니다.");
            return;
        }

        dialogues.Clear(); // 기존 대화 큐 초기화
        foreach (var dialogue in newDialogues)
            dialogues.Enqueue(dialogue); // 새로운 대화 추가

        isDialogueActive = true; // 대화 활성화
        dialoguePanel.SetActive(true); // 대화창 활성화
        DisplayDialogue(dialogues.Peek()); // 첫 번째 대화 출력
    }

    public void DisplayNextDialogue()
    {
        if (dialogues.Count == 0)
        {
            Debug.Log("더 이상 남아있는 대화가 없습니다.");
            EndDialogue(); // 대화 종료
            return;
        }

        var currentDialogue = dialogues.Dequeue(); // 다음 대화 가져오기
        if (currentDialogue.status == "CHOICE")
        {
            Debug.Log("선택지 대화 출력" + currentDialogue.talk);
            DisplayChoiceDialogue(currentDialogue); // 선택지 처리
            return;
        }

        DisplayDialogue(currentDialogue); // 일반 대화 출력
    }
    private void DisplayDialogue(DefaultTable.ScriptData script)
    {
        if (script == null)
        {
            Debug.LogError("null 대화를 출력하려고 시도했습니다.");
            return;
        }

        dialogueText.text = script.talk; // 대화 내용 설정
        DisplayCharacterImage(script.name); // 캐릭터 이미지 출력
    }

    private void DisplayChoiceDialogue(DefaultTable.ScriptData choiceDialogue)
    {
        EndDialogue(); // 대화 종료

        string[] choices = choiceDialogue.talk.Split('>'); // 선택지 구분자 기준으로 나누기
        Debug.Log($"선택지 대화 출력: {choices[1]}, {choices[2]}");
        if (choices.Length != 3)
        {
            Debug.LogError("잘못된 선택지 대화 형식입니다.");
            return;
        }

        choice1Button.gameObject.SetActive(true); // 선택지 1 버튼 활성화
        choice2Button.gameObject.SetActive(true); // 선택지 2 버튼 활성화

        choice1Button.GetComponentInChildren<TextMeshProUGUI>().text = choices[1]; // 선택지 1 텍스트 설정
        choice2Button.GetComponentInChildren<TextMeshProUGUI>().text = choices[2]; // 선택지 2 텍스트 설정

        // 동적으로 리스너 추가 가능
        choice1Button.onClick.RemoveAllListeners();
        choice2Button.onClick.RemoveAllListeners();
        //choiceDialogue.property
        Debug.Log($"선택지 1: {choiceDialogue.property[0]}, 선택지 2: {choiceDialogue.property[1]}");

        choice1Button.onClick.AddListener(() => Debug.Log("Choice 1 Button Clicked"));
        choice2Button.onClick.AddListener(() => Debug.Log("Choice 2 Button Clicked"));

        choice1Button.onClick.AddListener(() => MoveToScene(choiceDialogue.property[0]));
        choice2Button.onClick.AddListener(() => MoveToScene(choiceDialogue.property[1]));

    }

    public void MoveToScene(int sceneIndex)
    {
        Debug.Log($"씬 이동: {sceneIndex}");
        // 씬 전환 로직 구현
        closeChoice();
        currentDialogueIndex = sceneIndex;
        StartDialogue();
    }

    void closeChoice()
    {
        choice1Button.onClick.RemoveAllListeners();
        choice2Button.onClick.RemoveAllListeners();
        choice1Button.gameObject.SetActive(false); // 선택지 1 버튼 활성화
        choice2Button.gameObject.SetActive(false); // 선택지 2 버튼 활성화
    }

    private void DisplayCharacterImage(string characterName)
    {
        if (imageLoader != null)
            imageLoader.LoadAndDisplayImage(characterName); // 캐릭터 이미지 로드 및 표시
        else
            Debug.LogError("ImageLoader가 설정되지 않았습니다.");
    }

    public void EndDialogue()
    {
        isDialogueActive = false; // 대화 비활성화
        dialoguePanel.SetActive(false); // 대화창 비활성화
    
        Debug.Log("대화가 종료되었습니다.");
    }
}
