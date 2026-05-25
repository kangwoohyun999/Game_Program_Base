using UnityEngine;

// 플레이어 캐릭터를 조작하기 위한 사용자 입력을 감지
// 감지된 입력값을 다른 컴포넌트들이 사용할 수 있도록 제공
public class PlayerInput : MonoBehaviour {
    public string moveAxisName = "Vertical"; // 앞뒤 움직임을 위한 입력축 이름
    public string rotateAxisName = "Horizontal"; // 좌우 회전을 위한 입력축 이름
    public string fireButtonName = "Fire1"; // 발사를 위한 입력 버튼 이름
    public string reloadButtonName = "Reload"; // 재장전을 위한 입력 버튼 이름
    public string sprintButtonName = "Sprint"; // 스프린트 입력 버튼 이름
    public string dashButtonName = "Dash"; // 대쉬 입력 버튼 이름

    const float MAX_SPRINT_TIME = 5f;
    private float sprintTime = 0;

    private PlayerMovement playerMovement;

    // 값 할당은 내부에서만 가능
    public float move { get; private set; } // 감지된 움직임 입력값
    public float rotate { get; private set; } // 감지된 회전 입력값
    public bool fire { get; private set; } // 감지된 발사 입력값
    public bool reload { get; private set; } // 감지된 재장전 입력값
    public bool sprint { get; private set; } // 스프린트 입력값s

    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }
    // 매프레임 사용자 입력을 감지
    private void Update() {
        // 게임오버 상태에서는 사용자 입력을 감지하지 않는다
        if (GameManager.instance != null
            && GameManager.instance.isGameover)
        {
            move = 0;
            rotate = 0;
            fire = false;
            reload = false;
            sprint = false;
            return;
        }

        // move에 관한 입력 감지
        move = Input.GetAxis(moveAxisName);
        // rotate에 관한 입력 감지
        rotate = Input.GetAxis(rotateAxisName);
        // fire에 관한 입력 감지
        fire = Input.GetButton(fireButtonName);
        // reload에 관한 입력 감지
        reload = Input.GetButtonDown(reloadButtonName);
        // sprint
        if (Input.GetButtonDown(sprintButtonName))
        {
            sprint = true;
            sprintTime = 0f;
        }
        if (Input.GetButton(sprintButtonName) && sprint)
        {
            sprintTime += Time.deltaTime;
            if (sprintTime >= MAX_SPRINT_TIME)
                sprint = false;
        }
        if (Input.GetButtonUp(sprintButtonName))
        {
            sprint = false;
        }
        // dash
        if (Input.GetButtonDown(dashButtonName))
        {
            playerMovement.SetDashRequest();
        }
    }
}