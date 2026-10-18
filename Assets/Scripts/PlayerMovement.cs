using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float gravity = -9.81f;
    public float mouseSensitivity = 50f;

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
        if (GameManager.Instance != null && GameManager.Instance.IsGameEnded)
            return;

        // Movement
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * speed * Time.deltaTime);

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