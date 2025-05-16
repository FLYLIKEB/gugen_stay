using UnityEngine;

public class DoorController : MonoBehaviour
{
    public Transform targetPosition; // 카메라가 이동할 위치
    public CameraController cameraController; // CameraController 참조
    public PlayerMovement playerMovement; // PlayerMovement 참조
    private bool playerNearby = false; // 플레이어가 문 앞에 있는지 체크
    

    void OnMouseDown() // 방문 클릭 시 이동
    {
        MoveCameraAndPlayer();
    }

    void Update()
    {
        // 플레이어가 문 앞에 있을 때 엔터(Enter) 또는 스페이스바(Space) 입력 시 이동
        if (playerNearby && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space)))
        {
            MoveCameraAndPlayer();
        }
    }

    void MoveCameraAndPlayer()
    {
        if (targetPosition != null && cameraController != null && playerMovement != null) 
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
