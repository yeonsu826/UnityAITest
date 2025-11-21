using UnityEngine;

namespace UnityAI.Interaction
{
    /// <summary>
    /// 플레이어가 주울 수 있는 키 아이템
    /// F키를 눌러 획득하고 인벤토리에 추가됩니다.
    /// </summary>
    public class KeyItem : InteractableBase
    {
        [Header("키 설정")]
        [SerializeField] private string keyId = "YellowKey"; // 키의 고유 ID
        [SerializeField] private string keyDisplayName = "노란색 USB 키"; // 표시될 이름
        
        [Header("획득 효과")]
        [SerializeField] private bool destroyOnPickup = true; // 주운 후 오브젝트 제거
        [SerializeField] private float destroyDelay = 0.1f; // 제거 지연 시간
        [SerializeField] private GameObject pickupEffect; // 획득 이펙트 (선택)
        
        [Header("회전 효과")]
        [SerializeField] private bool rotateKey = true; // 키 회전 효과
        [SerializeField] private float rotationSpeed = 50f; // 회전 속도
        [SerializeField] private Vector3 rotationAxis = Vector3.up; // 회전 축
        
        [Header("떠다니는 효과")]
        [SerializeField] private bool floatKey = true; // 위아래 떠다니기
        [SerializeField] private float floatAmplitude = 0.2f; // 떠다니는 높이
        [SerializeField] private float floatSpeed = 1f; // 떠다니는 속도
        
        private Vector3 initialPosition;
        private float floatOffset = 0f;
        private bool isPickedUp = false;
        
        public override bool CanInteract => !isPickedUp;
        
        protected override void Start()
        {
            base.Start();
            
            // 초기 위치 저장
            initialPosition = transform.position;
            
            // 기본 힌트 텍스트 설정
            if (string.IsNullOrEmpty(interactionHintText))
            {
                interactionHintText = $"F키를 눌러 {keyDisplayName} 획득";
            }
            
            // 랜덤 오프셋으로 자연스러운 효과
            floatOffset = Random.Range(0f, Mathf.PI * 2f);
        }
        
        protected override void Update()
        {
            base.Update();
            
            if (!isPickedUp)
            {
                // 회전 효과
                if (rotateKey)
                {
                    transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime, Space.World);
                }
                
                // 떠다니는 효과
                if (floatKey)
                {
                    floatOffset += floatSpeed * Time.deltaTime;
                    float newY = initialPosition.y + Mathf.Sin(floatOffset) * floatAmplitude;
                    transform.position = new Vector3(transform.position.x, newY, transform.position.z);
                }
            }
        }
        
        /// <summary>
        /// 키 획득 (F키 입력 시)
        /// </summary>
        public override void Interact()
        {
            if (isPickedUp)
                return;
            
            // 키 획득
            PickupKey();
        }
        
        /// <summary>
        /// 키를 인벤토리에 추가
        /// </summary>
        private void PickupKey()
        {
            isPickedUp = true;
            
            // 인벤토리에 키 추가
            KeyInventory.Instance.AddKey(keyId);
            
            // 사운드 재생
            PlayInteractionSound();
            
            // 획득 이펙트 생성
            if (pickupEffect != null)
            {
                Instantiate(pickupEffect, transform.position, Quaternion.identity);
            }
            
            // UI 힌트 숨김
            if (uiManager != null)
            {
                uiManager.HideInteractionHint();
            }
            
            if (showDebugLogs)
                Debug.Log($"[KeyItem] {keyDisplayName} 획득! (ID: {keyId})");
            
            // 오브젝트 제거
            if (destroyOnPickup)
            {
                Destroy(gameObject, destroyDelay);
            }
            else
            {
                // 제거하지 않으면 비활성화
                gameObject.SetActive(false);
            }
        }
        
        /// <summary>
        /// 상호작용 힌트 텍스트
        /// </summary>
        public override string GetInteractionHintText()
        {
            return interactionHintText;
        }
        
        /// <summary>
        /// 키 ID 반환
        /// </summary>
        public string GetKeyId()
        {
            return keyId;
        }
        
        /// <summary>
        /// Scene 뷰에서 키 위치 시각화
        /// </summary>
        private void OnDrawGizmos()
        {
            if (!isPickedUp)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(transform.position, 0.5f);
                
                // 키 ID 표시
                #if UNITY_EDITOR
                UnityEditor.Handles.Label(transform.position + Vector3.up * 0.7f, $"Key: {keyId}");
                #endif
            }
        }
    }
}

