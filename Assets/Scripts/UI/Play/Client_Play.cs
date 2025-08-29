using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using Unity.Networking;
using System.Threading.Tasks;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;
using TMPro;
public class Client_Play : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(JoinGame);
    }


    // https://docs.unity.com/ugs/en-us/manual/relay/manual/relay-and-ngo#host_player

    public TMP_InputField joinCode;
    public async Task<bool> JoinClient(string joinCode)
    {
        try
        {
            await UnityServices.InitializeAsync();
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }

            var allocation = await RelayService.Instance.JoinAllocationAsync(joinCode: joinCode);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(AllocationUtils.ToRelayServerData(allocation, "dtls"));
            return !string.IsNullOrEmpty(joinCode) && NetworkManager.Singleton.StartClient();

        }
        catch (RelayServiceException e)
        {
            Debug.LogError("Failed to join Relay: " + e.Message);
        }
        return false;
    }
    async void JoinGame()
    {
        string _joinCode = joinCode.text;
        if (await JoinClient(_joinCode))
        {
            Debug.Log("Client started, connecting to server...");
        }
    }
}
