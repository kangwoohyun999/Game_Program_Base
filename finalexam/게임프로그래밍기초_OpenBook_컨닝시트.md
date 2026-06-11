# 게임프로그래밍기초 OPEN BOOK 컨닝시트
**9week ~ 14week | 객관식 · 주관식 · 게임 설계 및 작성 대비**

---

## 9W① AppleCatcher (3D) — 프리팹 · 충돌 · 제너레이터 · GameManager

### 충돌 구성 핵심
```
사과/폭탄 : Sphere Collider + APPLE / BOMB 태그
바구니    : Rigidbody(IsKinematic✓) + Box Collider(IsTrigger✓)
스테이지  : Box Collider  Size(3, 0.1, 3)
```

### ItemGenerator.cs 패턴
```csharp
// 랜덤 생성
int r = Random.Range(0, spawnList.Length);
GameObject obj = Instantiate(spawnList[r], spawnPos, Quaternion.identity);
obj.GetComponent<ItemController>().SetSpeed(speed);

// 난이도 조절 (외부에서 호출)
public void SetParameters(float interval, float speed, float ratio) {
    spawnInterval = interval;
    fallSpeed     = speed;
    bombRatio     = ratio;
}
```

### BasketController.cs (충돌 + 효과음)
```csharp
public AudioClip appleSE, bombSE;
AudioSource ad;

void Start() { ad = GetComponent<AudioSource>(); }

void OnTriggerEnter(Collider other) {
    if (other.CompareTag("APPLE")) {
        ad.PlayOneShot(appleSE);
        GameManager.instance.AddScore(1);
    } else if (other.CompareTag("BOMB")) {
        ad.PlayOneShot(bombSE);
        GameManager.instance.AddScore(-1);
    }
    Destroy(other.gameObject);
}
```

### GameManager.cs (싱글턴 + UI)
```csharp
using UnityEngine.UI;  // 필수!

public static GameManager instance;
public Text timeText, pointText;
int score;
float timeLimit = 30f;

void Awake() {
    if (instance == null) instance = this;
    else Destroy(gameObject);
}

public void AddScore(int s) {
    score += s;
    pointText.text = score + " point";
}

void Update() {
    timeLimit -= Time.deltaTime;
    timeText.text = timeLimit.ToString("F1");
    if (timeLimit <= 0) { timeLimit = 0; /* 게임 종료 */ }
}
```

---

## 9W② UniRun 플레이어 (Ch.11) — FSM · 애니메이터 · PlayerController

### 애니메이터 FSM 구성
```
상태:  Run ←→ Jump,  AnyState → Die

파라미터:
  Grounded  (Bool)    Run→Jump: false  /  Jump→Run: true
  Die       (Trigger) AnyState→Die

전이 설정:
  Has Exit Time    : 체크 해제
  Transition Duration : 0
  Die Loop Time    : 체크 해제
```

### 파라미터 제어 코드
```csharp
animator.SetBool("Grounded", isGrounded);
animator.SetTrigger("Die");
```

### PlayerController.cs 전체 핵심
```csharp
public float jumpForce = 700f;
public AudioClip deathClip;

Rigidbody2D    rigid;
Animator       anim;
AudioSource    audioSrc;
bool  isDead    = false;
bool  isGrounded = false;
int   jumpCount  = 0;

void Start() {
    rigid   = GetComponent<Rigidbody2D>();
    anim    = GetComponent<Animator>();
    audioSrc = GetComponent<AudioSource>();
}

void Update() {
    if (isDead) return;

    // 점프
    if (jumpCount < 2 && Input.GetMouseButtonDown(0)) {
        jumpCount++;
        rigid.velocity = new Vector2(rigid.velocity.x, jumpForce);
        audioSrc.Play();
    }
    // 낮은 점프
    if (Input.GetMouseButtonUp(0) && rigid.velocity.y > 0)
        rigid.velocity = new Vector2(rigid.velocity.x, rigid.velocity.y * 0.5f);

    anim.SetBool("Grounded", isGrounded);
}

// 바닥 감지 — 법선 벡터 y > 0.7
void OnCollisionEnter2D(Collision2D c) {
    foreach (var contact in c.contacts)
        if (contact.normal.y > 0.7f) { isGrounded = true; jumpCount = 0; }
}
void OnCollisionExit2D(Collision2D c)  { isGrounded = false; }

// 낙사 감지
void OnTriggerEnter2D(Collider2D c) {
    if (c.CompareTag("Dead")) Die();
}

void Die() {
    isDead = true;
    anim.SetTrigger("Die");
    audioSrc.clip = deathClip;
    audioSrc.Play();
    rigid.velocity = Vector2.zero;
    GameManager.instance.OnPlayerDead();
}
```

