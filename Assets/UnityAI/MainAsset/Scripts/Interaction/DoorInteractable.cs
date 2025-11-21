using UnityEngine;

namespace UnityAI.Interaction
{
    /// <summary>
    /// 문 상호작용을 처리하는 클래스
    /// F키를 눌러 문을 열고 닫습니다.
    /// </summary>
    public class DoorInteractable : InteractableBase
    {
    [Header("문 설정")]
    [SerializeField] private float openAngle = 90f; // 열릴 때 회전 각도
    [SerializeField] private float rotationSpeed = 3f; // 회전 속도
    
    [Header("키 필요 설정")]
    [SerializeField] private bool requireKey = false; // 키가 필요한지 여부
    [SerializeField] private string requiredKeyId = "YellowKey"; // 필요한 키 ID
    [SerializeField] private string noKeyHintText = "키를 찾아야 문을 열 수 있습니다"; // 키 없을 때 힌트
    
    [Header("트리거 설정")]
    [Tooltip("별도의 트리거 오브젝트 (회전하지 않고 위치만 따라감)")]
    [SerializeField] private Transform separateTrigger; // 별도 트리거 Transform
    [SerializeField] private bool lockTriggerRotation = true; // 트리거 회전 고정 여부
    
    [Header("자동 닫힘 설정")]
    [SerializeField] private bool autoClose = true; // 자동으로 닫을지 여부
    [SerializeField] private float autoCloseDelay = 3f; // 열린 후 자동으로 닫히는 시간
    
    [Header("문 사운드")]
    [SerializeField] private AudioClip openSound; // 문 열리는 소리
    [SerializeField] private AudioClip closeSound; // 문 닫히는 소리
    [SerializeField] private AudioClip lockedSound; // 잠긴 문 소리 (키 없을 때)
    
    private Quaternion closedRotation; // 닫힌 상태의 회전값
    private Quaternion openRotation; // 열린 상태의 회전값
    private bool isOpen = false; // 문이 열려있는지 여부
    private float currentOpenAngle = 90f; // 현재 열림 각도 (방향 포함)
    private float autoCloseTimer = 0f; // 자동 닫힘 타이머
    private Quaternion triggerInitialRotation; // 트리거 초기 회전값
        
        // 키가 필요하면 키 소유 여부 확인, 아니면 항상 상호작용 가능
        public override bool CanInteract => !requireKey || HasRequiredKey();
        
    protected override void Start()
    {
        base.Start();
        
        // 초기 회전값 저장
        closedRotation = transform.localRotation;
        
        // 별도 트리거가 있으면 초기 회전값 저장
        if (separateTrigger != null)
        {
            triggerInitialRotation = separateTrigger.rotation;
            
            if (showDebugLogs)
                Debug.Log($"[DoorInteractable] {gameObject.name}: 별도 트리거 설정됨 ({separateTrigger.name})");
        }
        
        // 기본 힌트 텍스트 설정
        if (string.IsNullOrEmpty(interactionHintText))
        {
            interactionHintText = "F키를 눌러 문 열기";
        }
    }
        
    protected override void Update()
    {
        base.Update();
        
        // 목표 회전값 결정
        Quaternion targetRotation = isOpen ? openRotation : closedRotation;
        
        // 현재 회전값을 목표 회전값으로 부드럽게 회전
        transform.localRotation = Quaternion.Slerp(
            transform.localRotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
        
        // 별도 트리거의 회전 고정 (위치는 따라가지만 회전은 고정)
        if (separateTrigger != null && lockTriggerRotation)
        {
            separateTrigger.rotation = triggerInitialRotation;
        }
        
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
        /// 문과 상호작용 (F키 입력 시)
        /// </summary>
        public override void Interact()
        {
            // 키가 필요하지만 없는 경우
            if (requireKey && !HasRequiredKey())
            {
                PlaySound(lockedSound);
                
                if (showDebugLogs)
                    Debug.Log($"[DoorInteractable] {gameObject.name}: 키가 필요합니다!");
                
                return;
            }
            
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
        /// 필요한 키를 소유하고 있는지 확인
        /// </summary>
        private bool HasRequiredKey()
        {
            return KeyInventory.Instance.HasKey(requiredKeyId);
        }
        
        /// <summary>
        /// 문 열기
        /// </summary>
        private void OpenDoor()
        {
            if (currentPlayer != null)
            {
                // 플레이어의 접근 방향 감지
                Vector3 playerDirection = currentPlayer.position - transform.position;
                
                // 문의 forward 방향과 플레이어 방향의 내적(dot product) 계산
                float dotProduct = Vector3.Dot(transform.forward, playerDirection.normalized);
                
                // 내적이 양수면 플레이어가 문의 앞쪽에서 접근 (한쪽으로 열림)
                // 내적이 음수면 플레이어가 문의 뒤쪽에서 접근 (반대쪽으로 열림)
                currentOpenAngle = dotProduct > 0 ? openAngle : -openAngle;
                
                // 열릴 때 회전 각도 계산 (플레이어 방향 기반)
                openRotation = closedRotation * Quaternion.Euler(0f, currentOpenAngle, 0f);
            }
            else
            {
                // 플레이어 정보가 없으면 기본 방향으로
                currentOpenAngle = openAngle;
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
            
            if (showDebugLogs)
                Debug.Log($"[DoorInteractable] {gameObject.name}: 문 열림 (각도: {currentOpenAngle}°)");
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
            if (playerInRange && uiManager != null && showInteractionHint)
            {
                uiManager.ShowInteractionHint(GetInteractionHintText());
            }
            
            if (showDebugLogs)
                Debug.Log($"[DoorInteractable] {gameObject.name}: 문 닫힘");
        }
        
        /// <summary>
        /// 플레이어가 범위에 진입했을 때
        /// </summary>
        protected override void OnPlayerEnter(Transform player)
        {
            // 키가 필요하지만 없는 경우, 힌트만 표시 (상호작용은 불가)
            if (requireKey && !HasRequiredKey())
            {
                if (uiManager != null && showInteractionHint)
                {
                    uiManager.ShowInteractionHint(noKeyHintText);
                }
                
                if (showDebugLogs)
                    Debug.Log($"[DoorInteractable] {gameObject.name}: 플레이어 진입 (키 없음)");
            }
        }
        
        /// <summary>
        /// 플레이어가 범위를 벗어났을 때
        /// </summary>
        protected override void OnPlayerExit(Transform player)
        {
            base.OnPlayerExit(player);
            
            // 문이 열려있으면 닫기
            if (isOpen && autoClose)
            {
                CloseDoor();
            }
        }
        
        /// <summary>
        /// 상호작용 힌트 텍스트
        /// </summary>
        public override string GetInteractionHintText()
        {
            // 키가 필요하지만 없는 경우 - F키 힌트 표시 안함
            if (requireKey && !HasRequiredKey())
            {
                return ""; // 빈 문자열 반환 (OnPlayerEnter에서 noKeyHintText 표시)
            }
            
            return isOpen ? "F키를 눌러 문 닫기" : "F키를 눌러 문 열기";
        }
        
        /// <summary>
        /// Scene 뷰에서 문 상태 시각화
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = isOpen ? Color.green : Color.red;
            Gizmos.DrawRay(transform.position, transform.forward * 2f);
            
            // 열린 각도 표시
            if (Application.isPlaying && isOpen)
            {
                Gizmos.color = Color.cyan;
                Vector3 openDirection = Quaternion.Euler(0f, currentOpenAngle, 0f) * transform.forward;
                Gizmos.DrawRay(transform.position, openDirection * 2f);
            }
        }
    }
}

