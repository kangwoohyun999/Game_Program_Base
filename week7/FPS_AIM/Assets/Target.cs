using UnityEngine;
using System.Collections;

/// <summary>
/// Easy  : 정지 (움직임 없음)
/// Normal: 좌우로 랜덤 이동 (0.5~1.2 초마다 새 목표 위치)
/// Hard  : 정지, 크기만 작음 (GameManager에서 scale 조절)
/// </summary>
public class Target : MonoBehaviour
{
    [Header("Normal 이동 속도")]
    public float moveSpeed = 4f;

    private bool     isMoving = false;
    private float    targetX;
    private float    fixedY;
    private float    fixedZ;
    private Coroutine moveRoutine;

    public void SetMode(Mode mode)
    {
        if (mode == Mode.Normal)
        {
            isMoving = true;
            fixedY   = transform.position.y;
            fixedZ   = transform.position.z;
            targetX  = transform.position.x;
            moveRoutine = StartCoroutine(RandomMoveRoutine());
        }
        // Easy / Hard : 정지
    }

    private IEnumerator RandomMoveRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(0.5f, 1.2f));
            targetX = Random.Range(-15f, 15f);
        }
    }

    private void Update()
    {
        if (!isMoving) return;

        Vector3 goal = new Vector3(targetX, fixedY, fixedZ);
        transform.position = Vector3.MoveTowards(transform.position, goal, moveSpeed * Time.deltaTime);
    }

    public void Hit()
    {
        GameManager.Instance.RegisterHit();
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (moveRoutine != null) StopCoroutine(moveRoutine);
    }
}
