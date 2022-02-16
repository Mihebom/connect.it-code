using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System;
using UnityEngine.SceneManagement;
using TMPro;


public class PlayFabManager : MonoBehaviour
{

    private string userEmail;
    private string userPassword;
    private string userName;
    private string userCountry;
    public GameObject logInPanel;
    public GameObject selectCountry;
    public Dropdown countryOptions;
    public GameObject leaderPanel;
    public GameObject listPref;
    public Transform listContainer;


    public void Start()
    {

        //Note: Setting title Id here can be skipped if you have set the value in Editor Extensions already.
        if (string.IsNullOrEmpty(PlayFabSettings.TitleId))
        {
            PlayFabSettings.TitleId = "55AC2"; // Please change this value to your own titleId from PlayFab Game Manager
        }
        PlayerPrefs.DeleteAll();
        //var request = new LoginWithCustomIDRequest { CustomId = "GettingStartedGuide", CreateAccount = true };
        //PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
        if (PlayerPrefs.HasKey("Email"))
        {
            userEmail = PlayerPrefs.GetString("Email");
            userPassword = PlayerPrefs.GetString("Passowrd");
            var request = new LoginWithEmailAddressRequest { Email = userEmail, Password = userPassword };
            PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);
        }


     
    }


    #region Login/Register

    //Logs user into the database
    private void OnLoginSuccess(LoginResult result)
    {
      
        Debug.Log("Login Successful!");
        PlayerPrefs.SetString("Email", userEmail);
        PlayerPrefs.SetString("Passowrd", userPassword);
        logInPanel.SetActive(false);
        SceneManager.LoadScene("MainMenu");
       
   
    }

    //Registers user info into the database
    private void onRegisterSuccess(RegisterPlayFabUserResult res)
    {
        Debug.Log("Register Successful!");
        PlayerPrefs.SetString("Email", userEmail);
        PlayerPrefs.SetString("Passowrd", userPassword);
        logInPanel.SetActive(false);
        selectCountry.SetActive(true);
    }
    private void OnLoginFailure(PlayFabError error)
    {
        var registerUser = new RegisterPlayFabUserRequest { Email = userEmail, Password = userPassword, Username = userName, DisplayName = userName };
        PlayFabClientAPI.RegisterPlayFabUser(registerUser, onRegisterSuccess, onRegisterFailure);
    }

    private void onRegisterFailure(PlayFabError err)
    { 
        Debug.LogError(err.GenerateErrorReport());
    }

    #endregion

    #region User Info

    //Receives user input and sets them to variables that the PlayFabClientAPI will send and store in the database or retrieve data from the database

    public void getUserEmail(string emailRecieved)
    {
        userEmail = emailRecieved;
    }

    public void getUserPassword(string passwordRecieved)
    {
        userPassword = passwordRecieved;
    }

    public void getUserName(string usernameRecieved)
    {
        userName = usernameRecieved;
    }

    public void getUserCountry(int dropDownVal)
    {
        userCountry = countryOptions.options[dropDownVal].text;
    }

    public void submitLogInInfo()
    {
        var request = new LoginWithEmailAddressRequest { Email = userEmail, Password = userPassword };
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);
    }

    #endregion

    #region Country Selection

    //Sets users country based on their input from the dropdown menu
    public void setCountry()
    {
        var request = new UpdateUserDataRequest
        {

            Data = new System.Collections.Generic.Dictionary<string, string>
            {
                {"Country", userCountry}
            }

        };

        PlayFabClientAPI.UpdateUserData(request, onSetCountrySuccess, onSetCountryFailure);
        SceneManager.LoadScene("MainMenu");
    }

    private void onSetCountrySuccess(UpdateUserDataResult obj)
    {
        Debug.Log("Country has been set!");
    }

    private void onSetCountryFailure(PlayFabError err)
    {

        Debug.LogError(err.GenerateErrorReport());
    }

    #endregion

    #region Leaderboard

    //Submitting score to leaderboard
    public void SubmitScore(String country, int playerScore)
    {
        PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName = country,
                    Value = playerScore
                }
            }
        }, result => OnStatisticsUpdate(result), FailureCallback);

    }

    //Checking if value submiited
    private void OnStatisticsUpdate(UpdatePlayerStatisticsResult updateResult)
    {
        Debug.Log("Successfully submitted high score");
    }

    //Checking if value submitted
    private void FailureCallback(PlayFabError error)
    {
        Debug.LogWarning("Something went wrong with your API call. Here's some debug information: ");
        Debug.LogError(error.GenerateErrorReport());
    }

    public void GetLeaderboard()
    {
        var requestLeaderboard = new GetLeaderboardRequest { StartPosition = 0, StatisticName = "France", MaxResultsCount = 10 };
        PlayFabClientAPI.GetLeaderboard(requestLeaderboard, onGetLeaderboard, FailureCallback);
    }


    void onGetLeaderboard(GetLeaderboardResult result)
    {
       

        foreach (PlayerLeaderboardEntry player in result.Leaderboard)
        {
            GameObject tempList = Instantiate(listPref, listContainer);
            leaderboardInfo li = tempList.GetComponent<leaderboardInfo>();
            li.displaynameTxt.text = player.DisplayName;
            li.scoreTxt.text = player.StatValue.ToString();
        }


    }


    #endregion

}