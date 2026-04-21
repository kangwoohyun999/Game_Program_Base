using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [Header("감도 설정")]
    public float     sensitivity  = 2.0f;
    public Transform playerCamera;

    private float xRotation = 0f;

    private void Start()
    {
        // 게임 시작 전이므로 커서 표시 (SelectionUI에서 시작)
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;
    }

    private void Update()
    {
        if (!GameManager.Instance.IsPlaying)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible   = true;
            return;
        }

        if (ResultUI.Instance != null && ResultUI.Instance.gameObject.activeSelf) return;
        if (playerCamera == null) return;

        // [ = 감도 낮추기,  ] = 감도 높이기
        if (Input.GetKeyDown(KeyCode.LeftBracket))
            sensitivity = Mathf.Max(0.5f, sensitivity - 0.2f);
        if (Input.GetKeyDown(KeyCode.RightBracket))
            sensitivity = Mathf.Min(10f, sensitivity + 0.2f);

        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        transform.Rotate(Vector3.up * mouseX);

        xRotation -= mouseY;
        xRotation  = Mathf.Clamp(xRotation, -90f, 90f);
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}
