using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float speed = 20f;

    private float moveRate;
    private float timeAfterMove;

    void Start()
    {
        moveRate = Random.Range(0.8f, 2.0f);
        timeAfterMove = 0f;
    }

    void Update()
    {
        timeAfterMove += Time.deltaTime;

        if (timeAfterMove > moveRate)
        {
            float dir = Random.Range(-1f, 1f);
            timeAfterMove = 0f;
            moveRate = Random.Range(0.8f, 2.0f);

            if (dir < 0) // 좌로 이동
            {
                if (transform.position.x > -4f)
                    transform.Translate(-speed * Time.deltaTime, 0, 0);
            }
            else // 우로 이동
            {
                if (transform.position.x < 4f)
                    transform.Translate(speed * Time.deltaTime, 0, 0);
            }
        }
    }
}