using UnityEngine;

// 카메라가 처음 설정된 위치에서 플레이어를 따라가는 스크립트
// Main Camera에 추가
public class CameraFollow : MonoBehaviour
{
    [Header("따라갈 대상")]
    public Transform target; // Player 드래그

    public float smoothSpeed = 8f; // 따라가는 부드러움

    private Vector3 offset; // 시작 시 자동 계산

    private void Start()
    {
        if (target == null) return;
        // 현재 카메라 위치와 플레이어 위치의 차이를 오프셋으로 저장
        offset = transform.position - target.position;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 targetPosition = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
    }
}