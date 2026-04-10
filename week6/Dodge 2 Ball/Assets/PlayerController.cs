using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody playerRigidbody;
    public float speed = 8f;

    private int lives = 5;

    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float xInput = Input.GetAxis("Horizontal");
        float zInput = Input.GetAxis("Vertical");

        float xSpeed = xInput * speed;
        float zSpeed = zInput * speed;

        Vector3 newVelocity = new Vector3(xSpeed, 0f, zSpeed);
        playerRigidbody.linearVelocity = newVelocity;
    }

    public void Die()
    {
        lives--;
        Debug.Log($"남은 체력: {lives}");

        if (lives <= 0)
        {
            gameObject.SetActive(false);
            Debug.Log("GAME OVER - 5번 맞음!");
        }
    }
}