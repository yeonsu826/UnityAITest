using UnityEngine;

/// <summary>
/// WASD 키로 이동, Space로 점프, Shift + Space로 높은 점프, 물체에 부딪치면 밀어냄
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("이동 설정")]
    [SerializeField] private float moveSpeed = 5f; // 플레이어 이동 속도
    [SerializeField] private float gravity = -9.81f; // 중력 값
    [SerializeField] private float groundCheckDistance = 0.2f; // 바닥 체크 거리
    [SerializeField] private float rotationSpeed = 10f; // 플레이어 회전 속도

    [Header("점프 설정")]
    [SerializeField] private float jumpHeight = 2f; // 기본 점프 높이
    [SerializeField] private float highJumpHeight = 3.5f; // 높은 점프 높이 (Shift + Space)

    [Header("카메라 설정")]
    [SerializeField] private Transform cameraTransform; // 카메라 Transform (방향 참조용)

    [Header("물리 충돌 설정")]
    [SerializeField] private float pushPower = 2f; // 물체를 밀어내는 힘

    /// <summary>
    /// CharacterController 컴포넌트 참조
    /// </summary>
    private CharacterController controller;

    /// <summary>
    /// 현재 수직 속도
    /// </summary>
    private Vector3 velocity;

    /// <summary>
    /// 플레이어가 땅에 닿아있는지 여부
    /// </summary>
    private bool isGrounded;

    /// <summary>
    /// 초기화
    /// </summary>
    void Start()
    {
        controller = GetComponent<CharacterController>();

        // 카메라가 설정되지 않았으면 메인 카메라 자동 할당
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
            if (cameraTransform == null)
            {
                Debug.LogWarning("PlayerMovement: 메인 카메라를 찾을 수 없습니다!");
            }
        }
    }

    /// <summary>
    /// 매 프레임 플레이어 이동 처리
    /// </summary>
    void Update()
    {
        HandleGroundCheck();
        HandleMovement();
        HandleGravity();
        HandleJump();
    }

    /// <summary>
    /// 바닥 체크 처리
    /// </summary>
    private void HandleGroundCheck()
    {
        // CharacterController의 바닥면에서 약간 아래로 레이캐스트
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 
            controller.bounds.extents.y + groundCheckDistance);

        // 땅에 닿았을 때 수직 속도 리셋
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // 작은 음수 값으로 땅에 붙어있게 함
        }
    }

    /// <summary>
    /// WASD 키 입력을 받아 플레이어 이동 처리 (카메라 방향 기준)
    /// </summary>
    private void HandleMovement()
    {
        if (cameraTransform == null) return;

        // WASD 입력 받기
        float horizontal = Input.GetAxis("Horizontal"); // A, D 키
        float vertical = Input.GetAxis("Vertical"); // W, S 키

        // 카메라의 forward와 right 벡터 계산 (Y축 제거하여 수평면만 사용)
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;
        
        // Y축 성분 제거 및 정규화 (수평면에서만 이동)
        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();

        // 이동 방향 계산 (카메라 기준)
        Vector3 move = cameraRight * horizontal + cameraForward * vertical;

        // CharacterController로 이동
        if (move.magnitude > 0.1f)
        {
            controller.Move(move * moveSpeed * Time.deltaTime);

            // 플레이어를 이동 방향으로 부드럽게 회전
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    /// <summary>
    /// 중력 적용
    /// </summary>
    private void HandleGravity()
    {
        // 중력 가속도 적용
        velocity.y += gravity * Time.deltaTime;

        // 수직 이동 적용
        controller.Move(velocity * Time.deltaTime);
    }

    /// <summary>
    /// 점프 입력 처리 (Space: 기본 점프, Shift + Space: 높은 점프)
    /// </summary>
    private void HandleJump()
    {
        // 땅에 닿아있지 않으면 점프 불가
        if (!isGrounded) return;

        bool spacePressed = Input.GetKeyDown(KeyCode.Space);
        bool shiftPressed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        // Space 키를 눌렀을 때
        if (spacePressed)
        {
            // Shift를 함께 누르고 있으면 높은 점프
            if (shiftPressed)
            {
                velocity.y = Mathf.Sqrt(highJumpHeight * -2f * gravity);
            }
            // Space만 누르면 기본 점프
            else
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }
    }

    /// <summary>
    /// CharacterController가 충돌할 때 호출되어 물체에 힘을 가함
    /// </summary>
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        
        Rigidbody body = hit.collider.attachedRigidbody;

        // Rigidbody가 없거나 kinematic이면 무시
        if (body == null || body.isKinematic)
            return;

        // 바닥을 밀지 않도록 위쪽 충돌만 무시
        if (hit.moveDirection.y < -0.3f)
            return;

        // 이동 방향으로 힘 계산
        Vector3 pushDirection = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
        
        // 문(HingeJoint)인 경우 충돌 지점에 힘을 가함 (더 효과적)
        if (body.GetComponent<HingeJoint>() != null)
        {
            // 문에는 충돌 지점에 직접 힘을 가해서 회전시킴
            body.AddForceAtPosition(pushDirection * pushPower * 50f, hit.point, ForceMode.Force);
        }
        else
        {
            // 일반 오브젝트는 속도 직접 설정
            body.linearVelocity = pushDirection * pushPower;
        }
    }
}

