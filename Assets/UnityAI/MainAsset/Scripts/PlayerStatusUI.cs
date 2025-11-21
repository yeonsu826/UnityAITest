using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityAI.Interaction;

/// <summary>
/// 플레이어 상태를 화면에 표시하는 UI 관리자
/// 키 획득 상태 등을 표시합니다.
/// </summary>
public class PlayerStatusUI : MonoBehaviour
{
    [Header("UI 텍스트")]
    [SerializeField] private TextMeshProUGUI statusText; // 상태 텍스트
    [SerializeField] private TextMeshProUGUI acquisitionText; // "획득!" 메시지 텍스트 (선택)
    
    [Header("텍스트 설정")]
    [SerializeField] private string defaultText = "상태: 정상"; // 기본 텍스트
    [SerializeField] private Color normalColor = Color.white; // 기본 색상
    
    [Header("획득 메시지 설정")]
    [SerializeField] private string acquisitionMessage = "획득!"; // 획득 시 표시할 메시지
    [SerializeField] private Color acquisitionColor = Color.yellow; // 획득 메시지 색상
    [SerializeField] private float acquisitionDisplayTime = 2f; // 획득 메시지 표시 시간
    
    [Header("애니메이션 효과")]
    [SerializeField] private bool animateAcquisition = true; // 획득 시 애니메이션
    [SerializeField] private float animationDuration = 2f; // 애니메이션 지속 시간
    
    [Header("디버그")]
    [SerializeField] private bool showDebugLogs = false;
    
    private bool hasKey = false;
    private float animationTimer = 0f;
    private float acquisitionTimer = 0f; // 획득 메시지 타이머
    private float originalScale = 1f;
    private float originalAcquisitionScale = 1f;
    
    private void Start()
    {
        InitializeUI();
        UpdateStatusText();
    }
    
    private void Update()
    {
        // 획득 메시지 타이머
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
    /// UI 초기화
    /// </summary>
    private void InitializeUI()
    {
        if (statusText == null)
        {
            // 자동으로 찾기 시도
            statusText = GetComponent<TextMeshProUGUI>();
            
            if (statusText == null)
            {
                // 자식에서 찾기
                statusText = GetComponentInChildren<TextMeshProUGUI>();
            }
            
            if (statusText == null)
            {
                Debug.LogError("[PlayerStatusUI] TextMeshProUGUI 컴포넌트를 찾을 수 없습니다!");
                return;
            }
        }
        
        originalScale = statusText.transform.localScale.x;
        
        // 획득 메시지 텍스트 초기화
        if (acquisitionText != null)
        {
            originalAcquisitionScale = acquisitionText.transform.localScale.x;
            acquisitionText.gameObject.SetActive(false); // 시작 시 숨김
        }
    }
    
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
                Debug.Log($"[PlayerStatusUI] 키 획득: {keyId}");
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
                Debug.LogWarning("[PlayerStatusUI] 획득 메시지 텍스트가 설정되지 않았습니다!");
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
            Debug.Log($"[PlayerStatusUI] 획득 메시지 표시: {acquisitionMessage}");
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
    /// 상태 텍스트 업데이트
    /// </summary>
    private void UpdateStatusText()
    {
        if (statusText == null)
            return;
        
        statusText.text = defaultText;
        statusText.color = normalColor;
    }
    
    /// <summary>
    /// 커스텀 텍스트 표시
    /// </summary>
    public void ShowCustomText(string text, Color color)
    {
        if (statusText == null)
            return;
        
        statusText.text = text;
        statusText.color = color;
    }
    
    /// <summary>
    /// 기본 텍스트로 복원
    /// </summary>
    public void ResetToDefault()
    {
        hasKey = false;
        
        if (statusText != null)
        {
            statusText.text = defaultText;
            statusText.color = normalColor;
        }
        
        HideAcquisitionMessage();
    }
    
    /// <summary>
    /// 현재 키 보유 상태 반환
    /// </summary>
    public bool HasKey()
    {
        return hasKey;
    }
    
    /// <summary>
    /// 획득 메시지 강제 표시 (테스트용)
    /// </summary>
    public void TestShowAcquisition()
    {
        ShowAcquisitionMessage();
    }
}

