using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// NPC 설명 UI를 관리하는 매니저 스크립트
/// Canvas에 부착하여 사용합니다.
/// </summary>
public class NPCUIManager : MonoBehaviour
{
    [Header("UI 패널 참조")]
    [SerializeField] private GameObject descriptionPanel;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI artistText;
    [SerializeField] private TextMeshProUGUI yearText;
    [SerializeField] private Image exhibitImage;
    [SerializeField] private Button closeButton;
    
    [Header("힌트 UI")]
    [SerializeField] private GameObject interactionHintPanel;
    [SerializeField] private TextMeshProUGUI hintText;
    
    [Header("애니메이션 설정")]
    [SerializeField] private bool useFadeAnimation = true;
    [SerializeField] private float fadeDuration = 0.3f;
    
    [Header("디버그")]
    [SerializeField] private bool showDebugLogs = true;
    
    private CanvasGroup canvasGroup;
    private NPCInteraction currentNPC;
    
    private void Awake()
    {
        // CanvasGroup 설정 (애니메이션용)
        if (descriptionPanel != null)
        {
            canvasGroup = descriptionPanel.GetComponent<CanvasGroup>();
            if (canvasGroup == null && useFadeAnimation)
            {
                canvasGroup = descriptionPanel.AddComponent<CanvasGroup>();
            }
        }
        else
        {
            Debug.LogError("[UI] Description Panel이 할당되지 않았습니다!");
        }
        
        // 닫기 버튼 이벤트 연결
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(OnCloseButtonClicked);
        }
        
        // 시작 시 UI 숨김
        if (descriptionPanel != null)
        {
            descriptionPanel.SetActive(false);
        }
        
        // 힌트 UI도 숨김
        if (interactionHintPanel != null)
        {
            interactionHintPanel.SetActive(false);
        }
        
        if (showDebugLogs)
        {
            Debug.Log($"[UI] NPCUIManager 초기화 - Panel: {(descriptionPanel != null ?"OK" : "ERROR")}, " +
                      $"Title: {(titleText != null ? "OK" : "ERROR")}, " +
                      $"Description: {(descriptionText != null ? "OK" : "ERROR")}");
        }
    }
    
    /// <summary>
    /// NPC 설명 UI 표시
    /// </summary>
    /// <param name="npcData">표시할 NPC 데이터</param>
    public void ShowNPCDescription(NPCData npcData)
    {
        if (showDebugLogs) Debug.Log("[UI] ShowNPCDescription 호출됨");
        
        if (npcData == null)
        {
            Debug.LogError("[UI] NPC Data가 null입니다!");
            return;
        }
        
        if (descriptionPanel == null)
        {
            Debug.LogError("[UI] Description Panel이 null입니다!");
            return;
        }
        
        if (showDebugLogs) Debug.Log($"[UI] NPC Data: {npcData.name} - {npcData.exhibitTitle}");
        
        // UI 내용 업데이트
        UpdateUIContent(npcData);
        
        // UI 표시
        descriptionPanel.SetActive(true);
        if (showDebugLogs) Debug.Log("[UI] Description Panel 활성화!");
        
        // 페이드 인 애니메이션
        if (useFadeAnimation && canvasGroup != null)
        {
            StopAllCoroutines();
            StartCoroutine(FadeIn());
        }
    }
    
    /// <summary>
    /// NPC 설명 UI 숨김
    /// </summary>
    public void HideNPCDescription()
    {
        if (descriptionPanel == null)
        {
            return;
        }
        
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
    }
    
    /// <summary>
    /// UI 내용 업데이트
    /// </summary>
    private void UpdateUIContent(NPCData npcData)
    {
        // 제목
        if (titleText != null)
        {
            titleText.text = npcData.exhibitTitle;
            titleText.fontSize = npcData.fontSize + 4; // 제목은 조금 더 크게
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
    /// 닫기 버튼 클릭 시
    /// </summary>
    private void OnCloseButtonClicked()
    {
        HideNPCDescription();
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
    
    /// <summary>
    /// ESC 키로 UI 닫기 (옵션)
    /// </summary>
    private void Update()
    {
        if (descriptionPanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            HideNPCDescription();
        }
    }

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
        }
    }
}

