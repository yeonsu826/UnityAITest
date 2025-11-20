using UnityEngine;

public class ExplosionEffect : MonoBehaviour
{
    [Header("Explosion Settings")]
    [SerializeField] private float explosionForce = 500f;
    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private float upwardsModifier = 1f;
    [SerializeField] private float lifetime = 3f;
    
    [Header("Particle Colors")]
    [SerializeField] private Gradient fireGradient;
    [SerializeField] private Gradient smokeGradient;
    
    private ParticleSystem fireParticles;
    private ParticleSystem smokeParticles;
    private ParticleSystem sparkParticles;
    private Light explosionLight;
    
    void Start()
    {
        CreateExplosionEffect();
        ApplyExplosionForce();
        Destroy(gameObject, lifetime);
    }
    
    void CreateExplosionEffect()
    {
        // 메인 폭발 (불꽃) 파티클
        fireParticles = CreateFireParticles();
        
        // 연기 파티클
        smokeParticles = CreateSmokeParticles();
        
        // 스파크 파티클
        sparkParticles = CreateSparkParticles();
        
        // 폭발 빛 효과
        CreateExplosionLight();
    }
    
    ParticleSystem CreateFireParticles()
    {
        GameObject fireObj = new GameObject("FireParticles");
        fireObj.transform.SetParent(transform);
        fireObj.transform.localPosition = Vector3.zero;
        
        ParticleSystem ps = fireObj.AddComponent<ParticleSystem>();
        var main = ps.main;
        main.startLifetime = 0.8f;
        main.startSpeed = new ParticleSystem.MinMaxCurve(8f, 15f);
        main.startSize = new ParticleSystem.MinMaxCurve(1f, 2f);
        main.startRotation = new ParticleSystem.MinMaxCurve(0, 360f * Mathf.Deg2Rad);
        main.maxParticles = 100;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        
        // 색상 설정
        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        if (fireGradient != null)
        {
            colorOverLifetime.color = new ParticleSystem.MinMaxGradient(fireGradient);
        }
        else
        {
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] { 
                    new GradientColorKey(new Color(1f, 0.9f, 0.4f), 0f),      // 밝은 노란색
                    new GradientColorKey(new Color(1f, 0.4f, 0f), 0.3f),       // 주황색
                    new GradientColorKey(new Color(0.8f, 0.1f, 0f), 0.7f),     // 빨간색
                    new GradientColorKey(new Color(0.2f, 0.2f, 0.2f), 1f)      // 어두운 회색
                },
                new GradientAlphaKey[] { 
                    new GradientAlphaKey(1f, 0f),
                    new GradientAlphaKey(1f, 0.5f),
                    new GradientAlphaKey(0f, 1f)
                }
            );
            colorOverLifetime.color = new ParticleSystem.MinMaxGradient(gradient);
        }
        
        // 크기 변화
        var sizeOverLifetime = ps.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        AnimationCurve sizeCurve = new AnimationCurve();
        sizeCurve.AddKey(0f, 0f);
        sizeCurve.AddKey(0.2f, 1f);
        sizeCurve.AddKey(1f, 0.3f);
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, sizeCurve);
        
        // Emission
        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] {
            new ParticleSystem.Burst(0f, 50, 80)
        });
        
        // Shape
        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.1f;
        
        // Renderer
        var renderer = ps.GetComponent<ParticleSystemRenderer>();
        renderer.renderMode = ParticleSystemRenderMode.Billboard;
        renderer.material = new Material(Shader.Find("Particles/Standard Unlit"));
        
        return ps;
    }
    
    ParticleSystem CreateSmokeParticles()
    {
        GameObject smokeObj = new GameObject("SmokeParticles");
        smokeObj.transform.SetParent(transform);
        smokeObj.transform.localPosition = Vector3.zero;
        
        ParticleSystem ps = smokeObj.AddComponent<ParticleSystem>();
        var main = ps.main;
        main.startLifetime = 2f;
        main.startSpeed = new ParticleSystem.MinMaxCurve(3f, 8f);
        main.startSize = new ParticleSystem.MinMaxCurve(1.5f, 3f);
        main.startRotation = new ParticleSystem.MinMaxCurve(0, 360f * Mathf.Deg2Rad);
        main.maxParticles = 50;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.gravityModifier = -0.2f; // 연기가 위로 올라감
        
        // 색상 설정
        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        if (smokeGradient != null)
        {
            colorOverLifetime.color = new ParticleSystem.MinMaxGradient(smokeGradient);
        }
        else
        {
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] { 
                    new GradientColorKey(new Color(0.3f, 0.3f, 0.3f), 0f),
                    new GradientColorKey(new Color(0.5f, 0.5f, 0.5f), 0.5f),
                    new GradientColorKey(new Color(0.6f, 0.6f, 0.6f), 1f)
                },
                new GradientAlphaKey[] { 
                    new GradientAlphaKey(0.8f, 0f),
                    new GradientAlphaKey(0.5f, 0.5f),
                    new GradientAlphaKey(0f, 1f)
                }
            );
            colorOverLifetime.color = new ParticleSystem.MinMaxGradient(gradient);
        }
        
        // 크기 변화
        var sizeOverLifetime = ps.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        AnimationCurve sizeCurve = new AnimationCurve();
        sizeCurve.AddKey(0f, 0.5f);
        sizeCurve.AddKey(1f, 2f);
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, sizeCurve);
        
        // Emission
        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] {
            new ParticleSystem.Burst(0.1f, 20, 40)
        });
        
        // Shape
        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.5f;
        
        // Renderer
        var renderer = ps.GetComponent<ParticleSystemRenderer>();
        renderer.renderMode = ParticleSystemRenderMode.Billboard;
        renderer.material = new Material(Shader.Find("Particles/Standard Unlit"));
        
        return ps;
    }
    
    ParticleSystem CreateSparkParticles()
    {
        GameObject sparkObj = new GameObject("SparkParticles");
        sparkObj.transform.SetParent(transform);
        sparkObj.transform.localPosition = Vector3.zero;
        
        ParticleSystem ps = sparkObj.AddComponent<ParticleSystem>();
        var main = ps.main;
        main.startLifetime = new ParticleSystem.MinMaxCurve(0.3f, 0.8f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(10f, 20f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.05f, 0.15f);
        main.maxParticles = 200;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.gravityModifier = 2f; // 스파크가 아래로 떨어짐
        
        // 색상 설정
        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { 
                new GradientColorKey(new Color(1f, 1f, 0.8f), 0f),
                new GradientColorKey(new Color(1f, 0.6f, 0.2f), 0.5f),
                new GradientColorKey(new Color(1f, 0.2f, 0f), 1f)
            },
            new GradientAlphaKey[] { 
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(1f, 0.7f),
                new GradientAlphaKey(0f, 1f)
            }
        );
        colorOverLifetime.color = new ParticleSystem.MinMaxGradient(gradient);
        
        // Emission
        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] {
            new ParticleSystem.Burst(0f, 100, 150)
        });
        
        // Shape
        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.1f;
        
        // Trail (꼬리 효과)
        var trails = ps.trails;
        trails.enabled = true;
        trails.ratio = 0.5f;
        trails.lifetime = 0.2f;
        trails.minVertexDistance = 0.2f;
        
        // Renderer
        var renderer = ps.GetComponent<ParticleSystemRenderer>();
        renderer.renderMode = ParticleSystemRenderMode.Billboard;
        renderer.material = new Material(Shader.Find("Particles/Standard Unlit"));
        
        return ps;
    }
    
    void CreateExplosionLight()
    {
        GameObject lightObj = new GameObject("ExplosionLight");
        lightObj.transform.SetParent(transform);
        lightObj.transform.localPosition = Vector3.zero;
        
        explosionLight = lightObj.AddComponent<Light>();
        explosionLight.type = LightType.Point;
        explosionLight.color = new Color(1f, 0.6f, 0.2f);
        explosionLight.intensity = 8f;
        explosionLight.range = explosionRadius * 2f;
        
        // 빛이 서서히 사라지도록
        StartCoroutine(FadeLight());
    }
    
    System.Collections.IEnumerator FadeLight()
    {
        float elapsed = 0f;
        float duration = 0.5f;
        float startIntensity = explosionLight.intensity;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            explosionLight.intensity = Mathf.Lerp(startIntensity, 0f, t);
            yield return null;
        }
        
        explosionLight.enabled = false;
    }
    
    void ApplyExplosionForce()
    {
        // 주변의 Rigidbody에 폭발력 적용
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, upwardsModifier);
            }
            
            // 파괴 가능한 오브젝트가 있다면 데미지 적용
            DestructibleObject destructible = hit.GetComponent<DestructibleObject>();
            if (destructible != null)
            {
                destructible.ApplyDamage(100f, transform.position);
            }
        }
    }
    
    // 외부에서 폭발을 생성하기 위한 정적 메서드
    public static void CreateExplosion(Vector3 position, float force = 500f, float radius = 5f)
    {
        GameObject explosionObj = new GameObject("Explosion");
        explosionObj.transform.position = position;
        
        ExplosionEffect explosion = explosionObj.AddComponent<ExplosionEffect>();
        explosion.explosionForce = force;
        explosion.explosionRadius = radius;
    }
    
    void OnDrawGizmosSelected()
    {
        // 폭발 반경을 시각화
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}

