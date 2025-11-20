using UnityEngine;

namespace UnityAI.Interaction
{
    /// <summary>
    /// 상호작용 가능한 오브젝트의 기본 인터페이스
    /// F키 등의 입력으로 상호작용할 수 있는 모든 오브젝트가 구현해야 합니다.
    /// </summary>
    public interface IInteractable
    {
        /// <summary>
        /// 상호작용 키 (기본: F)
        /// </summary>
        KeyCode InteractionKey { get; }
        
        /// <summary>
        /// 상호작용이 현재 가능한지 여부
        /// </summary>
        bool CanInteract { get; }
        
        /// <summary>
        /// 상호작용 실행 (F키를 눌렀을 때)
        /// </summary>
        void Interact();
        
        /// <summary>
        /// 플레이어가 상호작용 범위에 진입했을 때
        /// </summary>
        /// <param name="player">플레이어 Transform</param>
        void OnPlayerEnterRange(Transform player);
        
        /// <summary>
        /// 플레이어가 상호작용 범위를 벗어났을 때
        /// </summary>
        /// <param name="player">플레이어 Transform</param>
        void OnPlayerExitRange(Transform player);
        
        /// <summary>
        /// 상호작용 힌트 텍스트 (예: "F키를 눌러 대화하기")
        /// </summary>
        string GetInteractionHintText();
    }
}