---

## 10W① 배경 스크롤링 & 싱글턴 (Ch.12)

### ScrollingObject.cs
```csharp
public float speed = 10f;

void Update() {
    if (GameManager.instance.isGameover) return;
    transform.Translate(Vector3.left * speed * Time.deltaTime);
}
```

### BackgroundLoop.cs
```csharp
float width;

void Awake() {
    width = GetComponent<SpriteRenderer>().bounds.size.x;
}

void Update() {
    if (transform.position.x < -width) Reposition();
}

void Reposition() {
    transform.position += new Vector3(width * 2f, 0f, 0f);
}
```
> Sky에 **Box Collider 2D (IsTrigger ✓)** 추가 필수

### GameManager.cs (싱글턴 + 씬 재시작)
```csharp
using UnityEngine.SceneManagement;
using TMPro;

public static GameManager instance;
public bool  isGameover { get; private set; }
public TMP_Text scoreText, gameoverText;
public GameObject gameoverUI;
int score;

void Awake() {
    if (instance == null) instance = this;
    else Destroy(gameObject);
}

void Update() {
    if (isGameover && Input.GetMouseButtonDown(0))
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
}

public void AddScore(int s) {
    if (isGameover) return;
    score += s;
    scoreText.text = "Score : " + score;
}

public void OnPlayerDead() {
    isGameover = true;
    gameoverUI.SetActive(true);
}
```

### 캔버스 스케일러 비교
| 모드 | 특징 | 권장 여부 |
|---|---|---|
| 고정 픽셀 크기 | 화면 달라도 픽셀 크기 고정 → 커지면 UI가 작아 보임 | X |
| Scale With Screen Size | 기준 해상도(640×360) 기준 자동 확대/축소 | ✓ |

---

## 10W② 발판 반복 생성 (Ch.13) — 오브젝트 풀링

### PlatformSpawner.cs
```csharp
public GameObject platformPrefab;
public int   count         = 3;
public float timeBetSpawnMin = 1f, timeBetSpawnMax = 3f;
public float spawnX        = 20f;
public float yMin = -3.5f, yMax = 1.5f;

GameObject[] platforms;
int   currentIndex  = 0;
float timeBetSpawn;
float lastSpawnTime;

void Start() {
    platforms = new GameObject[count];
    for (int i = 0; i < count; i++)
        platforms[i] = Instantiate(platformPrefab);
    timeBetSpawn = 0f;
    lastSpawnTime = 0f;
}

void Update() {
    if (GameManager.instance.isGameover) return;

    if (Time.time >= lastSpawnTime + timeBetSpawn) {
        lastSpawnTime = Time.time;
        timeBetSpawn  = Random.Range(timeBetSpawnMin, timeBetSpawnMax);

        float h = Random.Range(yMin, yMax);
        platforms[currentIndex].SetActive(false);        // 리셋 트리거
        platforms[currentIndex].SetActive(true);
        platforms[currentIndex].transform.position = new Vector2(spawnX, h);
        currentIndex = (currentIndex + 1) % count;      // 순환
    }
}
```

### Platform.cs
```csharp
public GameObject[] obstacles;  // Obstacle Left/Mid/Right
bool stepped;

void OnEnable() {
    stepped = false;
    foreach (var ob in obstacles)
        ob.SetActive(Random.Range(0, 3) == 0);  // 1/3 확률 활성화
}

void OnCollisionEnter2D(Collision2D c) {
    if (!stepped && c.collider.CompareTag("Player")) {
        stepped = true;
        GameManager.instance.AddScore(1);
    }
}
```

---

## 11W 좀비 플레이어 (Ch.14) — 라이팅 · 블렌드트리 · 프로퍼티 · 시네머신

