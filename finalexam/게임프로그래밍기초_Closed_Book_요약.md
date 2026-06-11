# 게임프로그래밍기초 기말시험 요약 (Closed Book)
**9week ~ 14week | OX · 객관식 · 단답형 · 간단 코딩 대비**

---

## 9주차 ① AppleCatcher — 3D 게임 제작

### 게임 설계 개요
- 떨어지는 사과/폭탄을 바구니로 받는 3D 게임 (무대 3×3, 30초)
- 사과·폭탄은 재생산 → **프리팹(Prefab) 필수**
- 시간 경과에 따른 난이도 조정: 생성 간격·낙하속도·생성 비율 변경

### 충돌 판정 핵심

| 오브젝트 | 필요 컴포넌트 | 설정 |
|---|---|---|
| 사과·폭탄 | Sphere Collider + APPLE/BOMB 태그 | Center(0,0.25,0) Radius(0.25) |
| 바구니 | Rigidbody (IsKinematic ✓) + Box Collider (IsTrigger ✓) | Center(0,0.5,0) Size(0.5,0.1,0.5) |
| 스테이지 | Box Collider | Size(3,0.1,3) |

- `OnTriggerEnter()`에서 `tag` 비교 → 점수 증감
- 효과음: basket에 `AudioSource` 추가 → `PlayOneShot(clip)` 사용

### 제너레이터 (ItemGenerator.cs)
- 빈 오브젝트에 ItemGenerator.cs 링크, `public` 변수로 apple/bombPrefab 연결
- `public SetParameters()` → 외부에서 생성간격·낙하속도·생성비율 조절 가능
- `Random.Range`로 위치 & 아이템 종류 랜덤 선택

### GameManager / UI 갱신
- `using UnityEngine.UI;` 선언 **필수**
- BasketController → `GameManager.AddPoint()` 호출로 득점 전달
- Text 컴포넌트의 `text` 필드 직접 수정하여 UI 갱신
- 게임 종료 조건: 시간 0 이하 → `SceneManager.LoadScene()` 처리

---

## 9주차 ② UniRun — 2D 플레이어 제작 (Chapter 11)

### 2D 프로젝트 특징
- 이미지 → Sprite 타입 임포트, 카메라 **Orthographic** 모드
- **Sprite Sheet**: 단일 이미지에 여러 프레임 포함 → Multiple 모드 + Sprite Editor로 자름

### 컴포넌트 구성

| 컴포넌트 | 주요 설정 |
|---|---|
| Rigidbody 2D | Collision Detection: Continuous / Freeze Rotation Z |
| Circle Collider 2D | Offset(0, -0.57) / Radius 0.2 |
| Audio Source | Play On Awake 체크 해제 |

### 애니메이터 컨트롤러 & FSM
- **유한 상태 머신(FSM)**: 한 번에 하나의 상태, 전이(Transition)로 이동
- 상태: `Run` / `Jump` / `Die`
- 파라미터: `Grounded(Bool)` — Run↔Jump 전이 조건 / `Die(Trigger)` — AnyState→Die
- **Has Exit Time 해제** + **Transition Duration = 0** → 즉각 전환
- Die 애니메이션: **Loop Time 체크 해제**

### PlayerController.cs 핵심 로직
- `jumpCount < 2`이고 마우스 버튼 누를 때 → `velocity.y`에 `jumpForce` 추가 (이단 점프)
- 버튼 떼면 `velocity.y`가 양수일 때 절반으로 감소 (낮은 점프)
- `animator.SetBool("Grounded", isGrounded)`
- `OnTriggerEnter2D()` — Dead 태그 감지 → `Die()` 호출
- `OnCollisionEnter2D()` / `OnCollisionExit2D()` — 법선 벡터로 바닥 감지 (`contact.normal.y > 0.7`)

---

## 10주차 ① 배경 스크롤링 & 게임 매니저 (Chapter 12)

### 정렬 레이어 (Sorting Layer)
- 2D 오브젝트 그리는 순서 결정: `Background` < `Middleground` < `Foreground`
- Sprite Renderer → Sorting Layer에서 지정

### ScrollingObject.cs
```csharp
transform.Translate(Vector3.left * speed * Time.deltaTime);
```

### BackgroundLoop.cs — 무한 스크롤
- `Awake()`: `width = GetComponent<SpriteRenderer>().bounds.size.x`
- `Update()`: `transform.position.x < -width` 이면 `Reposition()`
- `Reposition()`: 현재 위치 + `width * 2` 만큼 오른쪽으로 재배치
- Box Collider 2D `IsTrigger ✓` (다른 오브젝트 밀지 않음)

### 캔버스 스케일러
- **고정 픽셀 크기**: 화면 크기 달라도 UI 픽셀 크기 고정 → 크면 상대적으로 작아 보임
- **Scale With Screen Size**: 기준 해상도(640×360) 기준 자동 확대/축소 ← **권장**

