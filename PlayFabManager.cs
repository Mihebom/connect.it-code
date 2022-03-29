using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System;

using UnityEngine.SceneManagement;
using TMPro;
using System.Text.RegularExpressions;

public class PlayFabManager : MonoBehaviour
{

    public static PlayFabManager Instance { get; private set; }
    private string userEmail;
    private string userPassword;
    private string userName;
    private string userCountry;
    private string leaderCountry;
    public string playFABID;
    public string playEntity;
    public int awardAmount;
    private int goldBars;
    public GameObject logInPanel;
    public GameObject selectCountry;
    public Dropdown countryOptions;
    public Dropdown leaderBoardCountryOptions;
    public GameObject leaderPanel;
    public GameObject listPref;
    public GameObject itemListPref;
    public GameObject shopListPref;
    public GameObject messageHolder;
    public Transform listContainer;
    public TextMeshProUGUI displayName;
    public TextMeshProUGUI errorPopUp;
    public TextMeshProUGUI inGameDisplayName;
    public TextMeshProUGUI gemsAmount;
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemLevel;
    public TextMeshProUGUI itemDesc;
    public TextMeshProUGUI itemPrice;
    public TextMeshProUGUI inGameCountryDisplay;
    public TextMeshProUGUI popUpMessage;
    public TextMeshProUGUI awardText;
    public Text backEndBarValue;
    public GameObject itemHolder;
    public Button test;
    public Image itemInfoIMG;
    public Sprite[] itemIMG;

    //For PlayFab matchmaking 
    public static string SessionTicket;
    public static string EntityId;

