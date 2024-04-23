using UnityEngine;
using UnityEngine.SceneManagement;
using static PlayerPrefsManager;

public class MenuManager : MonoBehaviour
{
    public MenuOptions menuOptions;

    void Start()
    {
        FirstGameStart();

        menuOptions.StartCardAnimation();

        HandleSaves();
    }

    void HandleSaves()
    {
        PlayerData playerData = SaveManager.LoadPlayerData();

        if (playerData == null || playerData.CurrentRun == null)
        {
            menuOptions.CoverCard(MenuOptions.CardPosition.Left);
            return;
        }

        HandleCards(playerData);
    }

    void HandleCards(PlayerData data)
    {
        if (GetPref<int>(PlayerPrefsEnum.HasWonAnyRun) != 1)
            menuOptions.CoverCard(MenuOptions.CardPosition.Right);

        if (!data.CurrentRun.IsOngoing)
            menuOptions.CoverCard(MenuOptions.CardPosition.Left);
        else
            menuOptions.LoadRunInfo(data);
    }

    void FirstGameStart()
    {
        if (!DoesPrefExists(PlayerPrefsEnum.AlreadyLaunchedGame))
        {
            SetPref(PlayerPrefsEnum.HasWonAnyRun, 0);
            SetPref(PlayerPrefsEnum.AlreadyLaunchedGame, 1);
            SaveManager.SavePlayerData(new()
            {
                CurrentRun = new()
                {
                    IsOngoing = false
                }
            });
        }
    }

    public void StartNewGameButtonClick()
    {
        SceneManager.LoadScene("Game");
    }
}