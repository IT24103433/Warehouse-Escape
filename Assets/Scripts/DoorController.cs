using UnityEngine;

public class DoorController : MonoBehaviour
{
    public float interactionDistance = 3f;
    public float openHeight = 4f;
    public float doorSpeed = 3f;

    private Transform player;
    private Vector3 closedPosition;
    private Vector3 openPosition;

    private bool isOpen = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        closedPosition = transform.position;
        openPosition = closedPosition + Vector3.up * openHeight;
    }

    void Update()
    {
        // Calculate distance between player and door
        float distance = Vector3.Distance(
            player.position,
            transform.position
        );

        // Press E when close to the door
        if (distance <= interactionDistance &&
            Input.GetKeyDown(KeyCode.E))
        {
            isOpen = !isOpen;
        }

        // Decide where the door should move
        Vector3 targetPosition;

        if (isOpen)
        {
            targetPosition = openPosition;
        }
        else
        {
            targetPosition = closedPosition;
        }

        // Smoothly move the door
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            doorSpeed * Time.deltaTime
        );
    }
}