using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

public class GameplayManager : MonoBehaviour
{
    public TMP_InputField ipField;
    public TMP_InputField portField;
    private UnityTransport transport;
    public GameObject canvasGameObject;

    private void OnClientConnected(ulong clientId)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            if (NetworkManager.Singleton.ConnectedClients.TryGetValue(clientId, out NetworkClient client))
            {
                if (client.PlayerObject != null && !client.PlayerObject.IsOwner)
                {
                    client.PlayerObject.GetComponent<NetworkObject>().ChangeOwnership(clientId);
                }
            }
        }
    }

    private void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

        if (transport == null)
            return;

        ipField.text = "127.0.0.1";
        portField.text = "7777";
    }

    public void OnClose()
    {
        Application.Quit();
    }

    public void OnHost()
    {
        Destroy(canvasGameObject);
        if (transport == null) return;

        int port = int.Parse(portField.text);
        transport.SetConnectionData("127.0.0.1", (ushort)port);

        NetworkManager.Singleton.StartHost();
    }

    public void OnJoin()
    {
        Destroy(canvasGameObject);
        if (transport == null) return;

        string ip = ipField.text;
        int port = int.Parse(portField.text);
        transport.SetConnectionData(ip, (ushort)port);

        NetworkManager.Singleton.StartClient();
    }

}
