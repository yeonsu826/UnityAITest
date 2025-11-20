using UnityEngine;

/// <summary>
/// 물리 엔진 기반 자동문 - HingeJoint와 모터를 사용하여 실제 문처럼 작동
/// 플레이어가 가까이 오면 자동으로 열리고, 떠나면 닫힘
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(HingeJoint))]
[RequireComponent(typeof(BoxCollider))]
public class PhysicsDoor : MonoBehaviour
{
    [Header("문 설정")]
    [SerializeField] private float openAngle = 90f; // 열릴 때 각도
    [SerializeField] private float motorForce = 100f; // 문을 여는 힘
    [SerializeField] private float motorSpeed = 50f; // 문이 열리는 속도
    
    [Header("트리거 설정")]
    [SerializeField] private float triggerDistance = 3f; // 문이 열리기 시작하는 거리
    [SerializeField] private LayerMask playerLayer; // 플레이어 레이어
    
    [Header("사운드 (선택사항)")]
    [SerializeField] private AudioClip openSound; // 문 열리는 소리
    [SerializeField] private AudioClip closeSound; // 문 닫히는 소리
    
    private HingeJoint hingeJoint;
    private Rigidbody rb;
    private AudioSource audioSource;
    private bool isOpen = false;
    private bool wasOpen = false;
    private Transform playerTransform;
    
    void Start()
    {
        // 컴포넌트 초기화
        hingeJoint = GetComponent<HingeJoint>();
        rb = GetComponent<Rigidbody>();
        
        // Rigidbody 설정
        rb.mass = 10f; // 문의 무게
        rb.angularDamping = 5f; // 회전 감쇠 (자연스러운 멈춤)
        rb.constraints = RigidbodyConstraints.FreezePosition; // 위치 고정, 회전만 가능
        
        // HingeJoint 설정
        SetupHingeJoint();
        
        // AudioSource 설정
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && (openSound != null || closeSound != null))
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
        
        // 플레이어 찾기
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogWarning("PhysicsDoor: Player 태그를 가진 오브젝트를 찾을 수 없습니다!");
        }
    }
    
    void SetupHingeJoint()
    {
        // HingeJoint 기본 설정
        hingeJoint.axis = Vector3.up; // Y축을 중심으로 회전
        hingeJoint.anchor = new Vector3(-0.5f, 0f, 0f); // 경첩 위치 (문의 한쪽 끝)
        
        // 각도 제한 설정
        hingeJoint.useLimits = true;
        JointLimits limits = hingeJoint.limits;
        limits.min = -openAngle; // 음수 방향 열림 각도
        limits.max = openAngle; // 양수 방향 열림 각도
        limits.bounciness = 0f; // 튕김 없음
        hingeJoint.limits = limits;
        
        // 모터 활성화 (자동으로 문 여닫기)
        hingeJoint.useMotor = true;
    }
    
    void Update()
    {
        if (playerTransform == null) return;
        
        // 플레이어와의 거리 계산
        float distance = Vector3.Distance(transform.position, playerTransform.position);
        
        // 플레이어 방향 계산
        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
        float dotProduct = Vector3.Dot(transform.forward, directionToPlayer);
        
        // 거리에 따라 문 열림/닫힘 결정
        if (distance < triggerDistance)
        {
            // 플레이어가 문의 어느 쪽에 있는지에 따라 열림 방향 결정
            float targetVelocity = dotProduct > 0 ? motorSpeed : -motorSpeed;
            SetMotor(targetVelocity, motorForce);
            
            if (!isOpen)
            {
                isOpen = true;
            }
        }
        else
        {
            // 문 닫기 (천천히 원래 위치로)
            SetMotor(0f, motorForce * 0.5f); // 닫을 때는 약한 힘
            
            if (isOpen)
            {
                isOpen = false;
            }
        }
        
        // 사운드 재생
        if (isOpen && !wasOpen)
        {
            PlaySound(openSound);
        }
        else if (!isOpen && wasOpen)
        {
            PlaySound(closeSound);
        }
        
        wasOpen = isOpen;
    }
    
    /// <summary>
    /// HingeJoint 모터 설정
    /// </summary>
    void SetMotor(float targetVelocity, float force)
    {
        JointMotor motor = hingeJoint.motor;
        motor.targetVelocity = targetVelocity;
        motor.force = force;
        hingeJoint.motor = motor;
    }
    
    /// <summary>
    /// 사운드 재생
    /// </summary>
    void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
    
    /// <summary>
    /// Scene 뷰에서 트리거 범위 시각화
    /// </summary>
    void OnDrawGizmosSelected()
    {
        Gizmos.color = isOpen ? Color.green : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, triggerDistance);
        
        // 문의 경첩 위치 표시
        Gizmos.color = Color.red;
        Vector3 hingePosition = transform.position + transform.TransformDirection(new Vector3(-0.5f, 0f, 0f));
        Gizmos.DrawSphere(hingePosition, 0.1f);
    }
}

