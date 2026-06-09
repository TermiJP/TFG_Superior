using UnityEngine;
using TMPro;

public class MainMenuUI : MonoBehaviour
{
    public RelayLobbyManager manager;

    public TMP_InputField codeInput;

    // BOTÓN HOST
    public async void HostGame()
    {
        string lobbyCode = await manager.CreateLobby();

        Debug.Log("Código lobby: " + lobbyCode);
    }

    // BOTÓN JOIN
    public async void JoinGame()
    {
        await manager.JoinLobby(codeInput.text);
    }

   
}