using UnityEngine;
using System.Collections;

public class Target : MonoBehaviour
{
    [Header("Å¸°Ù ¼³Á¤")]
    public float moveSpeed = 9f;

    private bool isMoving = false;
    private float targetX;
    private float fixedY;
    private float fixedZ;
    private Coroutine moveRoutine;

    public void SetMoving(bool moving)
    {
        isMoving = moving;
        if (isMoving)
        {
            fixedY = transform.position.y;
            fixedZ = transform.position.z;
            targetX = transform.position.x;
            moveRoutine = StartCoroutine(RandomMoveRoutine());
        }
    }

    private IEnumerator RandomMoveRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(0.5f, 1.2f));
            targetX = Random.Range(-15f, 15f);   // ÁÂ¿ì ÀÌµ¿ ¹üÀ§
        }
    }

    private void Update()
    {
        if (isMoving)
        {
            Vector3 targetPos = new Vector3(targetX, fixedY, fixedZ);
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
        }
    }

    public void Hit()
    {
        GameManager.Instance.RegisterHit();
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (moveRoutine != null)
            StopCoroutine(moveRoutine);
    }
}