### 라이팅 / GI 정리
```
라이트맵  : 빛 받는 모습을 미리 텍스처에 굽기 → 실시간 연산↓
GI 모드
  Baked Indirect  : 간접광만 굽기, 직사광·그림자 실시간
  Shadowmask(기본): 간접광 + 그림자 맵 함께 굽기
  Subtractive     : 전부 굽기, 가장 성능↑

라이트 모드: Baked / Realtime / Mixed
환경광(Ambient): 씬 전체 기본 빛, 그림자 없음
```

### 애니메이터 레이어 & 블렌드 트리
```
레이어 적용 순서 : 위→아래 덮어쓰기
  Base Movement  : 뛰기 / 점프 / 사망
  Upper Body     : 조준 / 재장전  ← 아바타 마스크 적용 (상체만)

블렌드 트리 (Movement 상태):
  Move 파라미터 값에 따라 클립 혼합
  임곗값(Threshold): 해당 클립 100% 재생되는 지점
  예) -1=뒤뛰기, 0=대기, 1=앞뛰기
```

### 프로퍼티 (Property)
```csharp
// 자동 구현 — 외부 읽기O, 외부 쓰기X
public float Move   { get; private set; }
public float Rotate { get; private set; }
public bool  Fire   { get; private set; }

// get/set 처리 삽입 예시
public float bytes {
    get { return m_bytes; }
    set { if (value >= 0) m_bytes = value; }
}
```

### PlayerMovement.cs 핵심
```csharp
void FixedUpdate() { Move(); Rotate(); }

void Move() {
    Vector3 d = transform.forward * moveSpeed
                * playerInput.move * Time.deltaTime;
    playerRigidbody.MovePosition(playerRigidbody.position + d);
    playerAnimator.SetFloat("Move", playerInput.move);
}

void Rotate() {
    float turn = playerInput.rotate * rotateSpeed * Time.deltaTime;
    Quaternion rot = Quaternion.Euler(0f, turn, 0f);
    playerRigidbody.MoveRotation(playerRigidbody.rotation * rot);
}
```

### 시네머신 설정 요약
```
Brain Camera : Main Camera에 Cinemachine Brain 컴포넌트 추가
Virtual Cam  : Follow & Look At = Player Character
Body         : Follow Offset(-8, 16, -8), Damping 0.1
Aim          : Soft Zone W/H = 0, Damping = 0  → 즉각 추적
FOV          : 20 (줌인)
```

---

## 12W 총과 슈터 (Ch.15) — 인터페이스 · ScriptableObject · 코루틴 · 레이캐스트 · IK

### 인터페이스 (Interface)
```csharp
public interface IItem       { void Use(GameObject target); }
public interface IDamageable {
    void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal);
}

// 느슨한 커플링 — 타입을 몰라도 사용 가능
void OnTriggerEnter(Collider c) {
    IItem item = c.GetComponent<IItem>();
    if (item != null) item.Use(gameObject);
}
```

### ScriptableObject (GunData)
```csharp
[CreateAssetMenu(fileName = "GunData", menuName = "Scriptable/GunData")]
public class GunData : ScriptableObject {
    public AudioClip shotClip, reloadClip;
    public float damage       = 25f;
    public int   magCapacity  = 30;
    public int   startAmmoRemain = 100;
    public float timeBetFire  = 0.12f;
    public float reloadTime   = 1.8f;
}
// 여러 Gun이 하나의 GunData 에셋 공유 → 데이터 불일치 방지
```

### 코루틴 (Coroutine)
```csharp
// 선언
IEnumerator ShotEffect(Vector3 hitPos) {
    muzzleFlashEffect.Play();
    shellEjectEffect.Play();
    gunAudioPlayer.PlayOneShot(gunData.shotClip);

    lineRenderer.enabled = true;
    lineRenderer.SetPosition(0, fireTransform.position);
    lineRenderer.SetPosition(1, hitPos);

    yield return new WaitForSeconds(0.03f);   // 0.03초 대기
    lineRenderer.enabled = false;
}
// 실행
StartCoroutine(ShotEffect(hitPosition));

// ReloadRoutine
IEnumerator ReloadRoutine() {
    state = State.Reloading;
    gunAudioPlayer.PlayOneShot(gunData.reloadClip);
    yield return new WaitForSeconds(gunData.reloadTime);
    // 탄창 채우기
    int toFill = gunData.magCapacity - magAmmo;
    if (toFill > ammoRemain) toFill = ammoRemain;
    magAmmo    += toFill;
    ammoRemain -= toFill;
    state = State.Ready;
}
```

