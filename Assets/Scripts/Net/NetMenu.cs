using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
public class NetMenu : MonoBehaviour
{
    [SerializeField] Button hostButton, clientButton;
    void Awake()
    {
        hostButton.onClick.AddListener(StartHost);
        clientButton.onClick.AddListener(StartClient);
    }

    void StartHost()
    {
        NetworkManager.Singleton.StartHost();
    }

    void StartClient()
    {
        NetworkManager.Singleton.StartClient();
    }

    void Start()
    {
        
    }

}   