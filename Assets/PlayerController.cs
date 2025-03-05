using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(NetworkObject))]
public class PlayerController : NetworkBehaviour
{
    public float speed = 5f;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Enable interpolation for smoother movement
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.freezeRotation = true; // Prevent unintended rotation

        if (!IsOwner)
        {
            rb.isKinematic = true; // Non-owners should not apply physics forces
        }
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return; // Only allow the local player to move

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveX, 0, moveZ) * speed;

        // Apply force for movement
        rb.velocity = movement;

        // Send movement update to the server
        MoveServerRpc(rb.position);
    }

    [ServerRpc]
    private void MoveServerRpc(Vector3 position, ServerRpcParams rpcParams = default)
    {
        // Server validates movement and updates position
        rb.position = position;

        // Send back updated position to clients
        UpdatePositionClientRpc(rb.position);
    }

    [ClientRpc]
    private void UpdatePositionClientRpc(Vector3 position)
    {
        if (!IsOwner) // Only update position for non-owners
        {
            rb.position = Vector3.Lerp(rb.position, position, Time.fixedDeltaTime * 10);
        }
    }
}
