using UnityEngine;

/// <summary>
/// 충돌 시 파괴 가능한 물체 (자식 오브젝트에 자동으로 Rigidbody 추가하여 파편으로 만듦)
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class DestructibleObject : MonoBehaviour
{
    [Header("파괴 설정")]
    [SerializeField] private float health = 100f; // 물체의 체력 (플레이어 충돌 시 무시됨)
    [SerializeField] private float minDamageSpeed = 1f; // 파괴되는 최소 속도
    [SerializeField] private float damageMultiplier = 20f; // 데미지 배율 (플레이어 충돌 시 무시됨)

    [Header("파괴 효과")]
    [SerializeField] private bool explodeOnDestroy = true; // 파괴 시 폭발 효과
    [SerializeField] private float explosionForce = 500f; // 폭발 힘
    [SerializeField] private float explosionRadius = 5f; // 폭발 반경
    [SerializeField] private GameObject destructionEffect; // 파괴 이펙트 프리팹 (선택사항)

    [Header("파편 설정")]
    [SerializeField] private bool createFragments = true; // 파편 생성 여부
    [SerializeField] private int fragmentCount = 5; // 파편 개수
    [SerializeField] private GameObject fragmentPrefab; // 파편 프리팹 (선택사항)
    [SerializeField] private bool useChildrenAsFragments = true; // 자식 오브젝트를 파편으로 사용
    [SerializeField] private float childFragmentLifetime = 5f; // 자식 파편이 사라지는 시간

    /// <summary>
    /// 현재 체력
    /// </summary>
    private float currentHealth;

    /// <summary>
    /// Rigidbody 컴포넌트
    /// </summary>
    private Rigidbody rb;

    /// <summary>
    /// MeshRenderer 컴포넌트
    /// </summary>
    private MeshRenderer meshRenderer;

    /// <summary>
    /// 초기화
    /// </summary>
    void Start()
    {
        currentHealth = health;
        rb = GetComponent<Rigidbody>();
        meshRenderer = GetComponent<MeshRenderer>();

        // 위치 고정 (중력으로 떨어지지 않도록)
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }
    }

    /// <summary>
    /// 플레이어에게 충돌당했을 때 호출 (PlayerMovement에서 호출) - 즉시 파괴
    /// </summary>
    public void OnPlayerHit(Vector3 hitPoint, float impactSpeed)
    {
        // 제약 해제
        if (rb != null && rb.constraints != RigidbodyConstraints.None)
        {
            rb.constraints = RigidbodyConstraints.None;
        }

        // 체력과 속도 무시하고 즉시 파괴
        DestroyObject(hitPoint);
    }

    /// <summary>
    /// 데미지를 받는 처리
    /// </summary>
    private void TakeDamage(float damage, Vector3 hitPoint)
    {
        currentHealth -= damage;

        // 체력이 0 이하가 되면 파괴
        if (currentHealth <= 0)
        {
            DestroyObject(hitPoint);
        }
    }

    /// <summary>
    /// 외부에서 데미지를 적용 (폭발 등)
    /// </summary>
    public void ApplyDamage(float damage, Vector3 hitPoint)
    {
        TakeDamage(damage, hitPoint);
    }

    /// <summary>
    /// 물체 파괴 처리
    /// </summary>
    private void DestroyObject(Vector3 hitPoint)
    {
        // 파괴 이펙트 생성
        if (destructionEffect != null)
        {
            Instantiate(destructionEffect, transform.position, Quaternion.identity);
        }

        // 폭발 파티클 이펙트 생성
        ExplosionEffect.CreateExplosion(transform.position, explosionForce, explosionRadius);

        // 자식 오브젝트를 파편으로 사용
        if (useChildrenAsFragments && transform.childCount > 0)
        {
            ActivateChildrenAsFragments(hitPoint);
        }
        // 일반 파편 생성
        else if (createFragments)
        {
            CreateFragments(hitPoint);
        }

        // 폭발 효과 (주변 물체에 힘 가하기)
        if (explodeOnDestroy)
        {
            ApplyExplosionForce();
        }

        // 물체 제거
        Destroy(gameObject);
    }

    /// <summary>
    /// 파편 생성
    /// </summary>
    private void CreateFragments(Vector3 hitPoint)
    {
        // 파편 프리팹이 있으면 그것을 사용, 없으면 기본 큐브 생성
        for (int i = 0; i < fragmentCount; i++)
        {
            GameObject fragment;

            if (fragmentPrefab != null)
            {
                fragment = Instantiate(fragmentPrefab, transform.position, Random.rotation);
            }
            else
            {
                // 기본 파편: 작은 큐브
                fragment = GameObject.CreatePrimitive(PrimitiveType.Cube);
                fragment.transform.position = transform.position + Random.insideUnitSphere * 0.5f;
                fragment.transform.rotation = Random.rotation;
                fragment.transform.localScale = transform.localScale * Random.Range(0.1f, 0.3f);

                // 원래 물체의 머티리얼 복사
                if (meshRenderer != null)
                {
                    fragment.GetComponent<MeshRenderer>().material = meshRenderer.material;
                }
            }

            // 파편에 물리 적용
            Rigidbody fragmentRb = fragment.GetComponent<Rigidbody>();
            if (fragmentRb == null)
            {
                fragmentRb = fragment.AddComponent<Rigidbody>();
            }

            // 랜덤한 방향으로 힘 가하기
            Vector3 explosionDirection = (fragment.transform.position - hitPoint).normalized;
            fragmentRb.AddForce(explosionDirection * explosionForce * 0.5f);
            fragmentRb.AddTorque(Random.insideUnitSphere * explosionForce * 0.3f);

            // 5초 후 파편 제거
            Destroy(fragment, 5f);
        }
    }

    /// <summary>
    /// 자식 오브젝트들을 파편으로 활성화
    /// </summary>
    private void ActivateChildrenAsFragments(Vector3 hitPoint)
    {
        // 모든 자식 오브젝트를 배열에 저장 (반복 중 부모 변경 방지)
        Transform[] children = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            children[i] = transform.GetChild(i);
        }

        // 각 자식 오브젝트 처리
        foreach (Transform child in children)
        {
            // 부모로부터 분리
            child.SetParent(null);

            // Rigidbody 추가 또는 가져오기
            Rigidbody childRb = child.GetComponent<Rigidbody>();
            if (childRb == null)
            {
                childRb = child.gameObject.AddComponent<Rigidbody>();
            }

            // Collider 확인 (없으면 BoxCollider 추가)
            Collider childCollider = child.GetComponent<Collider>();
            if (childCollider == null)
            {
                child.gameObject.AddComponent<BoxCollider>();
            }

            // 폭발 방향 계산
            Vector3 explosionDirection = (child.position - hitPoint).normalized;
            if (explosionDirection == Vector3.zero)
            {
                explosionDirection = Random.insideUnitSphere.normalized;
            }

            // 힘 가하기
            childRb.AddForce(explosionDirection * explosionForce, ForceMode.Impulse);
            childRb.AddTorque(Random.insideUnitSphere * explosionForce * 0.3f, ForceMode.Impulse);

            // 일정 시간 후 파편 제거
            Destroy(child.gameObject, childFragmentLifetime);
        }
    }

    /// <summary>
    /// 주변에 폭발 힘 적용
    /// </summary>
    private void ApplyExplosionForce()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider nearbyObject in colliders)
        {
            Rigidbody nearbyRb = nearbyObject.GetComponent<Rigidbody>();
            if (nearbyRb != null && nearbyRb != rb)
            {
                nearbyRb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
        }
    }

    /// <summary>
    /// 디버그용: 폭발 반경 그리기
    /// </summary>
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}

