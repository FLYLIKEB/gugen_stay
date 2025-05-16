using UnityEngine;

public class PortalController : MonoBehaviour
{
    public Transform targetPositionUp; // 카메라가 이동할 위치
    public Transform targetPositionDown; // 카메라가 이동할 위치
    public PlayerMovement playerMovement; // PlayerMovement 참조
    private bool playerNearby = false; // 플레이어가 문 앞에 있는지 체크
    
    void Update()
    {
        if (playerNearby)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                MoveCameraAndPlayer(targetPositionUp); // 위 방향키 입력 시 위로 이동
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                MoveCameraAndPlayer(targetPositionDown); // 아래 방향키 입력 시 아래로 이동
            }
        }
    }


    void MoveCameraAndPlayer(Transform targetPosition)
    {
        if (targetPosition != null && playerMovement != null) 
        {
            playerMovement.TeleportTo(targetPosition); // 🎯 순간이동 기능 호출
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // 플레이어가 닿으면 true
        {
            playerNearby = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // 플레이어가 떠나면 false
        {
            playerNearby = false;
        }
    }
}
