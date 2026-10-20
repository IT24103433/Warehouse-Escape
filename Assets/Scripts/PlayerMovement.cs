using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float gravity = -9.81f;
    public float mouseSensitivity = 50f;

    public bool IsCrouching { get; private set; }
    public bool IsAiming { get; private set; }
    public float jumpHeight = 1.1f;
    private float standingHeight;
    private Vector3 standingCenter, cameraHome;
    private float normalFov;
    private readonly Collider[] headroom = new Collider[32];
    private CharacterController controller;
    private Vector3 velocity;

    private Camera playerCamera;
    private float xRotation = 0f;

    public float CameraPitch
    {
        get => xRotation;
        set
        {
            xRotation = Mathf.Clamp(value, -80f, 80f);
            if (playerCamera != null)
                playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }
    }

    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();

        standingHeight = controller.height;
        standingCenter = controller.center;
        cameraHome = playerCamera.transform.localPosition;
        normalFov = playerCamera.fieldOfView;

        // Ensure Player Health
        var health = GetComponent<Health>();
        if (health == null)
        {
            health = gameObject.AddComponent<Health>();
        }
        health.SetIsPlayer(true);
        health.CharacterName = "Operative";
        health.SetMaxHealth(100);

        // Ensure First-Person Player Gun
        if (playerCamera != null && playerCamera.GetComponent<PlayerGun>() == null)
        {
            playerCamera.gameObject.AddComponent<PlayerGun>();
        }

        // Ensure Player HUD
        var uiCanvas = GameObject.Find("UI_Canvas");
        Canvas canvas = uiCanvas != null ? uiCanvas.GetComponent<Canvas>() : Object.FindFirstObjectByType<Canvas>();
        if (canvas != null && canvas.GetComponentInChildren<PlayerHUD>() == null)
        {
            canvas.gameObject.AddComponent<PlayerHUD>();
        }

        HumanoidMeshBuilder.EnsureRuntimeVisuals(transform, CharacterRole.Player);

        bool spawnAtGate = System.Linq.Enumerable.Any(System.Environment.GetCommandLineArgs(), a => a.Equals("-gate-test", System.StringComparison.OrdinalIgnoreCase));
        if (spawnAtGate)
        {
            if (controller != null) controller.enabled = false;
            transform.position = new Vector3(0f, 1.08f, 43.0f);
            transform.rotation = Quaternion.identity;
            if (controller != null) controller.enabled = true;
            Physics.SyncTransforms();
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Stop gameplay controls when result screen is active
        if (Time.timeScale == 0f || (GameManager.Instance != null && GameManager.Instance.IsGameEnded) || GetComponent<Health>().IsDead)
            return;

        if (Input.GetKeyDown(KeyCode.C)) SetCrouching(!IsCrouching);
        IsAiming = Input.GetMouseButton(1);
        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, IsAiming ? normalFov * .65f : normalFov, Time.deltaTime * 12f);
        playerCamera.transform.localPosition = Vector3.Lerp(playerCamera.transform.localPosition,
            cameraHome + (IsCrouching ? Vector3.down * (standingHeight * .45f) : Vector3.zero), Time.deltaTime * 12f);

        if (Input.GetKeyDown(KeyCode.Space)) TryJump();

        // Movement
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(Vector3.ClampMagnitude(move, 1f) * speed * (IsCrouching ? .5f : IsAiming ? .7f : 1f) * Time.deltaTime);

        // Gravity
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Mouse Look
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        playerCamera.transform.localRotation =
            Quaternion.Euler(xRotation, 0f, 0f);

        transform.Rotate(Vector3.up * mouseX);
    }

    public bool TryJump()
    {
        if (!controller.isGrounded || IsCrouching || Time.timeScale == 0f || GetComponent<Health>().IsDead ||
            (GameManager.Instance != null && GameManager.Instance.IsGameEnded)) return false;
        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        return true;
    }

    public bool SetCrouching(bool crouch)
    {
        if (!crouch && IsCrouching)
        {
            Vector3 center = transform.TransformPoint(standingCenter);
            float radius = controller.radius * .95f;
            float half = Mathf.Max(0f, standingHeight * .5f - controller.radius);
            int count = Physics.OverlapCapsuleNonAlloc(center + Vector3.up * half, center - Vector3.up * half,
                radius, headroom, ~0, QueryTriggerInteraction.Ignore);
            for (int i = 0; i < count; i++)
                if (!headroom[i].transform.IsChildOf(transform)) return false;
            if (count == headroom.Length) return false;
        }
        IsCrouching = crouch;
        controller.height = crouch ? standingHeight * .55f : standingHeight;
        controller.center = standingCenter - Vector3.up * ((standingHeight - controller.height) * .5f);
        return true;
    }

    /// <summary>
    /// Explicitly aims the player view toward a target world position.
    /// Sets root yaw and camera pitch synchronously so programmatic tests and automated benchmarks
    /// aim with precision without input-smoothing interference.
    /// </summary>
    public void AimAt(Vector3 targetWorldPos)
    {
        if (playerCamera == null) playerCamera = GetComponentInChildren<Camera>();
        Vector3 camPos = playerCamera != null ? playerCamera.transform.position : (transform.position + Vector3.up * 1.78f);
        Vector3 toTarget = targetWorldPos - camPos;
        if (toTarget.sqrMagnitude < 0.001f) return;

        // Yaw on player root
        Vector3 horizontalDir = toTarget;
        horizontalDir.y = 0f;
        if (horizontalDir.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.LookRotation(horizontalDir.normalized);
        }

        // Pitch on camera
        float distH = horizontalDir.magnitude;
        float pitch = -Mathf.Atan2(toTarget.y, distH) * Mathf.Rad2Deg;
        CameraPitch = pitch;
    }
}