using UnityEngine;

// 14주차 - 캐릭터 이동 구현
// 3주차 - 클래스와 오브젝트, 컴포넌트 사용
public class PlayerController : MonoBehaviour
{
    [Header("이동 설정")]
    public float moveSpeed = 5f;

    [Header("배고픔 설정")]
    public float maxHunger = 100f;
    public float hungerDecreaseRate = 2f; // 초당 감소량

    // 현재 배고픔 값 (프로퍼티로 외부 읽기 전용)
    public float hunger { get; private set; }

    // 컴포넌트 참조 (3주차 - 변수로 컴포넌트 사용하기)
    private Rigidbody rb;
    private Camera mainCamera;
    private Animator animator;
    private PlayerHealth playerHealth;

    // 무기 핫바 (현재 선택된 무기 인덱스)
    public int currentWeaponIndex { get; private set; } = 0;

    private void Awake()
    {
        // GetComponent - 3주차 핵심 내용
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        playerHealth = GetComponent<PlayerHealth>();
        mainCamera = Camera.main;
    }

    private void Start()
    {
        hunger = maxHunger;
    }

    private void Update()
    {
        // 배고픔 감소
        UpdateHunger();

        // 무기 핫바 입력 (1, 2, 3키)
        HandleWeaponSwitch();
    }

    private void FixedUpdate()
    {
        // 물리 기반 이동 (3주차 - 물리 엔진 적용)
        Move();
        Rotate();
    }

    // 이동 처리
    private void Move()
    {
        // 7주차 - 사용자 키 입력 받아서 처리
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 moveDir = new Vector3(h, 0f, v).normalized;
        Vector3 velocity = moveDir * moveSpeed;

        // 8주차 - 물리 엔진(Rigidbody) 활용
        rb.linearVelocity = new Vector3(velocity.x, rb.linearVelocity.y, velocity.z);

        // 애니메이션 (11주차 - 애니메이션)
        if (animator != null)
        {
            animator.SetFloat("Speed", moveDir.magnitude);
        }
    }

    // 마우스 방향으로 회전
    private void Rotate()
    {
        // 레이캐스트로 바닥 클릭 위치 찾기 (15주차 - 레이캐스트)
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f, LayerMask.GetMask("Ground")))
        {
            Vector3 lookTarget = hit.point;
            lookTarget.y = transform.position.y;

            // 7주차 - 오브젝트 회전
            transform.LookAt(lookTarget);
        }
    }

    // 배고픔 업데이트
    private void UpdateHunger()
    {
        hunger -= hungerDecreaseRate * Time.deltaTime;
        hunger = Mathf.Clamp(hunger, 0f, maxHunger);

        // 배고픔이 0이 되면 체력 감소
        if (hunger <= 0f && playerHealth != null)
        {
            playerHealth.OnDamage(5f * Time.deltaTime, transform.position, Vector3.up);
        }
    }

    // 무기 교체 (핫바 1, 2, 3)
    private void HandleWeaponSwitch()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            SwitchWeapon(0);
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            SwitchWeapon(1);
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            SwitchWeapon(2);
    }

    private void SwitchWeapon(int index)
    {
        currentWeaponIndex = index;
        // WeaponManager에 알림
        WeaponManager weaponManager = GetComponent<WeaponManager>();
        if (weaponManager != null)
        {
            weaponManager.EquipWeapon(index);
        }

        // UIManager로 핫바 UI 업데이트 (17주차 - 싱글턴)
        UIManager.instance?.UpdateHotbar(index);
    }
}