### 싱글턴 패턴
- `static` 변수는 메모리에 하나만 존재, 모든 오브젝트가 공유
- 클래스 이름으로 접근: `GameManager.instance.AddScore(1)`

```csharp
void Awake() {
    if (instance == null) instance = this;
    else Destroy(gameObject);
}
```

### GameManager.cs 핵심 기능
- `isGameover`, `score`, `scoreText(TMP)`, `gameoverUI` 필드
- `AddScore(int s)`: `score += s` → `scoreText.text` 갱신
- `OnPlayerDead()`: `isGameover = true` → `gameoverUI.SetActive(true)`
- `Update()`: `isGameover && 마우스 클릭` → `SceneManager.LoadScene(현재씬)`
- `ScrollingObject`에서 `isGameover` 체크 → true면 이동 정지

---

## 10주차 ② 발판 반복 생성 & 빌드 (Chapter 13)

### OnEnable() vs Start()

| 메서드 | 실행 시점 | 사용 예 |
|---|---|---|
| `Start()` | 게임 시작 시 1회 | 초기화 |
| `OnEnable()` | 컴포넌트 활성화될 때마다 | 오브젝트 풀 재활용 시 리셋 |

### 오브젝트 풀링 (Object Pooling)
- 미리 N개 오브젝트 생성 → '풀'에 보관 → 필요 시 재활용 (Destroy 없음)
- **장점**: 실시간 생성/파괴에 의한 성능 저하 방지
- **단점**: 초기 로딩 길어짐
- PlatformSpawner: count=3 발판 미리 생성 → 화면 밖 나가면 오른쪽으로 재배치

### PlatformSpawner 로직
- `Start()`: `platforms[]` 배열 생성, `Instantiate(prefab) × count`, `lastSpawnTime = 0`
- `Update()`: `Time.time >= lastSpawnTime + timeBetSpawn` → 현재 발판 비활성화 후 즉시 활성화 → 위치 재설정 → `currentIndex = (currentIndex + 1) % count`
- `timeBetSpawn` 랜덤 범위: `timeBetSpawnMin ~ timeBetSpawnMax`

### Platform.cs
- `OnEnable()`: `stepped = false`, 장애물 1/3 확률 활성화
- `OnCollisionEnter2D()`: Player 태그 충돌 && `!stepped` → `GameManager.instance.AddScore(1)`, `stepped = true`

---

## 11주차 좀비 서바이버 — 레벨 아트 & 플레이어 (Chapter 14)

### 라이팅

| GI 모드 | 특징 |
|---|---|
| 베이크된 간접 (Baked Indirect) | 간접광만 구움, 직사광·그림자는 실시간 |
| 섀도우마스크 (Shadowmask, **기본값**) | 간접광 + 그림자 맵 모두 구움 |
| 감산 (Subtractive) | 간접광·직사광·그림자 하나의 라이트맵, 가장 성능 좋음 |

- **라이트맵**: 오브젝트가 빛 받는 모습을 미리 텍스처에 구워둠 → 실시간 연산량 감소
- **글로벌 일루미네이션(GI)**: 직접광 + 간접광(다른 물체 반사) 표현
- **환경광(Ambient)**: 씬 전체에 깔리는 기본 빛 (그림자 없음)
- 라이트 모드: `Baked` / `Realtime` / `Mixed`

### 애니메이터 레이어 & 블렌드 트리
- 레이어 여러 개 → **위에서 아래 순서**로 덮어쓰기 적용 (Base Movement → Upper Body)
- **블렌드 트리**: Move 파라미터 값에 따라 여러 애니메이션 클립 자연스럽게 섞음
- **아바타 마스크**: 특정 신체 부위에만 애니메이션 적용 (상체만)

### 프로퍼티 (Property)
- 변수처럼 보이지만 `get`/`set` 접근자를 가진 특수 메서드
- 자동 변환, 유효성 검사, 접근 제어 분리 가능

```csharp
public float kiloBytes { get { return m_bytes * 0.001f; } }

// 자동 구현 프로퍼티
public float Move { get; private set; }
```

### PlayerMovement.cs 핵심
- `FixedUpdate()`에서 `Move()` / `Rotate()` 호출 → 물리 기반 이동
- `MovePosition()`: 리지드바디의 물리 이동 → 벽 통과 방지

```csharp
Vector3 moveDistance = transform.forward * moveSpeed * playerInput.move * Time.deltaTime;
playerRigidbody.MovePosition(playerRigidbody.position + moveDistance);
```

