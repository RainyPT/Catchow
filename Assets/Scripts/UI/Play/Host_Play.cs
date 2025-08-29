using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using Unity.Networking;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;

public class Host_Play : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(HostGame);
    }
    // https://docs.unity.com/ugs/en-us/manual/relay/manual/relay-and-ngo#host_player
    public async Task<string> CreateHost()
    {
        try
        {
            await UnityServices.InitializeAsync();
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
            var allocation = await RelayService.Instance.CreateAllocationAsync(1);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(AllocationUtils.ToRelayServerData(allocation, "dtls"));
            var joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);


            return NetworkManager.Singleton.StartHost() ? joinCode : null;
        }
        catch (RelayServiceException e)
        {
            Debug.LogError("Failed to Host with Relay: " + e.Message);
        }
        return null;
    }


    // Update is called once per frame
    async void HostGame()
    {
        string joinCode = await CreateHost();
        if (joinCode != null)
        {
            JoinCodeInfo.joinCode = joinCode;
            Debug.Log("Host started.");

            NetworkManager.Singleton.SceneManager.LoadScene("DevScene", LoadSceneMode.Single);
        }
    }
}