### 레이캐스트 (Raycast)
```csharp
void Shot() {
    RaycastHit hit;
    Vector3 hitPos = fireTransform.position
                   + fireTransform.forward * fireDistance;

    if (Physics.Raycast(fireTransform.position,
                        fireTransform.forward,
                        out hit, fireDistance)) {
        IDamageable target = hit.collider.GetComponent<IDamageable>();
        if (target != null)
            target.OnDamage(gunData.damage, hit.point, hit.normal);
        hitPos = hit.point;
    }
    StartCoroutine(ShotEffect(hitPos));
    magAmmo--;
    if (magAmmo <= 0) state = State.Empty;
}
// out 키워드: Raycast 내부에서 hit에 충돌 정보 채워줌
```

### Gun State 열거형
```csharp
public enum State { Ready, Empty, Reloading }
State state;

void Fire() {
    if (state == State.Ready &&
        Time.time >= lastFireTime + gunData.timeBetFire) {
        lastFireTime = Time.time;
        Shot();
    }
}
```

### IK (Inverse Kinematics)
```csharp
// OnAnimatorIK() — Upper Body 레이어 IK Pass 활성화 필수
void OnAnimatorIK(int layerIndex) {
    // Gun Pivot을 오른쪽 팔꿈치 위치로 이동
    gunPivot.position = playerAnimator.GetIKHintPosition(AvatarIKHint.RightElbow);

    // 왼손 IK
    playerAnimator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
    playerAnimator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1f);
    playerAnimator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandMount.position);
    playerAnimator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandMount.rotation);

    // 오른손 IK
    playerAnimator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1f);
    playerAnimator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1f);
    playerAnimator.SetIKPosition(AvatarIKGoal.RightHand, rightHandMount.position);
    playerAnimator.SetIKRotation(AvatarIKGoal.RightHand, rightHandMount.rotation);
}
```

---

## 13W 생명체 & 좀비 AI (Ch.16) — 다형성 · 이벤트 · NavMesh · 슬라이더

### 다형성 & 오버라이드
```csharp
// 부모
public class Monster : MonoBehaviour {
    public virtual void Attack() { Debug.Log("Monster Attack"); }
}
// 자식
public class Orc : Monster {
    public override void Attack() {
        base.Attack();            // 부모 먼저 호출
        Debug.Log("Orc Roar");
    }
}
// 다형성 사용
Monster m = new Orc();
m.Attack();  // → "Monster Attack" + "Orc Roar"
```

### LivingEntity.cs 핵심 구조
```csharp
public class LivingEntity : MonoBehaviour, IDamageable {
    public float startingHealth = 100f;
    public float health   { get; protected set; }  // 자식만 수정
    public bool  dead     { get; protected set; }
    public event Action onDeath;                   // 외부 발동 불가

    protected virtual void OnEnable() {
        dead   = false;
        health = startingHealth;
    }

    public virtual void OnDamage(float d, Vector3 p, Vector3 n) {
        health -= d;
        if (health <= 0 && !dead) Die();
    }

    public virtual void RestoreHealth(float h) {
        if (dead) return;
        health += h;
    }

    public virtual void Die() {
        onDeath?.Invoke();   // 이벤트 발동
        dead = true;
    }
}
```

### PlayerHealth.cs (오버라이드 예시)
```csharp
public class PlayerHealth : LivingEntity {
    public Slider     healthSlider;
    public AudioClip  deathClip, hitClip, itemPickupClip;

    protected override void OnEnable() {
        base.OnEnable();                          // 부모 초기화
        healthSlider.gameObject.SetActive(true);
        healthSlider.maxValue = startingHealth;
        healthSlider.value    = health;
    }

    public override void OnDamage(float d, Vector3 p, Vector3 n) {
        if (!dead) audioSrc.PlayOneShot(hitClip);
        base.OnDamage(d, p, n);                  // 실제 피해 적용
        healthSlider.value = health;              // UI 갱신
    }

    public override void Die() {
        base.Die();
        healthSlider.gameObject.SetActive(false);
        audioSrc.PlayOneShot(deathClip);
        animator.SetTrigger("Die");
        playerMovement.enabled  = false;
        playerShooter.enabled   = false;
    }

    void OnTriggerEnter(Collider c) {
        if (!dead) {
            IItem item = c.GetComponent<IItem>();
            if (item != null) {
                item.Use(gameObject);
                audioSrc.PlayOneShot(itemPickupClip);
            }
        }
    }
}
```