    //Hidden from user, used only in database
    public Text itemID;
    public Text itemCost;
    public List<GameObject> tempLeaderboardList = new List<GameObject>();
    public List<string> ownedItems = new List<string>();
    public List<string> ownedItemsCopy = new List<string>();
    public void Start()
    {
        Instance = this;
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

        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(), OnGetUsernameResult, OnLoginFailure);
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), onGetUserInventorySuccess, FailureCallback);
        var requestLeaderboard = new GetLeaderboardRequest { StartPosition = 0, StatisticName = userCountry, MaxResultsCount = 10 };
        PlayFabClientAPI.GetLeaderboard(requestLeaderboard, onGetLeaderboard, FailureCallback);
        getItem();
        getItemAmount();
        getCatalog();
        getUserData();
        ownedItemCheck();
    }

    #region Login/Register

    //Logs user into the database

    string Encrypt(string pass)
    {
        System.Security.Cryptography.MD5CryptoServiceProvider x = new System.Security.Cryptography.MD5CryptoServiceProvider();
        byte[] bs = System.Text.Encoding.UTF8.GetBytes(pass);
        bs = x.ComputeHash(bs);
        System.Text.StringBuilder s = new System.Text.StringBuilder();
        foreach (byte b in bs)
        {
            s.Append(b.ToString("x2").ToLower());
        }
        return s.ToString();
    }
    private void OnLoginSuccess(LoginResult result)
    {

        Debug.Log("Login Successful!");
        PlayerPrefs.SetString("Email", userEmail);

        PlayerPrefs.SetString("Passowrd", userPassword);
        logInPanel.SetActive(false);
        SceneManager.LoadScene("MainMenu");

        //playfab matchmaking
        SessionTicket = result.SessionTicket; 
        EntityId = result.EntityToken.Entity.Id;


    }

    //Registers user info into the database
    private void onRegisterSuccess(RegisterPlayFabUserResult res)
    {
        Debug.Log("Register Successful!");
        PlayerPrefs.SetString("Email", userEmail);
        PlayerPrefs.SetString("Passowrd", userPassword);
        logInPanel.SetActive(false);
        selectCountry.SetActive(true);

        //playfab matchmaking
        SessionTicket = res.SessionTicket; 
        EntityId = res.EntityToken.Entity.Id;
    }
    private void OnLoginFailure(PlayFabError error)
    {
        var registerUser = new RegisterPlayFabUserRequest { Email = userEmail, Password = Encrypt(userPassword), Username = userName, DisplayName = userName };
        PlayFabClientAPI.RegisterPlayFabUser(registerUser, onRegisterSuccess, onRegisterFailure);
    }

    private void onRegisterFailure(PlayFabError err)
    {
        errorPopUp.SetText(err.ErrorMessage);
        errorPopUp.color = Color.red;
    }

    public void logOut()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene("UserLogin");
    }

    #endregion

    #region User Info

    //Displays users name

    public void OnGetUsernameResult(GetAccountInfoResult result)
    {
        
        displayName.SetText("Welcome " + result.AccountInfo.Username + " !");
    }
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

    public void getLeaderdBoardCountry(int dropDownVal)
    {
        leaderCountry = leaderBoardCountryOptions.options[dropDownVal].text;
    }

    public void submitLogInInfo()
    {
        var request = new LoginWithEmailAddressRequest { Email = userEmail, Password = Encrypt(userPassword) };
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

    //Gets unique playfabID for each user
    public void onLogin(LoginResult playfabID)
    {
        playFABID = playfabID.PlayFabId;
    }

    //returns custom user data
    public void getUserData()
    {

        GetUserDataRequest request = new GetUserDataRequest() {

            PlayFabId = playFABID,
            Keys = null
        };

        PlayFabClientAPI.GetUserData(request, 
            
            
        
        result => {
          
           foreach(var data in result.Data)
            {
              inGameCountryDisplay.SetText(data.Value.Value);
                
            }
        
        
        
        }, error => {


            Debug.LogError(error.ErrorMessage);
        
        });

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


    public void changeLeaderBoardCountry()
    {

     var requestLeaderboard = new GetLeaderboardRequest { StartPosition = 0, StatisticName = leaderCountry, MaxResultsCount = 10 };
     PlayFabClientAPI.GetLeaderboard(requestLeaderboard, onGetLeaderboard, FailureCallback);
      
    }

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

    void onGetLeaderboard(GetLeaderboardResult result)
    {

        foreach (PlayerLeaderboardEntry player in result.Leaderboard)
        {
                GameObject tempList = Instantiate(listPref, listContainer);
                LeaderboardInfo li = tempList.GetComponent<LeaderboardInfo>();
                li.displaynameTxt.text = "UserName: " + player.DisplayName;
                li.scoreTxt.text = "Score: " + player.StatValue.ToString();
                tempLeaderboardList.Add(tempList);

        }

    }

    //Deletes objects in the leaderboard once a user changes the dropdown menu value
    public void deleteOnValueChange()
    {
        foreach (GameObject g in tempLeaderboardList)
        {
            Destroy(g);
        }
    }

    #endregion


    //Written by Matthew Ihebom
    #region Virtual Currencies

    public TextMeshProUGUI goldBarsValueText; //variable to display the text of gold bars on UI

    void onGetUserInventorySuccess(GetUserInventoryResult result)
    {
        goldBars = result.VirtualCurrency["GB"];
        goldBarsValueText.SetText($"Gold Bars: {goldBars}");
        backEndBarValue.text = goldBars.ToString();
    }

    #endregion

    #region Inventory/Store 
    public void getItem()
    {
        GetUserInventoryRequest request = new GetUserInventoryRequest();
        GetCatalogItemsRequest req = new GetCatalogItemsRequest();
        PlayFabClientAPI.GetCatalogItems(req, res =>
        {
            //List of all items in the database catalog
            List<CatalogItem> desc = res.Catalog;

            PlayFabClientAPI.GetUserInventory(request, result =>
            {

                //Saves all items in the user inventory into a list
                List<ItemInstance> inv = result.Inventory;

                //Iterates through each item in the inventory
                foreach (ItemInstance i in inv)
                {

                   
                    //Creates an item display box for each item in the array
                    GameObject temp = Instantiate(itemListPref, listContainer);
                    ItemInfo ii = temp.GetComponent<ItemInfo>();
                    ii.itemdisplayName.SetText(i.DisplayName);
                    ii.button.onClick.AddListener(() => itemHolder.SetActive(true));
                    
                    //Iterates through each image in the itemIMG array
                    foreach (Sprite im in itemIMG)
                    {
                        //Iterates through each item in the database catalog 
                        foreach (CatalogItem id in desc)
                        {
                            //Ensures that items are ensured suitable items based on their display name 
                            if (im.name == "clearquartz" && i.DisplayName == "Clear Quartz" && id.DisplayName == "Clear Quartz")
                            {
                                //Sets the appropriate image to the items display box
                                ii.itemIMG.sprite = im;
                                //Changes values specific to the item when its button is clicked
                                ii.button.onClick.AddListener(() => itemName.SetText("Clear Quartz"));
                                ii.button.onClick.AddListener(() => itemLevel.SetText(i.ItemClass));
                                ii.button.onClick.AddListener(() => itemInfoIMG.sprite = im);
                                ii.button.onClick.AddListener(() => itemDesc.SetText(id.Description));
                            }
                            else if (im.name == "amethyst" && i.DisplayName == "Amethyst" && id.DisplayName == "Amethyst")
                            {
                                ii.itemIMG.sprite = im;
                                ii.button.onClick.AddListener(() => itemName.SetText("Amethyst"));
                                ii.button.onClick.AddListener(() => itemLevel.SetText(i.ItemClass));
                                ii.button.onClick.AddListener(() => itemInfoIMG.sprite = im);
                                ii.button.onClick.AddListener(() => itemDesc.SetText(id.Description));
                            }
                            else if (im.name == "carnelian" && i.DisplayName == "Carnelian" && id.DisplayName == "Carnelian")
                            {
                                ii.itemIMG.sprite = im;
                                ii.button.onClick.AddListener(() => itemName.SetText("Carnelian"));
                                ii.button.onClick.AddListener(() => itemLevel.SetText(i.ItemClass));
                                ii.button.onClick.AddListener(() => itemInfoIMG.sprite = im);
                                ii.button.onClick.AddListener(() => itemDesc.SetText(id.Description));
                            }
                            else if (im.name == "citrine" && i.DisplayName == "Citrine" && id.DisplayName == "Citrine")
                            {
                                ii.itemIMG.sprite = im;
                                ii.button.onClick.AddListener(() => itemName.SetText("Citrine"));
                                ii.button.onClick.AddListener(() => itemLevel.SetText(i.ItemClass));
                                ii.button.onClick.AddListener(() => itemInfoIMG.sprite = im);
                                ii.button.onClick.AddListener(() => itemDesc.SetText(id.Description));
                            }
                            else if (im.name == "lapislazuli" && i.DisplayName == "Lapis Lazuli" && id.DisplayName == "Lapis Lazuli")
                            {
                                ii.itemIMG.sprite = im;
                                ii.button.onClick.AddListener(() => itemName.SetText("Lapis Lazuli"));
                                ii.button.onClick.AddListener(() => itemLevel.SetText(i.ItemClass));
                                ii.button.onClick.AddListener(() => itemInfoIMG.sprite = im);
                                ii.button.onClick.AddListener(() => itemDesc.SetText(id.Description));
                            }
                            else if (im.name == "obsidian" && i.DisplayName == "Obsidian" && id.DisplayName == "Obsidian")
                            {
                                ii.itemIMG.sprite = im;
                                ii.button.onClick.AddListener(() => itemName.SetText("Obsidian"));
                                ii.button.onClick.AddListener(() => itemLevel.SetText(i.ItemClass));
                                ii.button.onClick.AddListener(() => itemInfoIMG.sprite = im);
                                ii.button.onClick.AddListener(() => itemDesc.SetText(id.Description));
                            }
                            else if (im.name == "rosequartz" && i.DisplayName == "Rose Quartz" && id.DisplayName == "Rose Quartz")
                            {
                                ii.itemIMG.sprite = im;
                                ii.button.onClick.AddListener(() => itemName.SetText("Rose Quartz"));
                                ii.button.onClick.AddListener(() => itemLevel.SetText(i.ItemClass));
                                ii.button.onClick.AddListener(() => itemInfoIMG.sprite = im);
                                ii.button.onClick.AddListener(() => itemDesc.SetText(id.Description));
                            }
                            else if (im.name == "selenite" && i.DisplayName == "Selenite" && id.DisplayName == "Selenite")
                            {
                                ii.itemIMG.sprite = im;
                                ii.button.onClick.AddListener(() => itemName.SetText("Selenite"));
                                ii.button.onClick.AddListener(() => itemLevel.SetText(i.ItemClass));
                                ii.button.onClick.AddListener(() => itemInfoIMG.sprite = im);
                                ii.button.onClick.AddListener(() => itemDesc.SetText(id.Description));
                            }
                            else if (im.name == "tourmaline" && i.DisplayName == "Tourmaline" && id.DisplayName == "Tourmaline")
                            {
                                ii.itemIMG.sprite = im;
                                ii.button.onClick.AddListener(() => itemName.SetText("Tourmaline"));
                                ii.button.onClick.AddListener(() => itemLevel.SetText(i.ItemClass));
                                ii.button.onClick.AddListener(() => itemInfoIMG.sprite = im);
                                ii.button.onClick.AddListener(() => itemDesc.SetText(id.Description));
                            }
                            else if (im.name == "agate" && i.DisplayName == "Agate" && id.DisplayName == "Agate")
                            {
                                ii.itemIMG.sprite = im;
                                ii.button.onClick.AddListener(() => itemName.SetText("Agate"));
                                ii.button.onClick.AddListener(() => itemLevel.SetText(i.ItemClass));
                                ii.button.onClick.AddListener(() => itemInfoIMG.sprite = im);
                                ii.button.onClick.AddListener(() => itemDesc.SetText(id.Description));
                            }
                        }
                    }

                }

            }, error =>
            {

                Debug.Log("Err");

            });
        }, error => { });

    }

    public void getItemAmount()
    {
        GetUserInventoryRequest request = new GetUserInventoryRequest();
        PlayFabClientAPI.GetUserInventory(request, result => {



            List<ItemInstance> inv = result.Inventory;
            gemsAmount.SetText($"Gems: {inv.Count}");

        }, error => {

            Debug.Log("Err");

        });
    }

    public void getCatalog()
    {

        GetCatalogItemsRequest request = new GetCatalogItemsRequest();
        request.CatalogVersion = "Vault";
        PlayFabClientAPI.GetCatalogItems(request,

        result => {


        List<CatalogItem> catalog = result.Catalog;

        foreach (CatalogItem c in catalog)
        {

            GameObject temp = Instantiate(shopListPref, listContainer);
            PurchasableInfo pi = temp.GetComponent<PurchasableInfo>();
            pi.itemdisplayName.SetText(c.DisplayName);
            //returns the cost of each item in the list
            uint cost = c.VirtualCurrencyPrices["GB"];
    

                pi.button.onClick.AddListener(() => itemHolder.gameObject.SetActive(true));
                pi.button.onClick.AddListener(() => messageHolder.SetActive(false));
                foreach (Sprite im in itemIMG)
                {
                    //Iterates through each item in the database catalog 

                    //Ensures that items are ensured suitable items based on their display name 
                    if (im.name == "clearquartz" && c.DisplayName == "Clear Quartz")
                    {
                        //Sets the appropriate image to the items display box
                        pi.itemIMG.sprite = im;
                        //Changes values specific to the item when its button is clicked
                        pi.button.onClick.AddListener(() => itemName.SetText("Clear Quartz"));
                        pi.button.onClick.AddListener(() => itemLevel.SetText(c.ItemClass));
                        pi.button.onClick.AddListener(() => itemInfoIMG.sprite = im);
                        pi.button.onClick.AddListener(() => itemID.text = c.ItemId);
                        pi.button.onClick.AddListener(() => itemPrice.SetText($"Cost: {cost}"));
                        pi.button.onClick.AddListener(() => itemCost.text = cost.ToString());
                        pi.button.onClick.AddListener(() => itemDesc.SetText(c.Description));


                    }
                    else if (im.name == "amethyst" && c.DisplayName == "Amethyst")
                    {
                        pi.itemIMG.sprite = im;
                        pi.button.onClick.AddListener(() => itemName.SetText("Amethyst"));
                        pi.button.onClick.AddListener(() => itemLevel.SetText(c.ItemClass));
                        pi.button.onClick.AddListener(() => itemInfoIMG.sprite = im);
                        pi.button.onClick.AddListener(() => itemID.text = c.ItemId);
                        pi.button.onClick.AddListener(() => itemPrice.SetText($"Cost: {cost}"));
                        pi.button.onClick.AddListener(() => itemCost.text = cost.ToString());
                        pi.button.onClick.AddListener(() => itemDesc.SetText(c.Description));
                    }
                    else if (im.name == "carnelian" && c.DisplayName == "Carnelian")
                    {
                        pi.itemIMG.sprite = im;
                        pi.button.onClick.AddListener(() => itemName.SetText("Carnelian"));
                        pi.button.onClick.AddListener(() => itemLevel.SetText(c.ItemClass));
                        pi.button.onClick.AddListener(() => itemInfoIMG.sprite = im);
                        pi.button.onClick.AddListener(() => itemID.text = c.ItemId);
                        pi.button.onClick.AddListener(() => itemPrice.SetText($"Cost: {cost}"));
                        pi.button.onClick.AddListener(() => itemCost.text = cost.ToString());
                        pi.button.onClick.AddListener(() => itemDesc.SetText(c.Description));
                    }
                    else if (im.name == "citrine" && c.DisplayName == "Citrine")
                    {
                        pi.itemIMG.sprite = im;
                        pi.button.onClick.AddListener(() => itemName.SetText("Citrine"));
                        pi.button.onClick.AddListener(() => itemLevel.SetText(c.ItemClass));
                        pi.button.onClick.AddListener(() => itemInfoIMG.sprite = im);
                        pi.button.onClick.AddListener(() => itemID.text = c.ItemId);
                        pi.button.onClick.AddListener(() => itemPrice.SetText($"Cost: {cost}"));
                        pi.button.onClick.AddListener(() => itemCost.text = cost.ToString());
                        pi.button.onClick.AddListener(() => itemDesc.SetText(c.Description));
                    }
                    else if (im.name == "lapislazuli" && c.DisplayName == "Lapis Lazuli")
                    {
                        pi.itemIMG.sprite = im;
                        pi.button.onClick.AddListener(() => itemName.SetText("Lapis Lazuli"));
                        pi.button.onClick.AddListener(() => itemLevel.SetText(c.ItemClass));
                        pi.button.onClick.AddListener(() => itemInfoIMG.sprite = im);
                        pi.button.onClick.AddListener(() => itemID.text = c.ItemId);
                        pi.button.onClick.AddListener(() => itemPrice.SetText($"Cost: {cost}"));
                        pi.button.onClick.AddListener(() => itemCost.text = cost.ToString());
                        pi.button.onClick.AddListener(() => itemDesc.SetText(c.Description));
                    }
                    else if (im.name == "obsidian" && c.DisplayName == "Obsidian")
                    {
                        pi.itemIMG.sprite = im;
                        pi.button.onClick.AddListener(() => itemName.SetText("Obsidian"));
                        pi.button.onClick.AddListener(() => itemLevel.SetText(c.ItemClass));
                        pi.button.onClick.AddListener(() => itemInfoIMG.sprite = im);
                        pi.button.onClick.AddListener(() => itemID.text = c.ItemId);
                        pi.button.onClick.AddListener(() => itemPrice.SetText($"Cost: {cost}"));
                        pi.button.onClick.AddListener(() => itemCost.text = cost.ToString());
                        pi.button.onClick.AddListener(() => itemDesc.SetText(c.Description));
                    }
                    else if (im.name == "rosequartz" && c.DisplayName == "Rose Quartz")
                    {
                        pi.itemIMG.sprite = im;
                        pi.button.onClick.AddListener(() => itemName.SetText("Rose Quartz"));
                        pi.button.onClick.AddListener(() => itemLevel.SetText(c.ItemClass));
                        pi.button.onClick.AddListener(() => itemInfoIMG.sprite = im);
                        pi.button.onClick.AddListener(() => itemID.text = c.ItemId);
                        pi.button.onClick.AddListener(() => itemPrice.SetText($"Cost: {cost}"));
                        pi.button.onClick.AddListener(() => itemCost.text = cost.ToString());
                        pi.button.onClick.AddListener(() => itemDesc.SetText(c.Description));
                    }
                    else if (im.name == "selenite" && c.DisplayName == "Selenite")
                    {
                        pi.itemIMG.sprite = im;
                        pi.button.onClick.AddListener(() => itemName.SetText("Selenite"));
                        pi.button.onClick.AddListener(() => itemLevel.SetText(c.ItemClass));
                        pi.button.onClick.AddListener(() => itemInfoIMG.sprite = im);
                        pi.button.onClick.AddListener(() => itemID.text = c.ItemId);
                        pi.button.onClick.AddListener(() => itemPrice.SetText($"Cost: {cost}"));
                        pi.button.onClick.AddListener(() => itemCost.text = cost.ToString());
                        pi.button.onClick.AddListener(() => itemDesc.SetText(c.Description));
                    }
                    else if (im.name == "tourmaline" && c.DisplayName == "Tourmaline")
                    {
                        pi.itemIMG.sprite = im;
                        pi.button.onClick.AddListener(() => itemName.SetText("Tourmaline"));
                        pi.button.onClick.AddListener(() => itemLevel.SetText(c.ItemClass));
                        pi.button.onClick.AddListener(() => itemInfoIMG.sprite = im);
                        pi.button.onClick.AddListener(() => itemID.text = c.ItemId);
                        pi.button.onClick.AddListener(() => itemPrice.SetText($"Cost: {cost}"));
                        pi.button.onClick.AddListener(() => itemCost.text = cost.ToString());
                        pi.button.onClick.AddListener(() => itemDesc.SetText(c.Description));
                    }
                    else if (im.name == "agate" && c.DisplayName == "Agate")
                    {
                        pi.itemIMG.sprite = im;
                        pi.button.onClick.AddListener(() => itemName.SetText("Agate"));
                        pi.button.onClick.AddListener(() => itemLevel.SetText(c.ItemClass));
                        pi.button.onClick.AddListener(() => itemInfoIMG.sprite = im);
                        pi.button.onClick.AddListener(() => itemID.text = c.ItemId);
                        pi.button.onClick.AddListener(() => itemPrice.SetText($"Cost: {cost}"));
                        pi.button.onClick.AddListener(() => itemCost.text = cost.ToString());
                        pi.button.onClick.AddListener(() => itemDesc.SetText(c.Description));
                    }

                }
        }
           
        },
            
        err => {

            Debug.Log("Err");
       
        });

    }

    public void ownedItemCheck()
    {
        GetUserInventoryRequest request = new GetUserInventoryRequest();
        PlayFabClientAPI.GetUserInventory(request, result => {

            List<ItemInstance> tmp = result.Inventory;

            foreach(ItemInstance i in tmp)
            {

                if (i.DisplayName.Contains(itemName.text))
                {
                    ownedItems.Add(i.DisplayName);
                }
                    
               
            }
        
        }, error => { });
       
    }

    public void updateOnPurchase()
    {
        GetUserInventoryRequest request = new GetUserInventoryRequest();
        PlayFabClientAPI.GetUserInventory(request, result => {

            List<ItemInstance> tmp = result.Inventory;

            foreach (ItemInstance i in tmp)
            {

                if (!ownedItems.Contains(i.DisplayName))
                {
                    ownedItems.Add(i.DisplayName);
                }


            }

        }, error => { });

    }

    public void purchaseCheck()
    {

        if (ownedItems.Contains(itemName.text))
        {
            messageHolder.SetActive(true);
            popUpMessage.SetText("Item Already Owned!");
            popUpMessage.color = Color.yellow;
        }
        else
        {
            purchaseItem();
        }
        
    }

    public void purchaseItem()
    {

        if (Int32.Parse(backEndBarValue.text) < Int32.Parse(itemCost.text))
            {
                messageHolder.SetActive(true);
                popUpMessage.SetText("Insufficient Funds!");
                popUpMessage.color = Color.red;
        }
        else
        {
                messageHolder.SetActive(true);
                PurchaseItemRequest request = new PurchaseItemRequest();
                request.CatalogVersion = "Vault";
                request.ItemId = itemID.text;
                request.VirtualCurrency = "GB";
                request.Price = Int32.Parse(itemCost.text);
                PlayFabClientAPI.PurchaseItem(request, result => { }, error => { Debug.LogError(error.ErrorMessage); });
                popUpMessage.SetText("Purchased!");
                popUpMessage.color = Color.green;
            if (goldBars > 0)
            {
                goldBarsValueText.SetText($"Gold Bars: {goldBars -= Int32.Parse(itemCost.text)}");
            }
            else
            {
                goldBarsValueText.SetText($"Gold Bars: 0");
            }
        } 


    }
    void GetInventory()
    {
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), onGetUserInventorySuccess, FailureCallback);
    }

    #endregion

    #region GoldBar Award System

    public void awardPlayer()
    {


        for(int i = 100; i <= ScoreSystem.instance._score; i += 100)
        {
            if (ScoreSystem.instance._score > i)
            {
                
                awardAmount += 10;
                
            }
        }
      

        for (int i = 100; i <= ScoreSystem.instance._opponentScore; i += 100)
        {
            if (ScoreSystem.instance._opponentScore > i)
            {
                
                awardAmount += 10;
                
            }
        }


        AddUserVirtualCurrencyRequest request = new AddUserVirtualCurrencyRequest()
        {
            Amount = awardAmount,
            VirtualCurrency = "GB",
           
            
        };

        PlayFabClientAPI.AddUserVirtualCurrency(request, 
           
            
        
        result => {

            awardText.SetText(awardAmount.ToString());
            Debug.Log(awardAmount.ToString());
        }, 
        
        error => {

            Debug.LogError(error.ErrorMessage);
        
        });

       
    }


    #endregion
}
