using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 미술관 NPC 인터렉션을 관리하는 스크립트
/// 플레이어가 F키를 눌러 설명글 UI를 표시하고, 거리가 멀어지면 자동으로 UI를 숨깁니다.
/// </summary>
public class NPCInteraction : MonoBehaviour
{
    [Header("NPC 데이터")]
    [SerializeField] private NPCData npcData;
    
    [Header("인터렉션 설정")]
    [SerializeField] private float interactionRange = 3f; // 인터렉션 가능 범위
    [SerializeField] private float autoCloseDistance = 5f; // UI 자동 닫힘 거리
    [SerializeField] private KeyCode interactionKey = KeyCode.F;
    
    [Header("오디오")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip interactionSound;
    [SerializeField] private float soundVolume = 0.5f;
    
    [Header("UI 참조")]
    [SerializeField] private NPCUIManager uiManager;
    
    [Header("디버그")]
    [SerializeField] private bool showDebugLogs = true;
    
    private Transform player;
    private bool playerInRange = false;
    private bool isUIActive = false;
    
    private void Start()
    {
        // AudioSource가 없으면 자동으로 추가
        if (audioSource == null)
        {
            audioSource = gameObject.GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
        
        // AudioSource 설정
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0.5f; // 3D 사운드
        
        // UI 매니저 찾기
        if (uiManager == null)
        {
            uiManager = FindObjectOfType<NPCUIManager>();
            if (uiManager == null)
            {
                Debug.LogError("[NPC] Error NPCUIManager를 찾을 수 없습니다. Scene에 NPCUIManager를 추가해주세요.");
            }
            else
            {
                if (showDebugLogs) Debug.Log("[NPC] Ok NPCUIManager 자동으로 찾음");
            }
        }
        
        // 플레이어 찾기
        FindPlayer();
        
        // 초기 설정 확인
        if (showDebugLogs)
        {
            Debug.Log($"[NPC] 초기화 완료 - NPC Data: {(npcData != null ? "Ok" : "Error")}, UI Manager: {(uiManager != null ? "Ok" : "Error")}, Player: {(player != null ? "Ok" : "Error")}");
        }
    }
    
    private void Update()
    {
        if (player == null)
        {
            FindPlayer();
            return;
        }
        
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        
        // 플레이어가 인터렉션 범위 내에 있는지 체크
        if (distanceToPlayer <= interactionRange)
        {
            if (!playerInRange)
            {
                OnPlayerEnterRange();
            }
            
            // F키 입력 체크 (기존 Input System 사용)
            if (Input.GetKeyDown(interactionKey) && !isUIActive)
            {
                OnInteract();
            }
        }
        else
        {
            if (playerInRange)
            {
                OnPlayerExitRange();
            }
        }
        
        // UI가 활성화된 상태에서 거리 체크
        if (isUIActive && distanceToPlayer > autoCloseDistance)
        {
            CloseUI();
        }
    }
    
    /// <summary>
    /// 플레이어 오브젝트 찾기
    /// </summary>
    private void FindPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            if (showDebugLogs) Debug.Log($"[NPC] Ok 플레이어 찾음 (Tag 사용): {player.name}");
        }
        else
        {
            // Tag가 없을 경우 PlayerMovement 스크립트를 가진 오브젝트 찾기
            PlayerMovement playerMovement = FindObjectOfType<PlayerMovement>();
            if (playerMovement != null)
            {
                player = playerMovement.transform;
                if (showDebugLogs) Debug.Log($"[NPC] Ok 플레이어 찾음 (PlayerMovement 사용): {player.name}");
            }
            else
            {
                if (showDebugLogs) Debug.LogWarning("[NPC] Error 플레이어를 찾을 수 없습니다. 플레이어에 'Player' Tag를 추가하거나 PlayerMovement 스크립트를 붙여주세요.");
            }
        }
    }
    
    /// <summary>
    /// 플레이어가 인터렉션 범위에 진입했을 때
    /// </summary>
    private void OnPlayerEnterRange()
    {
        playerInRange = true;
        if (showDebugLogs) Debug.Log("[NPC] 플레이어가 인터렉션 범위에 진입! F키를 누르세요.");
        // 여기에 힌트 UI 표시 로직 추가 가능 (예: "F키를 눌러 대화하기")
    }
    
    /// <summary>
    /// 플레이어가 인터렉션 범위를 벗어났을 때
    /// </summary>
    private void OnPlayerExitRange()
    {
        playerInRange = false;
        // 힌트 UI 숨김
        
        // 범위를 벗어나면 UI도 닫기
        if (isUIActive)
        {
            CloseUI();
        }
    }
    
    /// <summary>
    /// 인터렉션 실행 (F키 입력 시)
    /// </summary>
    private void OnInteract()
    {
        if (showDebugLogs) Debug.Log("[NPC] 🔑 F키 입력 감지!");
        
        if (npcData == null)
        {
            Debug.LogWarning("[NPC] Error NPC 데이터가 설정되지 않았습니다. Inspector에서 NPC Data를 할당해주세요.");
            return;
        }
        
        if (showDebugLogs) Debug.Log($"[NPC] 📋 NPC Data 확인: {npcData.name}");
        
        // 효과음 재생
        PlayInteractionSound();
        
        // UI 표시
        ShowUI();
    }
    
    /// <summary>
    /// 인터렉션 효과음 재생
    /// </summary>
    private void PlayInteractionSound()
    {
        if (audioSource != null && interactionSound != null)
        {
            audioSource.PlayOneShot(interactionSound, soundVolume);
        }
    }
    
    /// <summary>
    /// NPC 설명 UI 표시
    /// </summary>
    private void ShowUI()
    {
        if (uiManager == null)
        {
            Debug.LogError("[NPC] Error UI Manager가 없습니다!");
            return;
        }
        
        if (npcData == null)
        {
            Debug.LogError("[NPC] Error NPC Data가 없습니다!");
            return;
        }
        
        if (showDebugLogs) Debug.Log("[NPC] 🎨 UI 표시 시도...");
        uiManager.ShowNPCDescription(npcData);
        isUIActive = true;
        if (showDebugLogs) Debug.Log("[NPC] Ok UI 표시 완료!");
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
        }
    }
    
    /// <summary>
    /// 외부에서 UI를 닫을 때 사용 (UI의 닫기 버튼 등)
    /// </summary>
    public void OnUIClosedExternally()
    {
        isUIActive = false;
    }
    
    // Gizmos로 인터렉션 범위 표시
    private void OnDrawGizmosSelected()
    {
        // 인터렉션 범위 (초록색)
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
        
        // 자동 닫힘 거리 (빨간색)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, autoCloseDistance);
    }
}

