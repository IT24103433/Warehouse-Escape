using UnityEngine;

public enum WarehouseTrapKind { Training, Alarm, Shock }
public enum WarehouseTrapState { Ready, Warning, Active, Cooldown }

/// <summary>Reusable, telegraphed floor trap. Trigger geometry never blocks navigation.</summary>
[RequireComponent(typeof(BoxCollider))]
public class WarehouseTrap : MonoBehaviour
{
    [SerializeField] private WarehouseTrapKind kind;
    [SerializeField] private int roomNumber;
    [SerializeField] private int damage = 15;
    [SerializeField] private float warningSeconds = 1.25f;
    [SerializeField] private float activeSeconds = 1f;
    [SerializeField] private float cooldownSeconds = 5f;
    private readonly Collider[] overlaps = new Collider[16];
    private BoxCollider sensor;
    private Renderer plate;
    private TextMesh label;
    private Material visualMaterial;
    private Material textMaterial;
    private float phaseTimer, nextHitAt;
    public WarehouseTrapKind Kind => kind;
    public int RoomNumber => roomNumber;
    public WarehouseTrapState State { get; private set; }
    public int Damage => damage;

    public void Configure(WarehouseTrapKind type, int room, int hitDamage)
    { kind = type; roomNumber = room; damage = hitDamage; }

    void Awake()
    {
        sensor = GetComponent<BoxCollider>();
        sensor.isTrigger = true;
        plate = transform.Find("Trap_Plate")?.GetComponent<Renderer>();
        label = GetComponentInChildren<TextMesh>();
        if (label != null)
        {
            label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            textMaterial = new Material(Shader.Find("Custom/DepthTestedText"));
            textMaterial.mainTexture = label.font.material.mainTexture;
            label.GetComponent<Renderer>().sharedMaterial = textMaterial;
            label.transform.localScale = Vector3.one * .1f;
            label.characterSize = .45f;
        }
        if (plate != null) visualMaterial = plate.material;
        RefreshVisual();
    }

    private bool Occupied(out Health player)
    {
        player = null;
        var center = transform.TransformPoint(sensor.center);
        var scale = transform.lossyScale;
        var half = Vector3.Scale(sensor.size * .5f, new Vector3(Mathf.Abs(scale.x), Mathf.Abs(scale.y), Mathf.Abs(scale.z)));
        int count = Physics.OverlapBoxNonAlloc(center, half, overlaps, transform.rotation, ~0, QueryTriggerInteraction.Ignore);
        for (int i = 0; i < count; i++)
        {
            var h = overlaps[i].GetComponentInParent<Health>();
            if (h != null && h.IsPlayer && !h.IsDead) { player = h; return true; }
        }
        return false;
    }

    public void ResetTrap()
    {
        State = WarehouseTrapState.Ready;
        phaseTimer = 0;
        nextHitAt = 0;
        RefreshVisual();
    }

    void Update()
    {
        if (GameManager.Instance != null && (GameManager.Instance.IsPaused || GameManager.Instance.IsGameEnded)) return;
        phaseTimer -= Time.deltaTime;
        switch (State)
        {
            case WarehouseTrapState.Ready:
                if (Occupied(out _))
                {
                    State = WarehouseTrapState.Warning;
                    phaseTimer = warningSeconds;
                    if (kind != WarehouseTrapKind.Training)
                        GuardPerception.ReportSound(transform.position, 28f, SoundType.Alarm, gameObject);
                    RefreshVisual();
                }
                break;
            case WarehouseTrapState.Warning:
                if (phaseTimer <= 0)
                {
                    State = WarehouseTrapState.Active;
                    phaseTimer = activeSeconds;
                    nextHitAt = Time.time;
                    RefreshVisual();
                }
                break;
            case WarehouseTrapState.Active:
                if (phaseTimer <= 0)
                {
                    State = WarehouseTrapState.Cooldown;
                    phaseTimer = cooldownSeconds;
                    RefreshVisual();
                }
                else if (kind == WarehouseTrapKind.Shock && Time.time >= nextHitAt && Occupied(out var player))
                {
                    nextHitAt = Time.time + .5f;
                    player.TakeDamage(damage, gameObject);
                }
                break;
            case WarehouseTrapState.Cooldown:
                if (phaseTimer <= 0) ResetTrap();
                break;
        }
        if (label != null && Camera.main != null)
        {
            var camera = Camera.main;
            bool near = Vector3.Distance(camera.transform.position, label.transform.position) < 7;
            label.gameObject.SetActive(near && GuardDecisionQueries.Clear(camera.transform.position,
                label.transform.position, ~0, transform));
            label.transform.rotation = Quaternion.LookRotation(label.transform.position - camera.transform.position);
        }
    }

    private void RefreshVisual()
    {
        Color color = State == WarehouseTrapState.Warning ? new Color(1, .65f, .05f)
            : State == WarehouseTrapState.Active ? Color.red
            : State == WarehouseTrapState.Cooldown ? Color.gray : new Color(.1f, .7f, 1);
        if (visualMaterial != null)
        {
            visualMaterial.SetColor("_BaseColor", color);
            visualMaterial.SetColor("_EmissionColor", color * .6f);
            visualMaterial.EnableKeyword("_EMISSION");
        }
        if (label != null)
        {
            string title = kind == WarehouseTrapKind.Training ? "TRAINING PAD - NO DAMAGE"
                : kind == WarehouseTrapKind.Alarm ? "SECURITY ALARM" : "SHOCK PAD";
            label.text = title + "\n" + (State == WarehouseTrapState.Warning ? "MOVE OFF THE PAD!"
                : State == WarehouseTrapState.Active ? "ACTIVE - KEEP CLEAR" : State.ToString().ToUpperInvariant());
            label.color = color;
        }
    }

    void OnDestroy()
    {
        if (visualMaterial != null) Destroy(visualMaterial);
        if (textMaterial != null) Destroy(textMaterial);
    }
}
