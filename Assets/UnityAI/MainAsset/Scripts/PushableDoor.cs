using UnityEngine;

/// <summary>
/// 플레이어가 실제로 밀어서 여는 물리 기반 문
/// CharacterController나 플레이어가 부딪히면 물리적으로 밀림
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(HingeJoint))]
public class PushableDoor : MonoBehaviour
{
    [Header("문 설정")]
    [SerializeField] private float openAngle = 90f; // 최대 열림 각도
    [SerializeField] private float springForce = 20f; // 문이 닫히려는 힘 (0이면 닫히지 않음)
    [SerializeField] private float damping = 5f; // 감쇠력 (문이 부드럽게 멈춤)
    
    [Header("사운드 (선택사항)")]
    [SerializeField] private AudioClip openSound; // 문 열리는 소리
    [SerializeField] private AudioClip closeSound; // 문 닫히는 소리
    [SerializeField] private float soundThreshold = 10f; // 소리가 나는 최소 각속도
    
    private HingeJoint hingeJoint;
    private Rigidbody rb;
    private AudioSource audioSource;
    private float lastAngle;
    private bool wasOpening = false;
    
    void Start()
    {
        // 컴포넌트 초기화
        hingeJoint = GetComponent<HingeJoint>();
        rb = GetComponent<Rigidbody>();
        
        // Rigidbody 설정
        rb.mass = 15f; // 문의 무게 (무거울수록 밀기 어려움)
        rb.angularDamping = damping; // 회전 감쇠
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
        
        lastAngle = hingeJoint.angle;
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
        limits.bounciness = 0.1f; // 약간의 튕김
        hingeJoint.limits = limits;
        
        // 스프링 설정 (문이 자동으로 닫히도록)
        if (springForce > 0)
        {
            hingeJoint.useSpring = true;
            JointSpring spring = hingeJoint.spring;
            spring.spring = springForce; // 스프링 힘
            spring.damper = damping * 2f; // 스프링 감쇠
            spring.targetPosition = 0f; // 닫힌 상태로 돌아감
            hingeJoint.spring = spring;
        }
        
        // 모터 비활성화 (수동으로 밀어야 함)
        hingeJoint.useMotor = false;
    }
    
    void Update()
    {
        // 문의 현재 각도
        float currentAngle = hingeJoint.angle;
        float angleDelta = Mathf.Abs(currentAngle - lastAngle);
        
        // 문이 움직이고 있는지 확인
        bool isOpening = angleDelta > soundThreshold * Time.deltaTime;
        
        // 사운드 재생
        if (isOpening && !wasOpening)
        {
            PlaySound(openSound);
        }
        else if (!isOpening && wasOpening && Mathf.Abs(currentAngle) < 5f)
        {
            PlaySound(closeSound);
        }
        
        lastAngle = currentAngle;
        wasOpening = isOpening;
    }
    
    /// <summary>
    /// 플레이어나 다른 물체와 충돌할 때 호출
    /// </summary>
    void OnCollisionEnter(Collision collision)
    {
        // CharacterController는 OnControllerColliderHit으로 처리되므로 여기선 처리 안 함
        if (collision.gameObject.CompareTag("Player"))
        {
            // 충돌 방향으로 약간의 힘 추가 (선택사항)
            Vector3 pushDirection = collision.contacts[0].normal;
            rb.AddForceAtPosition(-pushDirection * 50f, collision.contacts[0].point, ForceMode.Impulse);
        }
    }
    
    /// <summary>
    /// 사운드 재생
    /// </summary>
    void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null && !audioSource.isPlaying)
        {
            audioSource.PlayOneShot(clip);
        }
    }
    
    /// <summary>
    /// Scene 뷰에서 경첩 위치 시각화
    /// </summary>
    void OnDrawGizmosSelected()
    {
        // 문의 경첩 위치 표시
        Gizmos.color = Color.red;
        Vector3 hingePosition = transform.position + transform.TransformDirection(new Vector3(-0.5f, 0f, 0f));
        Gizmos.DrawSphere(hingePosition, 0.15f);
        
        // 열림 범위 표시
        Gizmos.color = Color.yellow;
        Vector3 openDirection = Quaternion.AngleAxis(openAngle, Vector3.up) * transform.forward;
        Gizmos.DrawRay(hingePosition, openDirection * 2f);
        
        Vector3 closeDirection = Quaternion.AngleAxis(-openAngle, Vector3.up) * transform.forward;
        Gizmos.DrawRay(hingePosition, closeDirection * 2f);
    }
}

