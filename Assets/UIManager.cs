using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

public class UIManager : MonoBehaviour
{
    public TMP_InputField ipField;
    public TMP_InputField portField;
    private UnityTransport transport;

    private void OnClientConnected(ulong clientId)
    {
        Debug.Log($"Client {clientId} Connected!");

        if (NetworkManager.Singleton.IsServer)
        {
            // Get the player's object and assign ownership
            if (NetworkManager.Singleton.ConnectedClients.TryGetValue(clientId, out NetworkClient client))
            {
                if (client.PlayerObject != null && !client.PlayerObject.IsOwner)
                {
                    client.PlayerObject.GetComponent<NetworkObject>().ChangeOwnership(clientId);
                    Debug.Log($"Ownership assigned to Client {clientId}");
                }
            }
        }
    }

    private void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

        if (transport == null)
        {
            Debug.LogError("Unity Transport not found! Please add it to NetworkManager.");
            return;
        }

        // Default IP & Port values
        ipField.text = "127.0.0.1";
        portField.text = "7777";
    }

    public void OnClose()
    {
        Application.Quit();
    }

    public void OnHost()
    {
        if (transport == null) return;

        int port = int.Parse(portField.text);
        transport.SetConnectionData("127.0.0.1", (ushort)port);

        NetworkManager.Singleton.StartHost();
        Debug.Log("Started as Host on port " + port);
    }

    public void OnJoin()
    {
        if (transport == null) return;

        string ip = ipField.text;
        int port = int.Parse(portField.text);
        transport.SetConnectionData(ip, (ushort)port);

        NetworkManager.Singleton.StartClient();
        Debug.Log("Connecting to " + ip + " on port " + port);
    }

}