### Zombie AI (UpdatePath 코루틴)
```csharp
IEnumerator UpdatePath() {
    while (!dead) {
        if (hasTarget) {
            navMeshAgent.isStopped = false;
            navMeshAgent.SetDestination(targetEntity.transform.position);
        } else {
            navMeshAgent.isStopped = true;
            // 범위 탐색 (반지름 20)
            Collider[] cols = Physics.OverlapSphere(
                transform.position, 20f, whatIsTarget);
            foreach (var col in cols) {
                LivingEntity e = col.GetComponent<LivingEntity>();
                if (e != null && !e.dead) {
                    targetEntity = e;
                    break;
                }
            }
        }
        yield return new WaitForSeconds(0.25f);
    }
}

// OnTriggerStay: 공격
void OnTriggerStay(Collider c) {
    if (!dead && Time.time >= lastAttackTime + timeBetAttack) {
        LivingEntity atk = c.GetComponent<LivingEntity>();
        if (atk != null && atk == targetEntity) {
            lastAttackTime = Time.time;
            Vector3 hp = c.ClosestPoint(transform.position);
            atk.OnDamage(damage, hp, (hp - transform.position).normalized);
        }
    }
}

// Die 오버라이드
public override void Die() {
    base.Die();
    GetComponents<Collider>().ToList().ForEach(c => c.enabled = false);
    navMeshAgent.isStopped = true;
    animator.SetTrigger("Die");
}
```

### UI 슬라이더 (World Space)
```
Canvas Render Mode    : World Space
Canvas Scaler         : Reference Pixels per Unit = 1
Canvas 위치           : (0, 0.3, 0)  Rotation(90, 0, 0)
Image Type (Fill)     : Filled  → 원형 채움
Interactable          : 체크 해제 (입력 무시)
```

---

## 14W 최종 완성 (Ch.17) — 좀비생성기 · 아이템 · 람다 · 포스트프로세싱

### ZombieSpawner.cs
```csharp
List<Zombie> zombies = new List<Zombie>();
int wave = 0;

void Update() {
    if (GameManager.instance.isGameover) return;
    if (zombies.Count == 0) SpawnWave();
    UIManager.instance.UpdateWaveText(wave, zombies.Count);
}

void SpawnWave() {
    wave++;
    int n = Mathf.RoundToInt(wave * 1.5f);
    for (int i = 0; i < n; i++) CreateZombie();
}

void CreateZombie() {
    ZombieData d  = zombieDatas[Random.Range(0, zombieDatas.Length)];
    Transform  sp = spawnPoints[Random.Range(0, spawnPoints.Length)];
    Zombie z = Instantiate(zombiePrefab, sp.position, sp.rotation)
               .GetComponent<Zombie>();
    z.Setup(d);
    zombies.Add(z);

    // 람다식으로 사망 이벤트 등록
    z.onDeath += () => {
        zombies.Remove(z);
        Destroy(z.gameObject, 10f);
        GameManager.instance.AddScore(100);
    };
}
```

### ItemSpawner.cs — NavMesh 랜덤 위치
```csharp
Vector3 GetRandomPointOnNavMesh(Vector3 center, float distance) {
    Vector3 rnd = center + Random.insideUnitSphere * distance;
    NavMeshHit hit;
    NavMesh.SamplePosition(rnd, out hit, distance, NavMesh.AllAreas);
    return hit.position;
}

void Spawn() {
    Vector3 pos = GetRandomPointOnNavMesh(
        playerTransform.position, maxDistance);
    pos.y += 0.5f;

    int idx = Random.Range(0, items.Length);
    GameObject item = Instantiate(items[idx], pos, Quaternion.identity);
    Destroy(item, 5f);

    lastSpawnTime = Time.time;
    timeBetSpawn  = Random.Range(timeBetSpawnMin, timeBetSpawnMax);
}
```

