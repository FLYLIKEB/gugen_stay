using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpeechBubble : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI speechText;
    public Image bubbleImage;
    public CanvasGroup canvasGroup;

    [Header("Animation Settings")]
    public float fadeInDuration = 0.3f;
    public float fadeOutDuration = 0.3f;
    public float displayDuration = 3f;

    private void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
        
        // 초기 상태 설정
        canvasGroup.alpha = 0f;
    }

    public void ShowMessage(string message)
    {
        StopAllCoroutines();
        speechText.text = message;
        StartCoroutine(ShowAndHideMessage());
    }

    private System.Collections.IEnumerator ShowAndHideMessage()
    {
        // 페이드 인
        float elapsedTime = 0f;
        while (elapsedTime < fadeInDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeInDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1f;

        // 메시지 표시 시간
        yield return new WaitForSeconds(displayDuration);

        // 페이드 아웃
        elapsedTime = 0f;
        while (elapsedTime < fadeOutDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeOutDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 0f;
    }
} 