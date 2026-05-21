using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // ==================== 기본 변수 (책 원본) ====================
    public AudioClip deathClip;
    public float jumpForce = 700f;

    // ==================== 새로 추가된 변수 ====================
    [Header("이동 설정")]
    public float moveSpeed = 6f;

    [Header("금화 시스템")]
    public GameObject coinPrefab;
    public float coinSpawnInterval = 2.2f;     // 금화 생성 간격
    public float coinSpawnRadius = 7f;         // 플레이어 주변 생성 범위
    public int maxCoinsAtOnce = 5;             // 화면에 동시에 존재할 수 있는 최대 금화 수

    [Header("바위 시스템")]
    public GameObject rockPrefab;
    public float rockSpawnInterval = 1.3f;

    [Header("체력")]
    public int maxHealth = 3;

    private Rigidbody2D rb;
    private Animator animator;
    private AudioSource audioSource;
    private SpriteRenderer spriteRenderer;

    private bool isDead = false;
    private int jumpCount = 0;
    private bool isGrounded = false;
    private int currentHealth;

    private float nextRockSpawnTime = 0f;
    private float nextCoinSpawnTime = 0f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (isDead) return;

        // 1. 좌우 이동
        float horizontal = Input.GetAxis("Horizontal");
        Vector2 velocity = rb.linearVelocity;
        velocity.x = horizontal * moveSpeed;
        rb.linearVelocity = velocity;

        // 스프라이트 좌우 반전
        if (horizontal != 0)
            spriteRenderer.flipX = horizontal < 0;

        // 2. 점프 (마우스 왼쪽 클릭)
        if (Input.GetMouseButtonDown(0) && jumpCount < 2)
        {
            Jump();
        }

        // 점프 높이 조절 (버튼 떼기)
        if (Input.GetMouseButtonUp(0) && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
        }

        // 3. 바위 생성
        if (Time.time >= nextRockSpawnTime)
        {
            SpawnRock();
            nextRockSpawnTime = Time.time + rockSpawnInterval;
        }

        // 4. 금화 생성 (개선됨)
        if (Time.time >= nextCoinSpawnTime)
        {
            SpawnCoin();
            nextCoinSpawnTime = Time.time + coinSpawnInterval;
        }

        // 애니메이션
        animator.SetBool("Ground", isGrounded);
        animator.SetFloat("VelocityY", rb.linearVelocity.y);
    }

    private void Jump()
    {
        jumpCount++;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce);
        audioSource.Play();
    }

    // ==================== 금화 생성 (개선 버전) ====================
    private void SpawnCoin()
    {
        if (coinPrefab == null) return;

        // 너무 많은 금화가 쌓이지 않도록 제한
        if (GameObject.FindGameObjectsWithTag("Coin").Length >= maxCoinsAtOnce)
            return;

        // 플레이어 주변 랜덤 위치
        float angle = Random.Range(0f, Mathf.PI * 2);
        float distance = Random.Range(4f, coinSpawnRadius);

        Vector3 spawnPos = transform.position + new Vector3(
            Mathf.Cos(angle) * distance,
            Random.Range(3f, 7f),  // 위쪽으로 주로 생성
            0f
        );

        // 화면 밖으로 나가지 않게 제한
        spawnPos.x = Mathf.Clamp(spawnPos.x, -9f, 9f);

        Instantiate(coinPrefab, spawnPos, Quaternion.identity);
    }

    private void SpawnRock()
    {
        if (rockPrefab == null) return;
        float spawnX = Random.Range(-8.5f, 8.5f);
        Instantiate(rockPrefab, new Vector3(spawnX, 13f, 0f), Quaternion.identity);
    }

    // ==================== 충돌 처리 ====================
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;

        // 바닥 착지 판단
        if (collision.contacts.Length > 0 && collision.contacts[0].normal.y > 0.7f)
        {
            isGrounded = true;
            jumpCount = 0;
        }

        if (collision.gameObject.CompareTag("Rock"))
        {
            TakeDamage();
            Destroy(collision.gameObject);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform") || 
            collision.gameObject.CompareTag("Start Platform"))
        {
            isGrounded = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead) return;

        if (other.CompareTag("Dead"))
        {
            Die();
        }
        else if (other.CompareTag("Coin"))
        {
            Destroy(other.gameObject);
            
            // 금화 먹으면 자동 점프
            if (jumpCount < 2)
            {
                Jump();
            }
            // 추가 효과 원하면 여기에 사운드나 이펙트 넣을 수 있음
        }
    }

    private void TakeDamage()
    {
        currentHealth--;
        if (currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        isDead = true;
        animator.SetTrigger("Die");
        audioSource.clip = deathClip;
        audioSource.Play();
        rb.linearVelocity = Vector2.zero;
    }
}