### 시네머신 (Cinemachine)
- **브레인 카메라 (Brain Camera)**: 진짜 카메라, 씬에 하나
- **가상 카메라 (Virtual Camera)**: 브레인의 분신, 여러 개 가능
- `Follow`: 카메라가 이동하며 추적할 대상 / `Look At`: 카메라가 회전하며 조준할 대상
- **데드존**(회전 X) → **소프트존**(부드럽게 회전) → **하드 리밋**(격하게 회전)

---

## 12주차 총과 슈터 (Chapter 15)

### C# 인터페이스
- 메서드 구현을 강제하는 계약, 이름 앞에 `I` 붙이는 것이 관례
- 상속 클래스는 인터페이스 메서드를 `public`으로 **반드시** 구현
- **느슨한 커플링(Loose Coupling)**: 특정 클래스 구현에 결합되지 않아 유연

```csharp
IItem item = other.GetComponent<IItem>();
if (item != null) item.Use(gameObject);
```

### IDamageable 인터페이스
- `OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)` 메서드 강제
- Gun의 `Shot()`에서 타겟의 `OnDamage()` 호출

### 스크립터블 오브젝트 (ScriptableObject)
- `MonoBehaviour`를 상속하지 않는 **데이터 컨테이너 에셋**
- 여러 오브젝트가 하나의 GunData를 공유 → 데이터 불일치 방지

```csharp
[CreateAssetMenu(fileName = "GunData", menuName = "Scriptable/GunData")]
public class GunData : ScriptableObject { ... }
```

### 코루틴 (Coroutine)
- `IEnumerator` 타입 반환, `yield return`으로 일시 정지
- `StartCoroutine(메서드이름())`으로 실행

```csharp
IEnumerator ShotEffect(Vector3 hitPos) {
    lineRenderer.enabled = true;
    yield return new WaitForSeconds(0.03f);
    lineRenderer.enabled = false;
}
```

### 레이캐스트 (Raycast)
- 보이지 않는 광선을 쏴 충돌하는 콜라이더 탐지
- `out` 키워드: 메서드가 반환값 외 추가 정보를 out 변수에 채워 반환

```csharp
RaycastHit hit;
if (Physics.Raycast(origin, direction, out hit, maxDistance)) {
    IDamageable target = hit.collider.GetComponent<IDamageable>();
    if (target != null) target.OnDamage(damage, hit.point, hit.normal);
}
```

### IK (Inverse Kinematics)
- **FK**: 부모→자식 순서 / **IK**: 자식(손) 위치 먼저 결정 → 부모(팔·어깨)가 맞춤
- `OnAnimatorIK()`에서 `SetIKPositionWeight()`, `SetIKPosition()` 등으로 제어
- `AvatarIKGoal`: RightHand, LeftHand / `AvatarIKHint`: RightElbow, LeftElbow

---

## 13주차 생명체 & 좀비 AI (Chapter 16)

### 다형성 & 오버라이드
- 자식 클래스는 부모 타입으로 취급 가능 (업캐스팅)
- `virtual` 메서드를 자식이 `override`로 재정의
- `base.메서드명()` → 부모의 원형 메서드 호출 후 기능 확장

```csharp
public virtual void Attack() { Debug.Log("Monster"); }

// 자식 클래스
public override void Attack() {
    base.Attack();       // 부모 호출
    Debug.Log("Orc");
}
```

### LivingEntity 기반 클래스
- `IDamageable` 상속 → `OnDamage()` 구현 필수
- 주요 필드: `startingHealth`, `health(프로퍼티)`, `dead(프로퍼티)`, `OnDeath(이벤트)`
- `protected set`: 클래스 외부 불가, 자식 클래스에서는 변경 가능
- `OnEnable()`: `health = startingHealth`, `dead = false` 초기화
- `Die()`: `onDeath` 이벤트 발동 → `dead = true`

### 이벤트 & 델리게이트
- **Action**: 입력·출력 없는 메서드를 등록할 수 있는 델리게이트 타입
- **event 키워드**: 클래스 외부에서 이벤트 발동 불가 (구독만 가능)
- 느슨한 커플링 해소: 이벤트 발생자는 구독자의 구현을 알 필요 없음

```csharp
public event Action onDeath;

onDeath += SaveData;     // 구독
onDeath?.Invoke();       // 발동
```

### 내비게이션 시스템

| 구성 요소 | 역할 |
|---|---|
| NavMesh | 에이전트가 걸어다닐 수 있는 표면 (미리 구워야 함) |
| NavMesh Agent | 경로 계산·이동 컴포넌트, `SetDestination()`으로 목적지 지정 |
| NavMesh Obstacle | 에이전트 경로를 막는 장애물 |
| Off Mesh Link | 끊어진 NavMesh 사이 연결 |

- `navMeshAgent.SetDestination(pos)`: 목적지 설정
- `navMeshAgent.isStopped = true`: 이동 중단

