using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float cameraSpeed = 1000f;
    public GameObject player;
    protected Transform targetPosition = null; // 목표 위치
    protected bool isMovingToTarget = false; // 목표로 이동 중인지 체크
    private float moveSpeed; // 이동 속도 저장

    private void LateUpdate() // LateUpdate 사용 -> 플레이어 이동 후 카메라가 따라오도록 설정
    {
        if (player != null)
        {
            // ⭐ 플레이어를 따라가는 기능 추가 (부드러운 카메라 이동)
            Vector3 targetPos = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, targetPos, cameraSpeed * Time.deltaTime);
        }
    }
}