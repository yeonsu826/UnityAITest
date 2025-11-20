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
        
        [Header("자동 닫힘 설정")]
        [SerializeField] private bool autoClose = true; // 자동으로 닫을지 여부
        [SerializeField] private float autoCloseDelay = 3f; // 열린 후 자동으로 닫히는 시간
        
        [Header("문 사운드")]
        [SerializeField] private AudioClip openSound; // 문 열리는 소리
        [SerializeField] private AudioClip closeSound; // 문 닫히는 소리
        
        private Quaternion closedRotation; // 닫힌 상태의 회전값
        private Quaternion openRotation; // 열린 상태의 회전값
        private bool isOpen = false; // 문이 열려있는지 여부
        private float currentOpenAngle = 90f; // 현재 열림 각도 (방향 포함)
        private float autoCloseTimer = 0f; // 자동 닫힘 타이머
        
        public override bool CanInteract => true; // 문은 항상 상호작용 가능
        
        protected override void Start()
        {
            base.Start();
            
            // 초기 회전값 저장
            closedRotation = transform.localRotation;
            
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

