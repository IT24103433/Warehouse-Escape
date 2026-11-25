using UnityEngine;

/// <summary>Short-lived damage feedback, created only after accepted health damage.</summary>
[RequireComponent(typeof(Health))]
public class DamageBloodEffect : MonoBehaviour
{
    private Health health;
    private float intensity;
    private Texture2D blood;
    public float Intensity => intensity;
    void Awake() { health = GetComponent<Health>(); }
    void OnEnable() { health.OnDamaged += OnDamage; }
    void OnDisable() { health.OnDamaged -= OnDamage; intensity = 0f; }
    void OnDestroy() { if (blood != null) Destroy(blood); }
    void Update() { intensity = Mathf.MoveTowards(intensity, 0f, Time.deltaTime * .65f); }
    private void OnDamage(int amount, GameObject source)
    {
        intensity = Mathf.Clamp01(intensity + .4f + amount / 100f);
        if (health.IsPlayer) return;
        var go = new GameObject("Blood_Hit_Burst");
        go.transform.position = transform.position + Vector3.up * .65f;
        var particles = go.AddComponent<ParticleSystem>();
        particles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        var main = particles.main;
        main.loop = false; main.duration = .15f; main.startLifetime = .4f;
        main.startSpeed = 1.2f; main.startSize = .045f;
        main.startColor = new Color(.55f, .015f, .025f, .9f);
        main.gravityModifier = .7f; main.maxParticles = 20;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        var emission = particles.emission; emission.enabled = false;
        var shape = particles.shape; shape.shapeType = ParticleSystemShapeType.Sphere; shape.radius = .12f;
        var renderer = go.GetComponent<ParticleSystemRenderer>();
        // Reuse the URP particle material rather than allocating materials per hit.
        renderer.sharedMaterial = Resources.Load<Material>("Effects/BloodParticles");
        particles.Emit(14);
        Destroy(go, 1f);
    }
    void OnGUI()
    {
        if (!health.IsPlayer || intensity <= 0f || Time.timeScale == 0f) return;
        if (blood == null)
        {
            blood = new Texture2D(128, 128, TextureFormat.RGBA32, false);
            blood.wrapMode = TextureWrapMode.Clamp;
            for (int y=0; y<128; y++) for (int x=0; x<128; x++)
            {
                float edge = Mathf.Max(Mathf.Abs(x / 127f * 2f - 1f), Mathf.Abs(y / 127f * 2f - 1f));
                float flecks = Mathf.PerlinNoise(x * .19f, y * .19f);
                float alpha = Mathf.SmoothStep(0f, .8f, Mathf.InverseLerp(.6f, 1f, edge)) * (.35f + flecks * .65f);
                blood.SetPixel(x,y,new Color(.55f,.005f,.015f,alpha));
            }
            blood.Apply(false, true);
        }
        Color previous = GUI.color;
        GUI.color = new Color(1f,1f,1f,intensity);
        GUI.DrawTexture(new Rect(0,0,Screen.width,Screen.height),blood);
        GUI.color = previous;
    }
}
