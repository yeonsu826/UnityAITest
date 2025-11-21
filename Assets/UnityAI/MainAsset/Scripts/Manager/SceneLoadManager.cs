using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 씬 로딩을 관리하는 매니저
/// Core 씬이 시작될 때 다른 씬들을 자동으로 로드합니다.
/// </summary>
public class SceneLoadManager : MonoBehaviour
{
    [Header("자동 로드 설정")]
    [Tooltip("Core 씬과 함께 자동으로 로드할 씬들")]
    [SerializeField] private string[] scenesToLoad = new string[] { "Lobby" };
    
    [Header("로드 옵션")]
    [SerializeField] private bool loadOnStart = true; // 시작 시 자동 로드
    [SerializeField] private bool useAdditiveMode = true; // Additive 모드 사용 (씬 겹치기)
    
    [Header("디버그")]
    [SerializeField] private bool showDebugLogs = true;
    
    private void Start()
    {
        if (loadOnStart)
        {
            LoadScenesAutomatically();
        }
    }
    
    /// <summary>
    /// 설정된 씬들을 자동으로 로드
    /// </summary>
    private void LoadScenesAutomatically()
    {
        if (scenesToLoad == null || scenesToLoad.Length == 0)
        {
            if (showDebugLogs)
                Debug.LogWarning("[SceneLoadManager] 로드할 씬이 설정되지 않았습니다!");
            return;
        }
        
        foreach (string sceneName in scenesToLoad)
        {
            if (string.IsNullOrEmpty(sceneName))
                continue;
            
            // 이미 로드된 씬인지 확인
            if (IsSceneLoaded(sceneName))
            {
                if (showDebugLogs)
                    Debug.Log($"[SceneLoadManager] '{sceneName}' 씬은 이미 로드되어 있습니다.");
                continue;
            }
            
            LoadScene(sceneName);
        }
    }
    
    /// <summary>
    /// 씬 로드
    /// </summary>
    public void LoadScene(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("[SceneLoadManager] 씬 이름이 비어있습니다!");
            return;
        }
        
        try
        {
            LoadSceneMode mode = useAdditiveMode ? LoadSceneMode.Additive : LoadSceneMode.Single;
            
            if (showDebugLogs)
                Debug.Log($"[SceneLoadManager] '{sceneName}' 씬 로딩 시작... (모드: {mode})");
            
            SceneManager.LoadScene(sceneName, mode);
            
            if (showDebugLogs)
                Debug.Log($"[SceneLoadManager] '{sceneName}' 씬 로딩 완료!");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[SceneLoadManager] '{sceneName}' 씬 로딩 실패: {e.Message}");
            Debug.LogError($"Build Settings에 '{sceneName}' 씬이 추가되어 있는지 확인하세요!");
        }
    }
    
    /// <summary>
    /// 씬이 이미 로드되어 있는지 확인
    /// </summary>
    public bool IsSceneLoaded(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.name == sceneName && scene.isLoaded)
            {
                return true;
            }
        }
        return false;
    }
    
    /// <summary>
    /// 씬 언로드
    /// </summary>
    public void UnloadScene(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("[SceneLoadManager] 씬 이름이 비어있습니다!");
            return;
        }
        
        if (!IsSceneLoaded(sceneName))
        {
            if (showDebugLogs)
                Debug.LogWarning($"[SceneLoadManager] '{sceneName}' 씬이 로드되어 있지 않습니다.");
            return;
        }
        
        try
        {
            if (showDebugLogs)
                Debug.Log($"[SceneLoadManager] '{sceneName}' 씬 언로드 시작...");
            
            SceneManager.UnloadSceneAsync(sceneName);
            
            if (showDebugLogs)
                Debug.Log($"[SceneLoadManager] '{sceneName}' 씬 언로드 완료!");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[SceneLoadManager] '{sceneName}' 씬 언로드 실패: {e.Message}");
        }
    }
    
    /// <summary>
    /// 비동기 씬 로드 (로딩 화면과 함께 사용 가능)
    /// </summary>
    public void LoadSceneAsync(string sceneName, System.Action<float> onProgress = null, System.Action onComplete = null)
    {
        StartCoroutine(LoadSceneAsyncCoroutine(sceneName, onProgress, onComplete));
    }
    
    /// <summary>
    /// 비동기 씬 로드 코루틴
    /// </summary>
    private System.Collections.IEnumerator LoadSceneAsyncCoroutine(string sceneName, System.Action<float> onProgress, System.Action onComplete)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("[SceneLoadManager] 씬 이름이 비어있습니다!");
            yield break;
        }
        
        LoadSceneMode mode = useAdditiveMode ? LoadSceneMode.Additive : LoadSceneMode.Single;
        
        if (showDebugLogs)
            Debug.Log($"[SceneLoadManager] '{sceneName}' 씬 비동기 로딩 시작...");
        
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, mode);
        
        while (!asyncLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            onProgress?.Invoke(progress);
            
            if (showDebugLogs)
                Debug.Log($"[SceneLoadManager] 로딩 진행률: {progress * 100:F0}%");
            
            yield return null;
        }
        
        if (showDebugLogs)
            Debug.Log($"[SceneLoadManager] '{sceneName}' 씬 비동기 로딩 완료!");
        
        onComplete?.Invoke();
    }
    
    /// <summary>
    /// 현재 로드된 모든 씬 출력 (디버그용)
    /// </summary>
    [ContextMenu("현재 로드된 씬 목록 출력")]
    public void PrintLoadedScenes()
    {
        Debug.Log($"[SceneLoadManager] 현재 로드된 씬 개수: {SceneManager.sceneCount}");
        
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            Debug.Log($"  [{i}] {scene.name} (로드됨: {scene.isLoaded})");
        }
    }
    
    /// <summary>
    /// Inspector에서 씬 목록에 빠르게 추가
    /// </summary>
    [ContextMenu("Lobby 씬 추가")]
    private void AddLobbyScene()
    {
        scenesToLoad = new string[] { "Lobby" };
        Debug.Log("[SceneLoadManager] Lobby 씬이 목록에 추가되었습니다.");
    }
}

