using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityAI.Interaction;

/// <summary>
/// 게임 내 모든 UI를 통합 관리하는 매니저
/// PlayerStatusUI + NPCUIManager 통합 버전
/// </summary>
public class UIManager : MonoBehaviour
{
    private static UIManager instance;
    
    /// <summary>
    /// 싱글톤 인스턴스
    /// </summary>
    public static UIManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<UIManager>();
                
                if (instance == null)
                {
                    Debug.LogWarning("[UIManager] 씬에 UIManager가 없습니다!");
                }
            }
            return instance;
        }
    }
    
    #region Player Status UI
    
    [Header("=== 플레이어 상태 UI ===")]
    [SerializeField] private TextMeshProUGUI statusText; // 상태 텍스트
    [SerializeField] private TextMeshProUGUI acquisitionText; // "획득!" 메시지 텍스트
    
    [Header("상태 텍스트 설정")]
    private string defaultStatusText = "상태: 키 없음";
    private Color normalStatusColor = Color.white;
    
    [Header("획득 메시지 설정")]
    [SerializeField] private string acquisitionMessage = "획득!";
    [SerializeField] private Color acquisitionColor = Color.yellow;
    [SerializeField] private float acquisitionDisplayTime = 2f;
    [SerializeField] private bool animateAcquisition = true;
    
    private float acquisitionTimer = 0f;
    private float originalAcquisitionScale = 1f;
    private bool hasKey = false;
    
    #endregion
    
    #region NPC Description UI
    
    [Header("=== NPC 설명 UI ===")]
    [SerializeField] private GameObject descriptionPanel;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI artistText;
    [SerializeField] private TextMeshProUGUI yearText;
    [SerializeField] private Image exhibitImage;
    [SerializeField] private Button closeButton;
    
    [Header("애니메이션 설정")]
    [SerializeField] private bool useFadeAnimation = true;
    [SerializeField] private float fadeDuration = 0.3f;
    
    private CanvasGroup canvasGroup;
    private NPCInteractable currentNPC;
    
    #endregion
    
    #region Interaction Hint UI
    
    [Header("=== 인터렉션 힌트 UI ===")]
    [SerializeField] private GameObject interactionHintPanel;
    [SerializeField] private TextMeshProUGUI hintText;
    
    #endregion
    
    [Header("=== 디버그 ===")]
    [SerializeField] private bool showDebugLogs = false;
    
    #region Unity Lifecycle
    
    private void Awake()
    {
        // 싱글톤 설정
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        InitializeUI();
    }
    
    private void Start()
    {
        UpdateStatusText();
    }
    
    private void Update()
    {
        // 획득 메시지 타이머
        UpdateAcquisitionTimer();
        
        // ESC 키로 NPC 설명 UI 닫기
        if (descriptionPanel != null && descriptionPanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            HideNPCDescription();
        }
    }
    
    #endregion
    
    #region Initialization
    
    /// <summary>
    /// UI 초기화
    /// </summary>
    private void InitializeUI()
    {
        // Player Status UI 초기화
        InitializePlayerStatusUI();
        
        // NPC Description UI 초기화
        InitializeNPCDescriptionUI();
        
        // Interaction Hint UI 초기화
        InitializeInteractionHintUI();
    }
    
    /// <summary>
    /// 플레이어 상태 UI 초기화
    /// </summary>
    private void InitializePlayerStatusUI()
    {
        // 획득 메시지 텍스트 초기화
        if (acquisitionText != null)
        {
            originalAcquisitionScale = acquisitionText.transform.localScale.x;
            acquisitionText.gameObject.SetActive(false);
        }
        
        if (showDebugLogs)
            Debug.Log("[UIManager] 플레이어 상태 UI 초기화 완료");
    }
    
    /// <summary>
    /// NPC 설명 UI 초기화
    /// </summary>
    private void InitializeNPCDescriptionUI()
    {
        // CanvasGroup 설정 (애니메이션용)
        if (descriptionPanel != null)
        {
            canvasGroup = descriptionPanel.GetComponent<CanvasGroup>();
            if (canvasGroup == null && useFadeAnimation)
            {
                canvasGroup = descriptionPanel.AddComponent<CanvasGroup>();
            }
            
            descriptionPanel.SetActive(false);
        }
        else
        {
            Debug.LogWarning("[UIManager] NPC Description Panel이 할당되지 않았습니다!");
        }
        
        // 닫기 버튼 이벤트 연결
        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(HideNPCDescription);
        }
        
        if (showDebugLogs)
            Debug.Log("[UIManager] NPC 설명 UI 초기화 완료");
    }
    
    /// <summary>
    /// 인터렉션 힌트 UI 초기화
    /// </summary>
    private void InitializeInteractionHintUI()
    {
        if (interactionHintPanel != null)
        {
            interactionHintPanel.SetActive(false);
        }
        
        if (showDebugLogs)
            Debug.Log("[UIManager] 인터렉션 힌트 UI 초기화 완료");
    }
    
    #endregion
    
    #region Player Status UI Methods
    
    /// <summary>
    /// 키 상태 업데이트 (KeyInventory에서 호출)
    /// </summary>
    public void UpdateKeyStatus(string keyId)
    {
        bool hadKey = hasKey;
        hasKey = !string.IsNullOrEmpty(keyId);
        
        // 키를 새로 획득했을 때 "획득!" 메시지 표시
        if (!hadKey && hasKey)
        {
            ShowAcquisitionMessage();
            
            if (showDebugLogs)
                Debug.Log($"[UIManager] 키 획득: {keyId}");
        }
    }
    
    /// <summary>
    /// "획득!" 메시지 표시
    /// </summary>
    private void ShowAcquisitionMessage()
    {
        if (acquisitionText == null)
        {
            if (showDebugLogs)
                Debug.LogWarning("[UIManager] 획득 메시지 텍스트가 설정되지 않았습니다!");
            return;
        }
        
        // 메시지 설정
        acquisitionText.text = acquisitionMessage;
        acquisitionText.color = acquisitionColor;
        acquisitionText.gameObject.SetActive(true);
        
        // 타이머 시작
        acquisitionTimer = acquisitionDisplayTime;
        
        // 초기 스케일 복원
        acquisitionText.transform.localScale = Vector3.one * originalAcquisitionScale;
        
        if (showDebugLogs)
            Debug.Log($"[UIManager] 획득 메시지 표시: {acquisitionMessage}");
    }
    
    /// <summary>
    /// "획득!" 메시지 숨김
    /// </summary>
    private void HideAcquisitionMessage()
    {
        if (acquisitionText != null)
        {
            acquisitionText.gameObject.SetActive(false);
            acquisitionText.transform.localScale = Vector3.one * originalAcquisitionScale;
        }
    }
    
    /// <summary>
    /// 획득 메시지 타이머 업데이트
    /// </summary>
    private void UpdateAcquisitionTimer()
    {
        if (acquisitionTimer > 0f)
        {
            acquisitionTimer -= Time.deltaTime;
            
            // 애니메이션 효과
            if (animateAcquisition && acquisitionText != null)
            {
                // 크기 펄스 효과
                float scale = 1f + Mathf.Sin(acquisitionTimer * 10f) * 0.15f;
                acquisitionText.transform.localScale = Vector3.one * scale * originalAcquisitionScale;
                
                // 페이드 아웃 효과
                float alpha = Mathf.Clamp01(acquisitionTimer / acquisitionDisplayTime);
                Color color = acquisitionText.color;
                color.a = alpha;
                acquisitionText.color = color;
            }
            
            // 타이머 종료 시 숨김
            if (acquisitionTimer <= 0f)
            {
                HideAcquisitionMessage();
            }
        }
    }
    
    /// <summary>
    /// 상태 텍스트 업데이트
    /// </summary>
    private void UpdateStatusText()
    {
        if (statusText == null)
            return;
        
        statusText.text = defaultStatusText;
        statusText.color = normalStatusColor;
    }
    
    /// <summary>
    /// 커스텀 상태 텍스트 표시
    /// </summary>
    public void ShowCustomStatus(string text, Color color)
    {
        if (statusText == null)
            return;
        
        statusText.text = text;
        statusText.color = color;
    }
    
    /// <summary>
    /// 상태 텍스트를 기본값으로 복원
    /// </summary>
    public void ResetStatusToDefault()
    {
        hasKey = false;
        UpdateStatusText();
        HideAcquisitionMessage();
    }
    
    /// <summary>
    /// 현재 키 보유 상태 반환
    /// </summary>
    public bool HasKey()
    {
        return hasKey;
    }
    
    #endregion
    
    #region NPC Description UI Methods
    
    /// <summary>
    /// NPC 설명 UI 표시
    /// </summary>
    public void ShowNPCDescription(NPCData npcData)
    {
        if (npcData == null)
        {
            Debug.LogError("[UIManager] NPC Data가 null입니다!");
            return;
        }
        
        if (descriptionPanel == null)
        {
            Debug.LogError("[UIManager] Description Panel이 null입니다!");
            return;
        }
        
        // UI 내용 업데이트
        UpdateNPCContent(npcData);
        
        // UI 표시
        descriptionPanel.SetActive(true);
        
        // 페이드 인 애니메이션
        if (useFadeAnimation && canvasGroup != null)
        {
            StopAllCoroutines();
            StartCoroutine(FadeIn());
        }
        
        if (showDebugLogs)
            Debug.Log($"[UIManager] NPC 설명 표시: {npcData.exhibitTitle}");
    }
    
    /// <summary>
    /// NPC 설명 UI 표시 (NPCInteractable 참조와 함께)
    /// </summary>
    public void ShowNPCDescription(NPCData npcData, NPCInteractable npcInteractable)
    {
        currentNPC = npcInteractable;
        ShowNPCDescription(npcData);
    }
    
    /// <summary>
    /// NPC 설명 UI 숨김
    /// </summary>
    public void HideNPCDescription()
    {
        if (descriptionPanel == null)
            return;
        
        // 페이드 아웃 애니메이션
        if (useFadeAnimation && canvasGroup != null)
        {
            StopAllCoroutines();
            StartCoroutine(FadeOut());
        }
        else
        {
            descriptionPanel.SetActive(false);
        }
        
        // NPC에게 UI가 닫혔음을 알림
        if (currentNPC != null)
        {
            currentNPC.OnUIClosedExternally();
            currentNPC = null;
        }
        
        if (showDebugLogs)
            Debug.Log("[UIManager] NPC 설명 숨김");
    }
    
    /// <summary>
    /// NPC UI 내용 업데이트
    /// </summary>
    private void UpdateNPCContent(NPCData npcData)
    {
        // 제목
        if (titleText != null)
        {
            titleText.text = npcData.exhibitTitle;
            titleText.fontSize = npcData.fontSize + 4;
        }
        
        // 설명
        if (descriptionText != null)
        {
            descriptionText.text = npcData.description;
            descriptionText.fontSize = npcData.fontSize;
        }
        
        // 작가명
        if (artistText != null)
        {
            if (!string.IsNullOrEmpty(npcData.artistName))
            {
                artistText.text = "작가: " + npcData.artistName;
                artistText.gameObject.SetActive(true);
            }
            else
            {
                artistText.gameObject.SetActive(false);
            }
        }
        
        // 제작 연도
        if (yearText != null)
        {
            if (!string.IsNullOrEmpty(npcData.yearCreated))
            {
                yearText.text = "제작 연도: " + npcData.yearCreated;
                yearText.gameObject.SetActive(true);
            }
            else
            {
                yearText.gameObject.SetActive(false);
            }
        }
        
        // 이미지
        if (exhibitImage != null)
        {
            if (npcData.exhibitImage != null)
            {
                exhibitImage.sprite = npcData.exhibitImage;
                exhibitImage.gameObject.SetActive(true);
            }
            else
            {
                exhibitImage.gameObject.SetActive(false);
            }
        }
    }
    
    /// <summary>
    /// 페이드 인 애니메이션
    /// </summary>
    private System.Collections.IEnumerator FadeIn()
    {
        canvasGroup.alpha = 0f;
        float elapsed = 0f;
        
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            yield return null;
        }
        
        canvasGroup.alpha = 1f;
    }
    
    /// <summary>
    /// 페이드 아웃 애니메이션
    /// </summary>
    private System.Collections.IEnumerator FadeOut()
    {
        canvasGroup.alpha = 1f;
        float elapsed = 0f;
        
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            yield return null;
        }
        
        canvasGroup.alpha = 0f;
        descriptionPanel.SetActive(false);
    }
    
    #endregion
    
    #region Interaction Hint UI Methods
    
    /// <summary>
    /// 인터렉션 힌트 표시
    /// </summary>
    public void ShowInteractionHint(string message = "F키를 눌러 감상하기")
    {
        if (interactionHintPanel != null)
        {
            if (hintText != null)
            {
                hintText.text = message;
            }
            interactionHintPanel.SetActive(true);
            
            if (showDebugLogs)
                Debug.Log($"[UIManager] 힌트 표시: {message}");
        }
    }
    
    /// <summary>
    /// 인터렉션 힌트 숨김
    /// </summary>
    public void HideInteractionHint()
    {
        if (interactionHintPanel != null)
        {
            interactionHintPanel.SetActive(false);
            
            if (showDebugLogs)
                Debug.Log("[UIManager] 힌트 숨김");
        }
    }
    
    #endregion
    
    #region Public Test Methods
    
    /// <summary>
    /// 획득 메시지 강제 표시 (테스트용)
    /// </summary>
    public void TestShowAcquisition()
    {
        ShowAcquisitionMessage();
    }
    
    #endregion
}

