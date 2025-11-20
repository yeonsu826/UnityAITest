using UnityEngine;

/// <summary>
/// 플레이어를 따라다니는 3인칭 카메라 (마우스 회전, 휠 줌 지원)
/// </summary>
public class ThirdPersonCamera : MonoBehaviour
{
    [Header("타겟 설정")]
    [SerializeField] private Transform target; // 따라다닐 플레이어 Transform

    [Header("카메라 위치 설정")]
    // 카메라의 기본 위치 오프셋 (플레이어 기준 높이 1, 뒤로 2만큼)
    // 카메라가 플레이어를 기준으로 위치할 오프셋 값입니다. 
    // y값(=1)은 카메라가 플레이어보다 얼마나 위에 있을지(높이), z값(=-2)은 얼마나 뒤에 있을지(거리)를 의미합니다.
    // 예시: (0, 1, -2)이면 플레이어 기준으로 1만큼 위 + 2만큼 뒤에 카메라가 위치합니다.
    private Vector3 offset = new Vector3(0f, 1f, -3f);
    [SerializeField] private float smoothSpeed = 10f; // 카메라 이동 부드러움 정도

    [Header("마우스 회전 설정")]
    [SerializeField] private bool enableMouseRotation = true; // 마우스로 카메라 회전 활성화
    [SerializeField] private float mouseSensitivity = 100f; // 마우스 감도
    private float fixedVerticalAngle = -20f; // 고정된 수직 각도 (위아래 각도)
    private float rotationDistance = 4f; // 플레이어와의 기본 거리

    [Header("줌 설정")]
    [SerializeField] private bool enableZoom = true; // 마우스 휠 줌 활성화
    [SerializeField] private float zoomSpeed = 2f; // 줌 속도
    [SerializeField] private float minDistance = 2f; // 최소 거리
    [SerializeField] private float maxDistance = 10f; // 최대 거리

    /// <summary>
    /// 현재 수평 회전 각도 (좌우)
    /// </summary>
    private float currentYaw = 0f;

    /// <summary>
    /// 현재 카메라와 플레이어 사이의 거리
    /// </summary>
    private float currentDistance;

    /// <summary>
    /// 초기화
    /// </summary>
    void Start()
    {
        // 타겟이 설정되지 않았으면 경고
        if (target == null)
        {
            Debug.LogWarning("ThirdPersonCamera: 타겟이 설정되지 않았습니다!");
        }

        // 마우스 커서 숨기기 (선택사항)
        if (enableMouseRotation)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        // 초기 거리 설정
        currentDistance = rotationDistance;
    }

    /// <summary>
    /// 카메라 업데이트 (LateUpdate 사용으로 플레이어 이동 후 실행)
    /// </summary>
    void LateUpdate()
    {
        if (target == null) return;

        if (enableMouseRotation)
        {
            HandleMouseRotation();
        }

        if (enableZoom)
        {
            HandleZoom();
        }

        HandleCameraPosition();
    }

    /// <summary>
    /// 마우스 입력으로 카메라 회전 처리 (좌우만)
    /// </summary>
    private void HandleMouseRotation()
    {
        // 마우스 입력 받기 (좌우만)
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;

        // 수평 각도만 업데이트
        currentYaw += mouseX;

        // ESC 키로 커서 토글
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }

    /// <summary>
    /// 마우스 휠 입력으로 줌 처리
    /// </summary>
    private void HandleZoom()
    {
        // 마우스 휠 입력 받기
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        // 거리 조정 (휠 위로 = 줌인 = 거리 감소, 휠 아래로 = 줌아웃 = 거리 증가)
        currentDistance -= scrollInput * zoomSpeed;

        // 거리 제한
        currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);
    }

    /// <summary>
    /// 카메라 위치 및 회전 업데이트
    /// </summary>
    private void HandleCameraPosition()
    {
        Vector3 desiredPosition;

        if (enableMouseRotation)
        {
            // 마우스 회전이 활성화된 경우: 구면 좌표계로 위치 계산 (좌우 회전만, 수직 각도 고정)
            Quaternion rotation = Quaternion.Euler(fixedVerticalAngle, currentYaw, 0);
            Vector3 direction = rotation * Vector3.back;
            desiredPosition = target.position + direction * currentDistance + Vector3.up * 2f;
        }
        else
        {
            // 고정 오프셋 사용
            desiredPosition = target.position + offset;
        }

        // 부드럽게 이동
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // 항상 타겟을 바라보기
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }
}

