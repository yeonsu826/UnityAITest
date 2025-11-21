using UnityEngine;

namespace UnityAI.Interaction
{
    /// <summary>
    /// IInteractable의 공통 기능을 구현한 추상 베이스 클래스
    /// 오디오, UI 힌트, 플레이어 감지 등의 공통 기능을 제공합니다.
    /// </summary>
    public abstract class InteractableBase : MonoBehaviour, IInteractable
    {
        [Header("상호작용 설정")]
        [SerializeField] protected KeyCode interactionKey = KeyCode.F;
        [SerializeField] protected string interactionHintText = "F키를 눌러 상호작용";
        
        [Header("오디오")]
        [SerializeField] protected AudioSource audioSource;
        [SerializeField] protected AudioClip interactionSound;
        [SerializeField] protected float soundVolume = 0.5f;
        
        [Header("UI")]
        [SerializeField] protected UIManager uiManager;
        [SerializeField] protected bool showInteractionHint = true;
        
        [Header("디버그")]
        [SerializeField] protected bool showDebugLogs = false;
        
        protected Transform currentPlayer;
        protected bool playerInRange = false;
        
        // IInteractable 구현
        public virtual KeyCode InteractionKey => interactionKey;
        public abstract bool CanInteract { get; }
        
        protected virtual void Start()
        {
            InitializeAudioSource();
            InitializeUIManager();
        }
        
        protected virtual void Update()
        {
            // 플레이어가 범위 내에 있고 F키를 누르면
            if (playerInRange && CanInteract && Input.GetKeyDown(interactionKey))
            {
                Interact();
            }
        }
        
        /// <summary>
        /// AudioSource 초기화
        /// </summary>
        protected virtual void InitializeAudioSource()
        {
            if (audioSource == null)
            {
                audioSource = GetComponent<AudioSource>();
                if (audioSource == null && interactionSound != null)
                {
                    audioSource = gameObject.AddComponent<AudioSource>();
                    audioSource.playOnAwake = false;
                    audioSource.spatialBlend = 0.5f; // 3D 사운드
                }
            }
        }
        
        /// <summary>
        /// UI Manager 초기화
        /// </summary>
        protected virtual void InitializeUIManager()
        {
            if (uiManager == null)
            {
                uiManager = UIManager.Instance;
                if (uiManager == null && showDebugLogs)
                {
                    Debug.LogWarning($"[Interactable] {gameObject.name}: UIManager를 찾을 수 없습니다.");
                }
            }
        }
        
        /// <summary>
        /// 상호작용 실행 - 하위 클래스에서 구현
        /// </summary>
        public abstract void Interact();
        
        /// <summary>
        /// 플레이어가 범위에 진입
        /// </summary>
        public virtual void OnPlayerEnterRange(Transform player)
        {
            currentPlayer = player;
            playerInRange = true;
            
            if (showDebugLogs)
                Debug.Log($"[Interactable] {gameObject.name}: 플레이어 진입");
            
            // UI 힌트 표시
            if (showInteractionHint && uiManager != null)
            {
                uiManager.ShowInteractionHint(GetInteractionHintText());
            }
            
            OnPlayerEnter(player);
        }
        
        /// <summary>
        /// 플레이어가 범위를 벗어남
        /// </summary>
        public virtual void OnPlayerExitRange(Transform player)
        {
            playerInRange = false;
            
            if (showDebugLogs)
                Debug.Log($"[Interactable] {gameObject.name}: 플레이어 퇴장");
            
            // UI 힌트 숨김
            if (uiManager != null)
            {
                uiManager.HideInteractionHint();
            }
            
            OnPlayerExit(player);
            
            currentPlayer = null;
        }
        
        /// <summary>
        /// 상호작용 힌트 텍스트 반환
        /// </summary>
        public virtual string GetInteractionHintText()
        {
            return interactionHintText;
        }
        
        /// <summary>
        /// 오디오 재생
        /// </summary>
        protected virtual void PlaySound(AudioClip clip, float volume = -1f)
        {
            if (audioSource != null && clip != null)
            {
                float vol = volume < 0 ? soundVolume : volume;
                audioSource.PlayOneShot(clip, vol);
            }
        }
        
        /// <summary>
        /// 상호작용 사운드 재생
        /// </summary>
        protected virtual void PlayInteractionSound()
        {
            PlaySound(interactionSound);
        }
        
        // 하위 클래스에서 오버라이드 가능한 이벤트 메서드들
        
        /// <summary>
        /// 플레이어 진입 시 추가 로직 (하위 클래스에서 오버라이드)
        /// </summary>
        protected virtual void OnPlayerEnter(Transform player) { }
        
        /// <summary>
        /// 플레이어 퇴장 시 추가 로직 (하위 클래스에서 오버라이드)
        /// </summary>
        protected virtual void OnPlayerExit(Transform player) { }
    }
}

