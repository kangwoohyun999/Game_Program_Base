using UnityEngine;
using TMPro;

public class ResultUI : MonoBehaviour
{
    public static ResultUI Instance;

    [Header("결과 UI")]
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI accuracyText;
    public TextMeshProUGUI rankText;

    private void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);   // 시작할 때 무조건 꺼두기
    }

    public void ShowResult()
    {
        gameObject.SetActive(true);

        timeText.text = $"경과 시간 : {GameManager.Instance.ElapsedTime:F2}초";
        accuracyText.text = $"명중률 : {GameManager.Instance.Accuracy:F1}%";
        rankText.text = GameManager.Instance.FinalRank;
    }

    private void Update()
    {
        if (gameObject.activeSelf && Input.GetMouseButtonDown(0))
        {
            gameObject.SetActive(false);
            GameManager.Instance.ReturnToSelection();
            SelectionUI.Instance.gameObject.SetActive(true);
        }
    }
}