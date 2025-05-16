using UnityEngine;
using UnityEngine.EventSystems;

public class MenuController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject menuPanel;

    private void Start()
    {
        menuPanel.SetActive(false); // 메뉴를 초기 상태에서 숨기기
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        menuPanel.SetActive(true); // 마우스가 UI에 진입하면 메뉴 표시
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        menuPanel.SetActive(false); // 마우스가 UI를 벗어나면 메뉴 숨김
    }

    
}