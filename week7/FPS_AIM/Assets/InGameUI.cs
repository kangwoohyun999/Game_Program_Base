using UnityEngine;
using TMPro;

public class InGameUI : MonoBehaviour
{
    public static InGameUI Instance;

    [Header("InGame UI")]
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI hitText;
    public TextMeshProUGUI modeText;

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
        hitText.text = GameManager.Instance.CurrentHits + " / " + GameManager.Instance.TotalTargets;

        if (modeText != null)
            modeText.text = GameManager.Instance.GetCurrentModeName();
    }
}