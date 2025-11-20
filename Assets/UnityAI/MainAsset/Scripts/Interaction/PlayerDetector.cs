using UnityEngine;

namespace UnityAI.Interaction
{
    /// <summary>
    /// 플레이어 감지 방식
    /// </summary>
    public enum DetectionMode
    {
        Distance,   // 거리 기반 감지
        Trigger,    // 물리 트리거 기반 감지
        Both        // 둘 다 사용
    }
    
    /// <summary>
    /// 플레이어 감지를 담당하는 컴포넌트
    /// IInteractable 오브젝트와 함께 사용하여 플레이어의 진입/퇴장을 감지합니다.
    /// </summary>
    [RequireComponent(typeof(IInteractable))]
    public class PlayerDetector : MonoBehaviour
    {
        [Header("감지 방식")]
        [SerializeField] private DetectionMode detectionMode = DetectionMode.Distance;
        
        [Header("거리 기반 설정")]
        [SerializeField] private float detectionRange = 3f;
        [SerializeField] private float checkInterval = 0.2f; // 거리 체크 간격 (최적화)
        
        [Header("트리거 기반 설정")]
        [Tooltip("트리거 콜라이더를 자동으로 생성할지 여부")]
        [SerializeField] private bool autoCreateTrigger = true;
        [SerializeField] private Vector3 triggerSize = new Vector3(3f, 2f, 3f);
        [SerializeField] private Vector3 triggerCenter = Vector3.zero;
        
        [Header("디버그")]
        [SerializeField] private bool showDebugLogs = false;
        [SerializeField] private bool showGizmos = true;
        
        private IInteractable interactable;
        private Transform player;
        private bool playerInRange = false;
        private float lastCheckTime = 0f;
        private int playersInTrigger = 0; // 트리거 내 플레이어 수
        
        private void Start()
        {
            // IInteractable 컴포넌트 가져오기
            interactable = GetComponent<IInteractable>();
            if (interactable == null)
            {
                Debug.LogError($"[PlayerDetector] {gameObject.name}: IInteractable 컴포넌트를 찾을 수 없습니다!");
                enabled = false;
                return;
            }
            
            // 플레이어 찾기
            FindPlayer();
            
            // 트리거 기반 모드면 콜라이더 설정
            if (detectionMode == DetectionMode.Trigger || detectionMode == DetectionMode.Both)
            {
                SetupTrigger();
            }
        }
        
        private void Update()
        {
            // 거리 기반 감지
            if (detectionMode == DetectionMode.Distance || detectionMode == DetectionMode.Both)
            {
                // 최적화: 일정 간격으로만 체크
                if (Time.time - lastCheckTime >= checkInterval)
                {
                    lastCheckTime = Time.time;
                    CheckDistanceToPlayer();
                }
            }
        }
        
        /// <summary>
        /// 플레이어 오브젝트 찾기
        /// </summary>
        private void FindPlayer()
        {
            // Tag로 찾기
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
                if (showDebugLogs)
                    Debug.Log($"[PlayerDetector] 플레이어 찾음: {player.name}");
            }
            else
            {
                // PlayerMovement 컴포넌트로 찾기
                PlayerMovement playerMovement = FindFirstObjectByType<PlayerMovement>();
                if (playerMovement != null)
                {
                    player = playerMovement.transform;
                    if (showDebugLogs)
                        Debug.Log($"[PlayerDetector] 플레이어 찾음 (PlayerMovement): {player.name}");
                }
                else
                {
                    Debug.LogWarning($"[PlayerDetector] {gameObject.name}: 플레이어를 찾을 수 없습니다!");
                }
            }
        }
        
        /// <summary>
        /// 트리거 콜라이더 설정
        /// </summary>
        private void SetupTrigger()
        {
            if (autoCreateTrigger)
            {
                // 기존 콜라이더 확인
                Collider existingCollider = GetComponent<Collider>();
                if (existingCollider == null)
                {
                    // BoxCollider 추가
                    BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
                    boxCollider.isTrigger = true;
                    boxCollider.size = triggerSize;
                    boxCollider.center = triggerCenter;
                    
                    if (showDebugLogs)
                        Debug.Log($"[PlayerDetector] BoxCollider 트리거 자동 생성");
                }
                else
                {
                    // 트리거로 설정
                    existingCollider.isTrigger = true;
                    if (showDebugLogs)
                        Debug.Log($"[PlayerDetector] 기존 콜라이더를 트리거로 설정");
                }
            }
        }
        
        /// <summary>
        /// 플레이어와의 거리 체크
        /// </summary>
        private void CheckDistanceToPlayer()
        {
            if (player == null)
            {
                FindPlayer();
                return;
            }
            
            float distance = Vector3.Distance(transform.position, player.position);
            bool inRange = distance <= detectionRange;
            
            // 상태 변경 시에만 이벤트 호출
            if (inRange && !playerInRange)
            {
                playerInRange = true;
                NotifyPlayerEnter();
            }
            else if (!inRange && playerInRange)
            {
                playerInRange = false;
                NotifyPlayerExit();
            }
        }
        
        /// <summary>
        /// 트리거 진입
        /// </summary>
        private void OnTriggerEnter(Collider other)
        {
            if (detectionMode == DetectionMode.Distance)
                return;
                
            if (other.CompareTag("Player"))
            {
                playersInTrigger++;
                
                if (player == null)
                    player = other.transform;
                
                // 트리거 전용 모드이거나, Both 모드에서 거리 감지가 아직 안됐을 때
                if (detectionMode == DetectionMode.Trigger || !playerInRange)
                {
                    playerInRange = true;
                    NotifyPlayerEnter();
                }
            }
        }
        
        /// <summary>
        /// 트리거 퇴장
        /// </summary>
        private void OnTriggerExit(Collider other)
        {
            if (detectionMode == DetectionMode.Distance)
                return;
                
            if (other.CompareTag("Player"))
            {
                playersInTrigger--;
                
                // 트리거 안에 플레이어가 없을 때
                if (playersInTrigger <= 0)
                {
                    playersInTrigger = 0;
                    
                    if (detectionMode == DetectionMode.Trigger || playerInRange)
                    {
                        playerInRange = false;
                        NotifyPlayerExit();
                    }
                }
            }
        }
        
        /// <summary>
        /// IInteractable에 플레이어 진입 알림
        /// </summary>
        private void NotifyPlayerEnter()
        {
            if (showDebugLogs)
                Debug.Log($"[PlayerDetector] {gameObject.name}: 플레이어 범위 진입");
            
            interactable?.OnPlayerEnterRange(player);
        }
        
        /// <summary>
        /// IInteractable에 플레이어 퇴장 알림
        /// </summary>
        private void NotifyPlayerExit()
        {
            if (showDebugLogs)
                Debug.Log($"[PlayerDetector] {gameObject.name}: 플레이어 범위 퇴장");
            
            interactable?.OnPlayerExitRange(player);
        }
        
        /// <summary>
        /// Gizmos로 감지 범위 시각화
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            if (!showGizmos)
                return;
            
            // 거리 기반 범위
            if (detectionMode == DetectionMode.Distance || detectionMode == DetectionMode.Both)
            {
                Gizmos.color = playerInRange ? Color.green : Color.yellow;
                Gizmos.DrawWireSphere(transform.position, detectionRange);
            }
            
            // 트리거 범위
            if (detectionMode == DetectionMode.Trigger || detectionMode == DetectionMode.Both)
            {
                Gizmos.color = playerInRange ? Color.cyan : Color.blue;
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawWireCube(triggerCenter, triggerSize);
            }
        }
    }
}

