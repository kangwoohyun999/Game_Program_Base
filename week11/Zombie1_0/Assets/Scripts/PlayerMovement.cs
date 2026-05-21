using UnityEngine;

// 플레이어 캐릭터를 사용자 입력에 따라 움직이는 스크립트
public class PlayerMovement : MonoBehaviour 
{
    public float moveSpeed = 5f;        // 기본 이동 속도
    public float rotateSpeed = 180f;    // 좌우 회전 속도

    [Header("전력질주 설정")]
    public float sprintMultiplier = 2f;     // 속도 배율
    public float sprintMaxDuration = 5f;    // 최대 지속 시간 (5초)

    [Header("구르기 설정")]
    public float rollDistance = 5f;         // 구르기 이동 거리
    public float rollDuration = 0.45f;      // 구르기 소요 시간
    public AnimationCurve rollCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private PlayerInput playerInput;
    private Rigidbody playerRigidbody;
    private Animator playerAnimator;

    // 전력질주 변수
    private float sprintTimer = 0f;
    private bool isSprinting = false;

    // 구르기 변수
    private bool isRolling = false;
    private float rollTimer = 0f;
    private Vector3 rollDirection;

    private void Start() 
    {
        playerInput = GetComponent<PlayerInput>();
        playerRigidbody = GetComponent<Rigidbody>();
        playerAnimator = GetComponent<Animator>();
    }

    private void Update()
    {
        // 구르기 입력
        if (playerInput.roll && !isRolling)
        {
            StartRoll();
        }

        // 전력질주 타이머 처리
        HandleSprint();
    }

    // FixedUpdate는 물리 갱신 주기에 맞춰 실행됨
    private void FixedUpdate() 
    {
        if (isRolling)
        {
            PerformRoll();
            return;
        }

        Rotate();
        Move();

        playerAnimator.SetFloat("Move", playerInput.move);
    }

    // 전력질주 처리
    private void HandleSprint()
    {
        if (playerInput.sprint && !isRolling)
        {
            if (!isSprinting)
            {
                isSprinting = true;
                sprintTimer = 0f;
            }

            sprintTimer += Time.deltaTime;

            // 5초 초과하면 강제 종료
            if (sprintTimer >= sprintMaxDuration)
            {
                isSprinting = false;
            }
        }
        else
        {
            isSprinting = false;
            sprintTimer = 0f;
        }
    }

    // 입력값에 따라 캐릭터를 앞뒤로 움직임
    private void Move() 
    {
        float currentSpeed = moveSpeed;
        if (isSprinting) currentSpeed *= sprintMultiplier;

        Vector3 moveDistance = playerInput.move * transform.forward * currentSpeed * Time.deltaTime;
        playerRigidbody.MovePosition(playerRigidbody.position + moveDistance);
    }

    // 입력값에 따라 캐릭터를 좌우로 회전
    private void Rotate() 
    {
        float turn = playerInput.rotate * rotateSpeed * Time.deltaTime;
        playerRigidbody.rotation = playerRigidbody.rotation * Quaternion.Euler(0, turn, 0f);
    }

    // ==================== 구르기 관련 ====================

    private void StartRoll()
    {
        isRolling = true;
        rollTimer = 0f;
        rollDirection = transform.forward;

        // 구르기 애니메이션 트리거 (Animator에 "Roll" 트리거 추가 필요)
        if (playerAnimator != null)
            playerAnimator.SetTrigger("Roll");

        // 구르는 동안 물리 충돌 영향 최소화
        playerRigidbody.linearVelocity = Vector3.zero;
    }

    private void PerformRoll()
    {
        rollTimer += Time.deltaTime;
        float normalized = Mathf.Clamp01(rollTimer / rollDuration);

        if (normalized >= 1f)
        {
            isRolling = false;
            return;
        }

        float distanceThisFrame = rollDistance * rollCurve.Evaluate(normalized) * Time.deltaTime / rollDuration;
        Vector3 moveThisFrame = rollDirection * distanceThisFrame;

        playerRigidbody.MovePosition(playerRigidbody.position + moveThisFrame);
    }
}