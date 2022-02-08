using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
public class USERLOGIN : MonoBehaviour
{

    private string userEmail;
    private string userPassword;
    private string userName;
    public GameObject logInPanel;

    public void Start()
    {
        //Note: Setting title Id here can be skipped if you have set the value in Editor Extensions already.
        if (string.IsNullOrEmpty(PlayFabSettings.TitleId))
        {
            PlayFabSettings.TitleId = "55AC2"; // Please change this value to your own titleId from PlayFab Game Manager
        }
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
    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Congratulations, you made your first successful API call!");
        PlayerPrefs.SetString("Email", userEmail);
        PlayerPrefs.SetString("Passowrd", userPassword);
        logInPanel.SetActive(false);
    }

    private void onRegisterSuccess(RegisterPlayFabUserResult res)
    {
        Debug.Log("Congratulations, you made your first successful API call!");
        PlayerPrefs.SetString("Email", userEmail);
        PlayerPrefs.SetString("Passowrd", userPassword);
        logInPanel.SetActive(false);
    }
    private void OnLoginFailure(PlayFabError error)
    {
        var registerUser = new RegisterPlayFabUserRequest { Email = userEmail, Password = userPassword, Username = userName };
        PlayFabClientAPI.RegisterPlayFabUser(registerUser, onRegisterSuccess, onRegisterFailure);
    }

    private void onRegisterFailure(PlayFabError err) 
    {

        Debug.LogError(err.GenerateErrorReport());
    }

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

    public void submitLogInInfo()
    {
        var request = new LoginWithEmailAddressRequest { Email = userEmail, Password = userPassword };
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);
    }
}