### Zombie AI 핵심
- `UpdatePath()` 코루틴: `while(!dead)` → 0.25초마다 반복
- `Physics.OverlapSphere(위치, 반지름, 레이어마스크)`: 범위 내 콜라이더 배열 반환
- 타겟 발견 시 `SetDestination(targetEntity.transform.position)`
- `OnTriggerStay()`: 공격 주기마다 타겟의 `OnDamage()` 호출
- `Die()`: 모든 콜라이더 비활성화, `isStopped = true`

### UI 슬라이더 (Slider)
- 슬라이더는 모습을 직접 그리지 않음 → Fill Rect 오브젝트 크기를 Value에 따라 조정
- `Image Type: Filled` → 원형 슬라이더 가능
- `Canvas Render Mode: World Space` → 3D 공간에 UI 배치 가능
- `Interactable` 체크 해제 → 사용자 상호작용 불가

---

## 14주차 최종 완성 & 포스트 프로세싱 (Chapter 17)

### 람다식 & 익명 함수
- **익명 함수**: 미리 정의하지 않고 인라인으로 즉석 생성하는 메서드
- **람다 표현식**: 익명 함수를 만드는 표현

```csharp
zombie.onDeath += () => {
    zombies.Remove(zombie);
    Destroy(zombie.gameObject, 10f);
    GameManager.instance.AddScore(100);
};
```

### 리스트 (List\<T\>)
```csharp
List<Zombie> zombies = new List<Zombie>();
zombies.Add(z);      // 추가
zombies.Remove(z);   // 제거
int n = zombies.Count; // 현재 수
```

### ZombieSpawner
- `SpawnWave()`: `wave++` → `spawnCount = Mathf.RoundToInt(wave * 1.5f)` → `CreateZombie()` 반복
- `CreateZombie()`: 랜덤 ZombieData 선택 → `Instantiate` → `Setup(data)` → `onDeath` 이벤트 구독
- 웨이브 완료 조건: `zombies.Count == 0`

### NavMesh 랜덤 위치
- `Random.insideUnitSphere * distance`: 반지름 distance인 구 내부 랜덤 벡터
- `NavMesh.SamplePosition(randomPos, out hit, distance, NavMesh.AllAreas)`: 가장 가까운 NavMesh 위 점 탐색

### 포스트 프로세싱 (Post-Processing)
게임 화면이 최종 출력되기 전 카메라 이미지 버퍼에 삽입하는 추가 처리

| 효과 | 설명 |
|---|---|
| 모션 블러 | 빠른 물체 잔상 |
| 블룸 (Bloom) | 밝은 경계에서 빛 산란 ('뽀샤시') |
| 컬러 그레이딩 | 색상·대비·감마 교정 (인스타 필터) |
| 비네트 | 화면 가장자리 어둡게 |
| 그레인 | 필름 노이즈 효과 |
| 색 수차 | 경계 번짐, 방사능 효과 |

### 렌더링 경로
- **포워드 렌더링**: 전통 방식, 메모리↓ 성능↓, 광원 최대 4개
- **디퍼드 셰이딩**: 라이팅 연산 지연, 광원 수 제한 없음, 포스트 프로세싱 최적

---

## 핵심 개념 OX / 단답형 정리

| 개념 | 핵심 내용 |
|---|---|
| `OnEnable()` | 컴포넌트 활성화될 때마다 실행 (Start()는 1회) |
| 오브젝트 풀링 | 미리 생성 후 재활용 → 실시간 생성/파괴 X |
| 싱글턴 | static 변수 + instance 패턴 → 하나만 존재, 어디서나 접근 |
| FSM | 유한 상태 머신, 한 번에 하나의 상태, 전이로 이동 |
| 코루틴 | `IEnumerator` + `yield` → 대기 시간 포함 가능 |
| 레이캐스트 | `Physics.Raycast()` + `out RaycastHit` → 광선 충돌 검사 |
| IK vs FK | IK: 자식→부모 / FK: 부모→자식 |
| 인터페이스 | 메서드 구현 강제, 느슨한 커플링 |
| ScriptableObject | 데이터 에셋, MonoBehaviour 아님 |
| NavMesh | 미리 구워야 함, Agent로 경로 계산 |
| 블렌드 트리 | 파라미터 값에 따라 애니메이션 클립 혼합 |
| 디퍼드 셰이딩 | 포스트 프로세싱 최적, 광원 수 제한 없음 |
| `event` 키워드 | 외부에서 발동 불가 (구독만 가능) |
| `protected` | 클래스 외부 접근 X, 자식 클래스 접근 O |

> ⚠️ **시험 전 꼭 확인**: `OnTriggerEnter2D` vs `OnCollisionEnter2D` 차이 / `Awake` → `OnEnable` → `Start` 실행 순서 / `static` 변수의 동작 원리
