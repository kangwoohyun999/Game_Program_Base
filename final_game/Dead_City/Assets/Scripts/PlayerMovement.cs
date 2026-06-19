using UnityEngine;

// 플레이어 이동 + 마우스 방향 회전 (7주차 - 이동/충돌, 15주차 - 레이캐스트)
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;

    private Animator playerAnimator;
    private PlayerInput playerInput;
    private Rigidbody playerRigidbody;
    private Camera mainCamera;

    private void Start()
    {
        playerInput     = GetComponent<PlayerInput>();
        playerRigidbody = GetComponent<Rigidbody>();
        playerAnimator  = GetComponent<Animator>();
        mainCamera      = Camera.main;
    }

    private void FixedUpdate()
    {
        Move();
        RotateTowardsMouse();

        if (playerAnimator != null)
            playerAnimator.SetFloat("Move", playerInput.move);
    }

    private void Move()
    {
        // 카메라 기준 방향 계산
        // 카메라 forward/right에서 Y축 성분 제거 후 정규화
        Vector3 camForward = mainCamera.transform.forward;
        Vector3 camRight   = mainCamera.transform.right;
        camForward.y = 0f;
        camRight.y   = 0f;
        camForward.Normalize();
        camRight.Normalize();

        // W/S → 카메라 앞뒤, A/D → 카메라 좌우
        Vector3 moveDir = (camForward * playerInput.move + camRight * playerInput.rotate).normalized;
        Vector3 moveDistance = moveDir * moveSpeed * Time.deltaTime;

        playerRigidbody.MovePosition(playerRigidbody.position + moveDistance);
    }

    // 마우스 커서 방향으로 회전 (레이캐스트 - 15주차)
    private void RotateTowardsMouse()
    {
        if (mainCamera == null) return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f, LayerMask.GetMask("Ground")))
        {
            Vector3 lookTarget = hit.point;
            lookTarget.y = transform.position.y;

            Vector3 dir = lookTarget - transform.position;
            if (dir.sqrMagnitude > 0.001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(dir);
                playerRigidbody.rotation = Quaternion.Slerp(
                    playerRigidbody.rotation, targetRot, 20f * Time.deltaTime);
            }
        }
    }
}
