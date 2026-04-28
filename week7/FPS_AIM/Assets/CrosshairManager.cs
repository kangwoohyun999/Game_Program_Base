using UnityEngine;

public class CrosshairManager : MonoBehaviour
{
    public static CrosshairManager Instance;
    public GameObject crosshairPanel;

    private void Awake()
    {
        Instance = this;
        if (crosshairPanel == null)
            crosshairPanel = gameObject;

        HideCrosshair();
    }

    public void ShowCrosshair() => crosshairPanel?.SetActive(true);
    public void HideCrosshair() => crosshairPanel?.SetActive(false);
}