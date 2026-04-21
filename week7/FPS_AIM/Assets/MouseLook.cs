using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [Header("감도 설정")]
    public float sensitivity = 2.0f;
    public Transform playerCamera;

    private float xRotation = 0f;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (!GameManager.Instance.IsPlaying)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            return;
        }

        // Pause 중에는 Look 금지
        if (PauseUI.Instance != null && PauseUI.Instance.IsPaused)
            return;

        if (playerCamera == null) return;

        // 감도 조절
        if (Input.GetKeyDown(KeyCode.LeftBracket)) sensitivity = Mathf.Max(0.5f, sensitivity - 0.2f);
        if (Input.GetKeyDown(KeyCode.RightBracket)) sensitivity = Mathf.Min(10f, sensitivity + 0.2f);

        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        transform.Rotate(Vector3.up * mouseX);

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}