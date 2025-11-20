using UnityEngine;

/// <summary>
/// 플레이어가 트리거 영역에 들어가서 F키를 누르면 회전하는 문
/// BoxCollider를 트리거로 사용하여 플레이어 감지
/// </summary>
[RequireComponent(typeof(BoxCollider))]
public class AutoRotatingDoor : MonoBehaviour
{
    [Header("문 설정")]
    [SerializeField] private float openAngle = 90f; // 열릴 때 회전 각도
    [SerializeField] private float rotationSpeed = 3f; // 회전 속도
    [SerializeField] private KeyCode interactionKey = KeyCode.F; // 상호작용 키

    [Header("자동 닫힘 설정")]
    [SerializeField] private bool autoClose = true; // 자동으로 닫을지 여부
    [SerializeField] private float autoCloseDelay = 3f; // 열린 후 자동으로 닫히는 시간

    [Header("사운드 (선택사항)")]
    [SerializeField] private AudioClip openSound; // 문 열리는 소리
    [SerializeField] private AudioClip closeSound; // 문 닫히는 소리

    [Header("UI (선택사항)")]
    [SerializeField] private NPCUIManager uiManager; // 힌트 UI용

    private Quaternion closedRotation; // 닫힌 상태의 회전값
    private Quaternion openRotation; // 열린 상태의 회전값
    private bool isOpen = false; // 문이 열려있는지 여부
    private bool playerInRange = false; // 플레이어가 범위 안에 있는지
    private int playerCount = 0; // 트리거 안의 플레이어 수
    private AudioSource audioSource; // 오디오 소스 컴포넌트
    private float currentOpenAngle = 90f; // 현재 열림 각도 (방향 포함)
    private float autoCloseTimer = 0f; // 자동 닫힘 타이머

    private void Start()
    {
        // 초기 회전값 저장
        closedRotation = transform.localRotation;

        // BoxCollider를 트리거로 설정
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        boxCollider.isTrigger = true;
        boxCollider.size = new Vector3(1f, 2f, 3f); // 트리거 크기 조정 (필요시 Inspector에서 수정)

        // AudioSource 컴포넌트 가져오기 또는 추가
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && (openSound != null || closeSound != null))
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        // UI Manager 찾기
        if (uiManager == null)
        {
            uiManager = FindFirstObjectByType<NPCUIManager>();
        }
    }

    private void Update()
    {
        // F키 입력 체크
        if (playerInRange && Input.GetKeyDown(interactionKey))
        {
            ToggleDoor();
        }

        // 목표 회전값 결정
        Quaternion targetRotation = isOpen ? openRotation : closedRotation;
        
        // 현재 회전값을 목표 회전값으로 부드럽게 회전
        transform.localRotation = Quaternion.Slerp(
            transform.localRotation, 
            targetRotation, 
            rotationSpeed * Time.deltaTime
        );

        // 자동 닫힘 타이머
        if (isOpen && autoClose)
        {
            autoCloseTimer += Time.deltaTime;
            if (autoCloseTimer >= autoCloseDelay)
            {
                CloseDoor();
            }
        }
    }

    /// <summary>
    /// 문 열기/닫기 토글
    /// </summary>
    private void ToggleDoor()
    {
        if (isOpen)
        {
            CloseDoor();
        }
        else
        {
            OpenDoor();
        }
    }

    /// <summary>
    /// 문 열기
    /// </summary>
    private void OpenDoor()
    {
        // 플레이어 찾기
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            // 플레이어의 접근 방향 감지
            Vector3 playerDirection = player.transform.position - transform.position;
            
            // 문의 forward 방향과 플레이어 방향의 내적(dot product) 계산
            float dotProduct = Vector3.Dot(transform.forward, playerDirection.normalized);
            
            // 내적이 양수면 플레이어가 문의 앞쪽에서 접근 (한쪽으로 열림)
            // 내적이 음수면 플레이어가 문의 뒤쪽에서 접근 (반대쪽으로 열림)
            currentOpenAngle = dotProduct > 0 ? openAngle : -openAngle;
            
            // 열릴 때 회전 각도 계산 (플레이어 방향 기반)
            openRotation = closedRotation * Quaternion.Euler(0f, currentOpenAngle, 0f);
        }

        isOpen = true;
        autoCloseTimer = 0f;
        PlaySound(openSound);

        // 힌트 UI 숨김
        if (uiManager != null)
        {
            uiManager.HideInteractionHint();
        }
    }

    /// <summary>
    /// 문 닫기
    /// </summary>
    private void CloseDoor()
    {
        isOpen = false;
        autoCloseTimer = 0f;
        PlaySound(closeSound);

        // 플레이어가 아직 범위 안에 있으면 힌트 다시 표시
        if (playerInRange && uiManager != null)
        {
            uiManager.ShowInteractionHint("F키를 눌러 문 열기");
        }
    }

    /// <summary>
    /// 플레이어가 트리거 영역에 들어올 때 호출
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerCount++;
            playerInRange = true;
            
            // 문이 닫혀있을 때만 힌트 표시
            if (!isOpen && uiManager != null)
            {
                uiManager.ShowInteractionHint("F키를 눌러 문 열기");
            }
        }
    }

    /// <summary>
    /// 플레이어가 트리거 영역에서 나갈 때 호출
    /// </summary>
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerCount--;
            
            // 트리거 안에 플레이어가 없으면
            if (playerCount <= 0)
            {
                playerCount = 0;
                playerInRange = false;

                // 힌트 UI 숨김
                if (uiManager != null)
                {
                    uiManager.HideInteractionHint();
                }

                // 문이 열려있으면 닫기
                if (isOpen)
                {
                    CloseDoor();
                }
            }
        }
    }

    /// <summary>
    /// 사운드 재생
    /// </summary>
    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    /// <summary>
    /// Scene 뷰에서 트리거 범위 시각화
    /// </summary>
    private void OnDrawGizmos()
    {
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        if (boxCollider != null)
        {
            Gizmos.color = isOpen ? Color.green : Color.yellow;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(boxCollider.center, boxCollider.size);
        }
    }
}

