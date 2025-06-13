using UnityEngine;
using System.Collections;
using UGS;
using DefaultTable;
using System.Linq;
using System.Collections.Generic;

public class NPCController : MonoBehaviour
{
    [Header("NPC 기본 설정")]
    public string npcKey; // NPCData의 key 값
    public float interactionDistance = 2f;
    public bool isInteractable = true;

    [Header("NPC 애니메이션")]
    public Animator animator;
    private bool isTalking = false;
    private NPCData npcData;
    private bool hasTalkingAnimation = false;

    [Header("말풍선 설정")]
    public SpeechBubble speechBubble;
    public Vector3 bubbleOffset = new Vector3(0, 1.5f, 0);
    private float lastTalkTime = 0f;
    private float talkCooldown = 5f; // 말풍선 표시 간격

    private void Start()
    {
        // NPC 태그 설정
        gameObject.tag = "NPC";

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        // Animator 파라미터 확인
        if (animator != null)
        {
            foreach (AnimatorControllerParameter param in animator.parameters)
            {
                if (param.name == "IsTalking")
                {
                    hasTalkingAnimation = true;
                    break;
                }
            }
        }

        // 말풍선이 없으면 찾기
        if (speechBubble == null)
        {
            speechBubble = GetComponentInChildren<SpeechBubble>();
        }

        // NPCData 로드
        LoadNPCData();
    }

    private void LoadNPCData()
    {
        // NPCData 로드 확인
        if (!DefaultTable.NPCData.GetList().Any())
        {
            Debug.LogWarning("NPCData가 비어있습니다. Google Sheets에서 데이터를 로드합니다.");
            DefaultTable.NPCData.Load(true); // 강제 리로드
        }

        // NPCData에서 해당 key를 가진 NPC 찾기
        npcData = DefaultTable.NPCData.GetList().Find(npc => npc.key == npcKey);
        if (npcData == null)
        {
            Debug.LogError($"NPC with key '{npcKey}' not found in NPCData!");
        }
        else
        {
            Debug.Log($"NPC 데이터 로드 성공: {npcData.key}, displayName: {npcData.displayName}");
        }
    }

    private void Update()
    {
        // 플레이어와의 거리 확인
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            
            // 상호작용 가능 거리 내에 있고 상호작용 키를 눌렀을 때
            if (distance <= interactionDistance && Input.GetKeyDown(KeyCode.E) && isInteractable)
            {
                StartDialogue();
            }

            // 플레이어를 향해 회전
            if (distance <= interactionDistance)
            {
                LookAtPlayer();
            }

            // 말풍선 위치 업데이트
            if (speechBubble != null)
            {
                speechBubble.transform.position = transform.position + bubbleOffset;
            }
        }
    }

    public void StartDialogue()
    {
        if (!isTalking && npcData != null)
        {
            isTalking = true;
            if (animator != null && hasTalkingAnimation)
            {
                animator.SetBool("IsTalking", true);
            }

            // ScriptData 로드 확인
            if (!DefaultTable.ScriptData.GetList().Any())
            {
                Debug.LogWarning("ScriptData가 비어있습니다. Google Sheets에서 데이터를 로드합니다.");
                DefaultTable.ScriptData.Load(true); // 강제 리로드
            }

            // NPC의 대화 내용을 가져와서 대화 시작
            var scriptData = DefaultTable.ScriptData.GetList()
                .FindAll(script => script.name == npcData.key)
                .OrderBy(script => script.index)
                .ToList();

            if (scriptData.Count > 0)
            {
                // 랜덤 대화 선택
                var randomScript = scriptData[Random.Range(0, scriptData.Count)];
                ShowSpeechBubble(randomScript.talk);
            }
            else
            {
                // 기본 대화 표시
                ShowSpeechBubble($"안녕하세요! 저는 {npcData.displayName}입니다.");
            }

            // 대화 종료
            StartCoroutine(EndDialogueAfterDelay(3f));
        }
    }

    private void ShowSpeechBubble(string message)
    {
        if (speechBubble != null && Time.time - lastTalkTime >= talkCooldown)
        {
            speechBubble.ShowMessage(message);
            lastTalkTime = Time.time;
        }
    }

    private IEnumerator EndDialogueAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        EndDialogue();
    }

    public void EndDialogue()
    {
        isTalking = false;
        if (animator != null && hasTalkingAnimation)
        {
            animator.SetBool("IsTalking", false);
        }
    }

    // NPC가 바라보는 방향을 플레이어 쪽으로 회전
    public void LookAtPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Vector3 direction = player.transform.position - transform.position;
            direction.y = 0; // Y축 회전만 적용
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }
        }
    }

    // 디버그용 시각화
    private void OnDrawGizmosSelected()
    {
        // 상호작용 가능 거리 표시
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
} 