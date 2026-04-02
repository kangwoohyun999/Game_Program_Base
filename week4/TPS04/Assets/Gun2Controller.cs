using UnityEngine;

public class Gun2Controller : MonoBehaviour
{
    private GameObject player;

    void Start()
    {
        player = GameObject.Find("Player");
    }

    void Update()
    {
        if (player == null) return;

        // PDF 지시: Player를 향해 Gun2 회전
        Vector3 dir = player.transform.position - this.transform.position;
        this.transform.rotation = Quaternion.LookRotation(-dir);   // 책에서 -dir 사용
    }
}