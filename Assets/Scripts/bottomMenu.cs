using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.UI.Image;
using UnityEngine.SceneManagement;

public class bottomMenu : MonoBehaviour , IPointerDownHandler , IPointerEnterHandler, IPointerExitHandler
{
    public Image selected;

    public enum TipoBottom
    {
        Play,
        Settings,
        Multiplayer,
        Exit
    }

    public TipoBottom estadoBottom;

    void Start()
    {
       selected.enabled = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
       selected.enabled = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        selected.enabled = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        switch (estadoBottom)
        {
            case TipoBottom.Play:
                SceneManager.LoadScene("Gameplay");
                break;
            case TipoBottom.Settings:
                var settingsCanvas = GameObject.Find("SettingsCanvas").GetComponent<Canvas>();
                settingsCanvas.enabled = true;  
                break;
            case TipoBottom.Multiplayer:
                var lobbyCanvas= GameObject.Find("LobbyCanvas").GetComponent<Canvas>();
                lobbyCanvas.enabled = true;
                var lobby = GameObject.Find("LobbyCanvas").GetComponent<Animator>();
                lobby.SetBool("Awake", true);
                var menuCanvas= GameObject.Find("Menu").GetComponent<Canvas>();
                menuCanvas.enabled = false;
                break;
            case TipoBottom.Exit:
                Application.Quit();
                break;
        }

    }
}
