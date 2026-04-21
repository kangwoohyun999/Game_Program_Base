using UnityEngine;
using TMPro;

public class InGameUI : MonoBehaviour
{
    public static InGameUI Instance;

    [Header("InGame UI")]
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI hitText;
    public TextMeshProUGUI modeText;
    public TextMeshProUGUI sensitivityText;   // 감도 표시 (선택)

    private void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        if (CrosshairManager.Instance != null)
            CrosshairManager.Instance.ShowCrosshair();
    }

    private void Update()
    {
        if (!GameManager.Instance.IsPlaying) return;

        timeText.text = GameManager.Instance.ElapsedTime.ToString("F2") + "s";
        hitText.text  = GameManager.Instance.CurrentHits + " / " + GameManager.Instance.TotalTargets;

        if (modeText != null)
            modeText.text = GameManager.Instance.GetCurrentModeName();

        // 감도 UI (MouseLook 컴포넌트에서 읽어옴)
        if (sensitivityText != null)
        {
            var ml = FindFirstObjectByType<MouseLook>();
            if (ml != null)
                sensitivityText.text = $"Sensitivity : {ml.sensitivity:F1}";
        }
    }

    public void ForceUpdateHitText(int current, int total)
    {
        hitText.text = current + " / " + total;
    }
}
