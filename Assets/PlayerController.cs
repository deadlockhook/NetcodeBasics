using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(NetworkObject))]
public class PlayerController : NetworkBehaviour
{
    public float speed = 5f;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.freezeRotation = true; 

    }

    private void FixedUpdate() 
    {
        if (!IsOwner) return;

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(moveX, 0, moveZ).normalized;
        MoveServerRpc(moveDirection);
    }

    [ServerRpc]
    private void MoveServerRpc(Vector3 direction, ServerRpcParams rpcParams = default)
    {
        ulong clientId = rpcParams.Receive.SenderClientId;

        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(clientId, out NetworkClient client))
        {
            Rigidbody playerRb = client.PlayerObject.GetComponent<Rigidbody>();

            if (playerRb != null)
            {
                playerRb.velocity = direction * speed;
                UpdatePositionClientRpc(playerRb.position, playerRb.velocity);
            }
        }
    }

    [ClientRpc]
    private void UpdatePositionClientRpc(Vector3 position, Vector3 velocity)
    {
        if (!IsOwner)
            rb.velocity = velocity; 
        
    }

}
