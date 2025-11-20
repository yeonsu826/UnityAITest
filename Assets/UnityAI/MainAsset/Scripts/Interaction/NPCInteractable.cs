using UnityEngine;

namespace UnityAI.Interaction
{
    /// <summary>
    /// NPC와의 상호작용을 처리하는 클래스
    /// F키를 눌러 NPC 설명 UI를 표시합니다.
    /// </summary>
    public class NPCInteractable : InteractableBase
    {
        [Header("NPC 설정")]
        [SerializeField] private NPCData npcData;
        
        [Header("자동 닫힘 설정")]
        [SerializeField] private float autoCloseDistance = 5f; // UI 자동 닫힘 거리
        
        private bool isUIActive = false;
        
        public override bool CanInteract => !isUIActive && npcData != null;
        
        protected override void Start()
        {
            base.Start();
            
            // 초기 설정 확인
            if (showDebugLogs)
            {
                Debug.Log($"[NPCInteractable] 초기화 완료 - NPC Data: {(npcData != null ? "✓" : "✗")}, " +
                         $"UI Manager: {(uiManager != null ? "✓" : "✗")}");
            }
            
            if (npcData == null)
            {
                Debug.LogWarning($"[NPCInteractable] {gameObject.name}: NPC Data가 설정되지 않았습니다!");
            }
        }
        
        protected override void Update()
        {
            base.Update();
            
            // UI가 활성화된 상태에서 거리 체크 (자동 닫힘)
            if (isUIActive && currentPlayer != null)
            {
                float distance = Vector3.Distance(transform.position, currentPlayer.position);
                if (distance > autoCloseDistance)
                {
                    CloseUI();
                }
            }
        }
        
        /// <summary>
        /// NPC와 상호작용 (F키 입력 시)
        /// </summary>
        public override void Interact()
        {
            if (npcData == null)
            {
                Debug.LogWarning($"[NPCInteractable] {gameObject.name}: NPC Data가 없어서 상호작용할 수 없습니다.");
                return;
            }
            
            if (showDebugLogs)
                Debug.Log($"[NPCInteractable] {gameObject.name}: 상호작용 - {npcData.name}");
            
            // 효과음 재생
            PlayInteractionSound();
            
            // UI 표시
            ShowUI();
        }
        
        /// <summary>
        /// NPC 설명 UI 표시
        /// </summary>
        private void ShowUI()
        {
            if (uiManager == null)
            {
                Debug.LogError($"[NPCInteractable] {gameObject.name}: UI Manager가 없습니다!");
                return;
            }
            
            if (showDebugLogs)
                Debug.Log($"[NPCInteractable] UI 표시: {npcData.name}");
            
            // 자신을 함께 전달 (UI 닫힘 콜백을 위해)
            uiManager.ShowNPCDescription(npcData, this);
            isUIActive = true;
            
            // UI가 열렸으니 힌트 숨김
            if (uiManager != null)
            {
                uiManager.HideInteractionHint();
            }
        }
        
        /// <summary>
        /// NPC 설명 UI 닫기
        /// </summary>
        private void CloseUI()
        {
            if (uiManager != null)
            {
                uiManager.HideNPCDescription();
                isUIActive = false;
                
                // 플레이어가 아직 범위 안에 있으면 힌트 다시 표시
                if (playerInRange && showInteractionHint)
                {
                    uiManager.ShowInteractionHint(GetInteractionHintText());
                }
            }
            
            if (showDebugLogs)
                Debug.Log($"[NPCInteractable] UI 닫힌");
        }
        
        /// <summary>
        /// 외부에서 UI를 닫을 때 사용 (UI의 닫기 버튼 등)
        /// </summary>
        public void OnUIClosedExternally()
        {
            isUIActive = false;
            
            if (showDebugLogs)
                Debug.Log($"[NPCInteractable] UI 외부에서 닫힘");
        }
        
        /// <summary>
        /// 플레이어가 범위를 벗어났을 때
        /// </summary>
        protected override void OnPlayerExit(Transform player)
        {
            base.OnPlayerExit(player);
            
            // UI가 열려있으면 닫기
            if (isUIActive)
            {
                CloseUI();
            }
        }
        
        /// <summary>
        /// 상호작용 힌트 텍스트
        /// </summary>
        public override string GetInteractionHintText()
        {
            if (npcData != null && !string.IsNullOrEmpty(npcData.name))
            {
                return $"F키를 눌러 {npcData.name}와(과) 대화하기";
            }
            return "F키를 눌러 대화하기";
        }
        
        /// <summary>
        /// Scene 뷰에서 시각화 (선택사항 - PlayerDetector가 이미 표시하므로)
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            // 자동 닫힘 거리 표시 (빨간색)
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, autoCloseDistance);
        }
    }
}

