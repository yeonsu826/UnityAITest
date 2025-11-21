using UnityEngine;
using System.Collections.Generic;

namespace UnityAI.Interaction
{
    /// <summary>
    /// 플레이어가 소유한 키를 관리하는 싱글톤 인벤토리
    /// </summary>
    public class KeyInventory : MonoBehaviour
    {
        private static KeyInventory instance;
        
        /// <summary>
        /// 싱글톤 인스턴스
        /// </summary>
        public static KeyInventory Instance
        {
            get
            {
                if (instance == null)
                {
                    // 씬에서 찾기
                    instance = FindFirstObjectByType<KeyInventory>();
                    
                    // 없으면 새로 생성
                    if (instance == null)
                    {
                        GameObject obj = new GameObject("KeyInventory");
                        instance = obj.AddComponent<KeyInventory>();
                    }
                }
                return instance;
            }
        }
        
        [Header("디버그")]
        [SerializeField] private bool showDebugLogs = true;
        
        // 소유한 키 목록 (키 ID로 관리)
        private HashSet<string> ownedKeys = new HashSet<string>();
        
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
            }
        }
        
        /// <summary>
        /// 키 추가
        /// </summary>
        public void AddKey(string keyId)
        {
            if (string.IsNullOrEmpty(keyId))
            {
                Debug.LogWarning("[KeyInventory] 키 ID가 비어있습니다!");
                return;
            }
            
            if (ownedKeys.Add(keyId))
            {
                if (showDebugLogs)
                    Debug.Log($"[KeyInventory] 키 획득: {keyId}");
                
                // 이벤트 발생
                OnKeyAdded(keyId);
            }
            else
            {
                if (showDebugLogs)
                    Debug.Log($"[KeyInventory] 이미 소유한 키: {keyId}");
            }
        }
        
        /// <summary>
        /// 키를 소유하고 있는지 확인
        /// </summary>
        public bool HasKey(string keyId)
        {
            return ownedKeys.Contains(keyId);
        }
        
        /// <summary>
        /// 키 제거 (사용 후 제거가 필요한 경우)
        /// </summary>
        public void RemoveKey(string keyId)
        {
            if (ownedKeys.Remove(keyId))
            {
                if (showDebugLogs)
                    Debug.Log($"[KeyInventory] 키 제거: {keyId}");
                
                OnKeyRemoved(keyId);
            }
        }
        
        /// <summary>
        /// 모든 키 제거
        /// </summary>
        public void ClearAllKeys()
        {
            ownedKeys.Clear();
            if (showDebugLogs)
                Debug.Log("[KeyInventory] 모든 키 제거");
        }
        
        /// <summary>
        /// 소유한 키 목록 반환
        /// </summary>
        public HashSet<string> GetAllKeys()
        {
            return new HashSet<string>(ownedKeys);
        }
        
        /// <summary>
        /// 키 추가 이벤트
        /// </summary>
        private void OnKeyAdded(string keyId)
        {
            // UI 업데이트
            UIManager uiManager = UIManager.Instance;
            if (uiManager != null)
            {
                uiManager.UpdateKeyStatus(keyId);
            }
        }
        
        /// <summary>
        /// 키 제거 이벤트
        /// </summary>
        private void OnKeyRemoved(string keyId)
        {
            // UI 업데이트
            UIManager uiManager = UIManager.Instance;
            if (uiManager != null)
            {
                uiManager.UpdateKeyStatus(null);
            }
        }
        
        /// <summary>
        /// 디버그용 - 인스펙터에서 확인
        /// </summary>
        private void OnGUI()
        {
            if (showDebugLogs && ownedKeys.Count > 0)
            {
                GUILayout.BeginArea(new Rect(10, 10, 300, 100));
                GUILayout.Label($"소유한 키: {ownedKeys.Count}개");
                foreach (string key in ownedKeys)
                {
                    GUILayout.Label($"  - {key}");
                }
                GUILayout.EndArea();
            }
        }
    }
}

