using UnityEngine;

public class ExplosionTester : MonoBehaviour
{
    [Header("Test Settings")]
    [SerializeField] private KeyCode explosionKey = KeyCode.E;
    [SerializeField] private float spawnDistance = 5f;
    
    [Header("Explosion Settings")]
    [SerializeField] private float explosionForce = 500f;
    [SerializeField] private float explosionRadius = 5f;
    
    [Header("Visual Indicator")]
    [SerializeField] private bool showSpawnPoint = true;
    
    private Camera mainCamera;
    
    void Start()
    {
        mainCamera = Camera.main;
    }
    
    void Update()
    {
        if (Input.GetKeyDown(explosionKey))
        {
            SpawnExplosion();
        }
        
        // 마우스 클릭으로도 폭발 생성
        if (Input.GetMouseButtonDown(0))
        {
            SpawnExplosionAtMousePosition();
        }
    }
    
    void SpawnExplosion()
    {
        // 플레이어 앞쪽에 폭발 생성
        Vector3 explosionPosition = transform.position + transform.forward * spawnDistance;
        ExplosionEffect.CreateExplosion(explosionPosition, explosionForce, explosionRadius);
        
        Debug.Log($"폭발 생성: {explosionPosition}");
    }
    
    void SpawnExplosionAtMousePosition()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit))
        {
            // 레이캐스트가 맞은 위치에 폭발 생성
            ExplosionEffect.CreateExplosion(hit.point, explosionForce, explosionRadius);
            Debug.Log($"폭발 생성 (마우스 클릭): {hit.point}");
        }
        else
        {
            // 레이캐스트가 맞지 않으면 카메라 앞쪽에 생성
            Vector3 explosionPosition = mainCamera.transform.position + mainCamera.transform.forward * spawnDistance;
            ExplosionEffect.CreateExplosion(explosionPosition, explosionForce, explosionRadius);
            Debug.Log($"폭발 생성 (카메라 앞): {explosionPosition}");
        }
    }
    
    void OnDrawGizmos()
    {
        if (!showSpawnPoint) return;
        
        // 폭발이 생성될 위치를 시각화
        Gizmos.color = Color.yellow;
        Vector3 spawnPos = transform.position + transform.forward * spawnDistance;
        Gizmos.DrawWireSphere(spawnPos, 0.5f);
    }
}

