using UnityEngine;

/// <summary>
/// NPC의 정보를 담는 ScriptableObject
/// 미술관 전시물에 대한 설명 등을 저장합니다.
/// </summary>
[CreateAssetMenu(fileName = "New NPC Data", menuName = "UnityAI/NPC Data", order = 1)]
public class NPCData : ScriptableObject
{
    [Header("NPC 기본 정보")]
    [Tooltip("NPC의 이름")]
    public string npcName = "미술관 가이드";
    
    [Header("전시물 설명")]
    [Tooltip("전시물 또는 장소의 제목")]
    [TextArea(15, 20)]
    public string exhibitTitle = "작품 제목";
    
    [Tooltip("전시물에 대한 설명 내용")]
    [TextArea(5, 15)]
    public string description = "여기에 전시물에 대한 상세 설명을 입력하세요.";
    
    [Header("추가 정보")]
    [Tooltip("작가명 (선택사항)")]
    public string artistName = "";


    [Header("제작 연도")]
    [Tooltip("제작 연도 (선택사항)")]
    [TextArea(1, 2)]
    public string yearCreated = "";
    
    [Header("작품 이미지")]
    [Tooltip("작품 이미지 (선택사항)")]
    [TextArea(1, 2)]
    public Sprite exhibitImage;
    
    [Header("UI 설정")]
    [Tooltip("설명 글의 폰트 크기")]
    [Range(12, 36)]
    public int fontSize = 16;
    
    
}