### 아이템 스크립트 패턴 (IItem 구현)
```csharp
// Coin
public void Use(GameObject target) {
    GameManager.instance.AddScore(coinValue);
    Destroy(gameObject);
}
// HealthPack
public void Use(GameObject target) {
    LivingEntity e = target.GetComponent<LivingEntity>();
    if (e != null) e.RestoreHealth(healAmount);
    Destroy(gameObject);
}
// AmmoPack
public void Use(GameObject target) {
    PlayerShooter ps = target.GetComponent<PlayerShooter>();
    if (ps != null) ps.gun.ammoRemain += ammoAmount;
    Destroy(gameObject);
}
```

### 포스트 프로세싱 설정
```
1. Main Camera → Post-process Layer 추가
   Layer: PostProcessing / Anti-aliasing: FXAA

2. + > 3D Object > Post-process Volume 생성
   Layer: PostProcessing / Is Global: ✓
   Profile: Global Profile 할당

렌더링 경로 (Camera)
  포워드 렌더링 : 광원 최대 4개, 메모리↓
  디퍼드 셰이딩 : 광원 무제한, 포스트프로세싱 최적  ← 권장
  Allow MSAA   : Off (디퍼드와 함께 사용)
```

### 포스트 프로세싱 효과 목록
| 효과 | 설명 |
|---|---|
| 블룸 (Bloom) | 밝은 경계에서 빛 산란 ('뽀샤시') |
| 모션 블러 | 빠른 물체 잔상 |
| 컬러 그레이딩 | 색상·대비·감마 교정 (인스타 필터) |
| 비네트 (Vignette) | 화면 가장자리 어둡게 → 포커스 강조 |
| 그레인 (Grain) | 필름 노이즈 입자 |
| 색 수차 (Chromatic Aberration) | 경계 번짐, 방사능 효과 |

---

## 빠른 참조 — 메서드 · 용어 총정리

| 메서드 / 용어 | 설명 |
|---|---|
| `Awake()` | Start보다 1프레임 앞서 실행 |
| `Start()` | 최초 1회만 실행 |
| `OnEnable()` | 컴포넌트 활성화될 때마다 실행 |
| `FixedUpdate()` | 물리 갱신 주기(기본 0.02s)마다 실행 |
| `OnTriggerEnter2D` | 2D 트리거 충돌 시작 |
| `OnCollisionEnter2D` | 2D 일반 충돌 시작 |
| `OnTriggerStay` | 충돌 지속 중 매 프레임 |
| `OnCollisionExit2D` | 2D 충돌 종료 |
| `virtual` | 자식이 override 가능한 메서드 |
| `override` | 부모 메서드 재정의 |
| `base.Method()` | 부모의 원형 메서드 호출 |
| `event Action` | 외부에서 발동 불가 델리게이트 |
| `Action` | 입출력 없는 델리게이트 타입 |
| `static` 변수 | 메모리에 하나만 존재, 모두 공유 |
| 싱글턴 | static instance 패턴, 하나만 존재 + 어디서나 접근 |
| `ScriptableObject` | 데이터 에셋, MonoBehaviour 아님 |
| `[CreateAssetMenu]` | 에셋 생성 메뉴 추가 특성 |
| `IEnumerator` | 코루틴의 반환 타입 |
| `yield return` | 코루틴 일시 정지 |
| `StartCoroutine()` | 코루틴 시작 |
| `Physics.Raycast()` | 광선 충돌 검사 |
| `out` 키워드 | 메서드가 추가 결과를 out 변수에 반환 |
| `RaycastHit` | 레이캐스트 충돌 정보 구조체 |
| `NavMesh.SamplePosition` | NavMesh 위 가장 가까운 점 탐색 |
| `Random.insideUnitSphere` | 반지름 1인 구 내부 랜덤 벡터 |
| `AvatarIKGoal` | 손/발 IK 대상 (LeftHand, RightHand …) |
| `AvatarIKHint` | 팔꿈치/무릎 IK 힌트 |
| `Mathf.RoundToInt()` | 실수 → 정수 반올림 |
| `Physics.OverlapSphere()` | 구 범위 내 콜라이더 배열 반환 |
| `protected` | 클래스 외부 접근 X, 자식 클래스 접근 O |
| `List<T>` | 가변 크기 배열, Add/Remove/Count |
| 람다식 `() => {}` | 익명 함수, 이벤트 등록에 자주 사용 |
