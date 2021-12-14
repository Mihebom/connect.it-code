using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Mirror;


public class roundSystem : NetworkBehaviour
{

    public TextMeshProUGUI roundDisplay;

    public TextMeshProUGUI timerDisplay;

    public TextMeshProUGUI currentPlayerTurn;

    public TextMeshProUGUI franceFacts;

    public TextMeshProUGUI chinaFacts;

    public TextMeshProUGUI user1Score;

    public TextMeshProUGUI user2Score;

    public TextMeshProUGUI user1FinalScore;

    public TextMeshProUGUI user2FinalScore;

    public TextMeshProUGUI winnerorloser;

    public GameObject franceFactsHolder;

    public GameObject chinaFactsHolder;

    public GameObject winningScreen;

    public GameObject gameOver;

    [SyncVar]public int madeTurnServer = 1;

    [SyncVar]public int madeTurnClient = 1;

    [SyncVar] public int currentRound = 1;

    [SyncVar]public int remainingSeconds = 10;

    private string[] facts = new string[7];

    private string[] chinafacts = new string[7];

    public bool subtractTime = false;

    void Start()
    {
        #region France Facts

        facts[0] = "France is the Most-Visited Country in the World.";

        facts[1] = "France is Smaller Than Texas.";

        facts[2] = "Frances Has the Largest Art Museum.";

        facts[3] = "The French Eat 25,000 Tons of Snails Each Year.";

        facts[4] = "France Produces Over 1,500 Types of Cheese.";

        facts[5] = "Supermarkets in France Can't Throw Away Food.";

        facts[6] = "France Had a King - That Lasted Only 20 Minutes.";

        #endregion

        #region China Facts

        chinafacts[0] = "China is the third largest country in the world.";

        chinafacts[1] = "Toilet Paper was invented in China.";

        chinafacts[2] = "Red Symbolizes happiness in China.";

        chinafacts[3] = "Fortune Cookies are not a Chinese custom.";

        chinafacts[4] = "There is only one time zone in China.";

        chinafacts[5] = "Ping Pong is China's National Sport";

        chinafacts[6] = "Tea was discovered in China";

        #endregion

        timerDisplay.GetComponent<TextMeshProUGUI>().text = "00:" + remainingSeconds;

    }

    #region France Round System

    [Server]
    public void rounds()
    {
        //Server
        if (isServer && isClient && remainingSeconds <= 0 && madeTurnServer == 1)
        {
            GameBoard.Instance.gameObject.SetActive(false);
            currentPlayerTurn.gameObject.SetActive(true);
            franceFactsHolder.gameObject.SetActive(true);
            franceFacts.SetText($"{facts[Random.Range(0, facts.Length)]}");
            currentPlayerTurn.SetText("User 2's Turn !");
            madeTurnServer += 1;
            remainingSeconds = 10;
            StartCoroutine(timeSubtraction());

        }
        else if (isServer && isClient && remainingSeconds <= 0 && madeTurnServer == 2)
        {
            //Round 2
            GameBoard.Instance.gameObject.SetActive(true);
            currentPlayerTurn.gameObject.SetActive(false);
            franceFactsHolder.gameObject.SetActive(false);
            madeTurnServer += 1;
            remainingSeconds = 10;
            roundDisplay.SetText($"Round {currentRound += 1}");
            StartCoroutine(timeSubtraction());

        }
        else if (isServer && isClient && remainingSeconds <= 0 && madeTurnServer == 3)
        {
            GameBoard.Instance.gameObject.SetActive(false);
            currentPlayerTurn.gameObject.SetActive(true);
            franceFactsHolder.gameObject.SetActive(true);
            franceFacts.SetText($"{facts[Random.Range(0, facts.Length)]}");
            currentPlayerTurn.SetText("User 2's Turn !");
            madeTurnServer += 1;
            remainingSeconds = 10;
            StartCoroutine(timeSubtraction());

        }
        else if (isServer && isClient && remainingSeconds <= 0 && madeTurnServer == 4)
        {
            //Round 3
            GameBoard.Instance.gameObject.SetActive(true);
            currentPlayerTurn.gameObject.SetActive(false);
            franceFactsHolder.gameObject.SetActive(false);
            roundDisplay.SetText($"Round {currentRound+=1}");
            madeTurnServer += 1;
            remainingSeconds = 10;
            StartCoroutine(timeSubtraction());

        }
        else if (isServer && isClient && remainingSeconds <= 0 && madeTurnServer == 5)
        {
            GameBoard.Instance.gameObject.SetActive(false);
            currentPlayerTurn.gameObject.SetActive(true);
            franceFactsHolder.gameObject.SetActive(true);
            franceFacts.SetText($"{facts[Random.Range(0, facts.Length)]}");
            currentPlayerTurn.SetText("User 2's Turn !");
            madeTurnServer += 1;
            remainingSeconds = 10;
            StartCoroutine(timeSubtraction());

        }
        else if (isServer && isClient && remainingSeconds <= 0 && madeTurnServer == 6)
        {
            GameBoard.Instance.gameObject.SetActive(false);
            currentPlayerTurn.gameObject.SetActive(false);
            franceFactsHolder.gameObject.SetActive(true);
            madeTurnServer += 1;
            gameOver.gameObject.SetActive(true);
            roundDisplay.SetText("Rounds Finished!");
            remainingSeconds = 5;
            StartCoroutine(timeSubtraction());

        } 
        else if(isServer && isClient && remainingSeconds <= 0 && madeTurnServer == 7)
        {
            GameBoard.Instance.gameObject.SetActive(false);
            franceFactsHolder.gameObject.SetActive(false);
            winningScreen.gameObject.SetActive(true);
            user1Score.gameObject.SetActive(false);
            user2Score.gameObject.SetActive(false);
            gameOver.gameObject.SetActive(false);
            madeTurnServer += 1;
            user1FinalScore.SetText($"User 1's Score  {ScoreSystem.instance._score}");
            user2FinalScore.SetText($"User 2's Score  {ScoreSystem.instance._opponentScore}");
            remainingSeconds = 5;
            StartCoroutine(timeSubtraction());

            if (ScoreSystem.instance.score > ScoreSystem.instance.opponentScore)
            {

                winnerorloser.SetText("You Win!!");

            }
            else
            {
                winnerorloser.SetText("You lose.");

            }

        }
        else if(isServer && isClient && remainingSeconds <= 0 && madeTurnServer == 8)
        {
            SceneManager.LoadScene("demoOver");
        }

        clientRounds();
    }

    [ClientRpc]
    public void clientRounds()
    {
        //Client
        if (isClientOnly && GameBoard.Instance && remainingSeconds >= 0 && madeTurnClient == 1)
        {
            GameBoard.Instance.gameObject.SetActive(false);
            currentPlayerTurn.gameObject.SetActive(true);
            franceFactsHolder.gameObject.SetActive(true);
            franceFacts.SetText($"{facts[Random.Range(0, facts.Length)]}");
            currentPlayerTurn.SetText("User 1's Turn !");
            madeTurnClient += 1;

        }  
        else if (isClientOnly && remainingSeconds <= 0 && madeTurnClient == 2)
        {
            GameBoard.Instance.gameObject.SetActive(true);
            currentPlayerTurn.gameObject.SetActive(false);
            franceFactsHolder.gameObject.SetActive(false);
            madeTurnClient += 1;

        }  
        else if (isClientOnly && remainingSeconds <= 0 && madeTurnClient == 3 && madeTurnServer == 2)
        {
            GameBoard.Instance.gameObject.SetActive(false);
            currentPlayerTurn.gameObject.SetActive(true);
            franceFactsHolder.gameObject.SetActive(true);
            franceFacts.SetText($"{facts[Random.Range(0, facts.Length)]}");
            currentPlayerTurn.SetText("User 1's Turn !");
            roundDisplay.SetText($"Round 2");
            madeTurnClient += 1;

        }
        else if (isClientOnly && remainingSeconds <= 0 && madeTurnClient == 4 && madeTurnServer == 3)
        { 
            GameBoard.Instance.gameObject.SetActive(true);
            currentPlayerTurn.gameObject.SetActive(false);
            franceFactsHolder.gameObject.SetActive(false);
            madeTurnClient += 1;

        }
        else if (isClientOnly && remainingSeconds <= 0 && madeTurnClient == 5 && madeTurnServer == 4)
        {
            GameBoard.Instance.gameObject.SetActive(false);
            currentPlayerTurn.gameObject.SetActive(true);
            franceFactsHolder.gameObject.SetActive(true);
            franceFacts.SetText($"{facts[Random.Range(0, facts.Length)]}");
            currentPlayerTurn.SetText("User 1's Turn !");
            roundDisplay.SetText($"Round 3");
            madeTurnClient += 1;

        }
        else if (isClientOnly && remainingSeconds <= 0 && madeTurnClient == 6 && madeTurnServer == 5)
        {
            GameBoard.Instance.gameObject.SetActive(true);
            currentPlayerTurn.gameObject.SetActive(false);
            franceFactsHolder.gameObject.SetActive(false);
            madeTurnClient += 1; 

        }
        else if (isClientOnly && remainingSeconds <= 0 && madeTurnClient == 7 && madeTurnServer == 6)
        {
            GameBoard.Instance.gameObject.SetActive(false);
            gameOver.gameObject.SetActive(true);
            franceFactsHolder.gameObject.SetActive(true);
            madeTurnClient += 1;
            roundDisplay.SetText("Rounds Finished!");
     
        } else if(isClientOnly && remainingSeconds <= 0 && madeTurnClient == 8 && madeTurnServer == 7)
        {
            GameBoard.Instance.gameObject.SetActive(false);
            franceFactsHolder.gameObject.SetActive(false);
            winningScreen.gameObject.SetActive(true);
            user1Score.gameObject.SetActive(false);
            user2Score.gameObject.SetActive(false);
            gameOver.gameObject.SetActive(false);
            madeTurnClient += 1;
            user1FinalScore.SetText($"User 1's Score  {ScoreSystem.instance._score}");
            user2FinalScore.SetText($"User 2's Score  {ScoreSystem.instance._opponentScore}");

            if(ScoreSystem.instance.opponentScore > ScoreSystem.instance.score)
            {

                winnerorloser.SetText("You Win!!");

            } else
            {
                winnerorloser.SetText("You lose.");

            }

        }
        else if (isClientOnly && remainingSeconds <= 0 && madeTurnClient == 9 && madeTurnServer == 8)
        {
            SceneManager.LoadScene("demoOver");
        }

    }

    #endregion

    #region China Round System

    [Server]
    public void chinaRounds()
    {
        //Server
        if (isServer && isClient && remainingSeconds <= 0 && madeTurnServer == 1)
        {
            ChinaGameBoard.Instance.gameObject.SetActive(false);
            currentPlayerTurn.gameObject.SetActive(true);
            chinaFactsHolder.gameObject.SetActive(true);
            chinaFacts.SetText($"{chinafacts[Random.Range(0, facts.Length)]}");
            currentPlayerTurn.SetText("User 2's Turn !");
            madeTurnServer += 1;
            remainingSeconds = 10;
            StartCoroutine(timeSubtraction());

        }
        else if (isServer && isClient && remainingSeconds <= 0 && madeTurnServer == 2)
        {
            //Round 2
            ChinaGameBoard.Instance.gameObject.SetActive(true);
            currentPlayerTurn.gameObject.SetActive(false);
            chinaFactsHolder.gameObject.SetActive(false);
            madeTurnServer += 1;
            remainingSeconds = 10;
            roundDisplay.SetText($"Round {currentRound += 1}");
            StartCoroutine(timeSubtraction());

        }
        else if (isServer && isClient && remainingSeconds <= 0 && madeTurnServer == 3)
        {
            ChinaGameBoard.Instance.gameObject.SetActive(false);
            currentPlayerTurn.gameObject.SetActive(true);
            chinaFactsHolder.gameObject.SetActive(true);
            chinaFacts.SetText($"{chinafacts[Random.Range(0, facts.Length)]}");
            currentPlayerTurn.SetText("User 2's Turn !");
            madeTurnServer += 1;
            remainingSeconds = 10;
            StartCoroutine(timeSubtraction());

        }
        else if (isServer && isClient && remainingSeconds <= 0 && madeTurnServer == 4)
        {
            //Round 3
            ChinaGameBoard.Instance.gameObject.SetActive(true);
            currentPlayerTurn.gameObject.SetActive(false);
            chinaFactsHolder.gameObject.SetActive(false);
            roundDisplay.SetText($"Round {currentRound += 1}");
            madeTurnServer += 1;
            remainingSeconds = 10;
            StartCoroutine(timeSubtraction());

        }
        else if (isServer && isClient && remainingSeconds <= 0 && madeTurnServer == 5)
        {
            ChinaGameBoard.Instance.gameObject.SetActive(false);
            currentPlayerTurn.gameObject.SetActive(true);
            chinaFactsHolder.gameObject.SetActive(true);
            chinaFacts.SetText($"{chinafacts[Random.Range(0, facts.Length)]}");
            currentPlayerTurn.SetText("User 2's Turn !");
            madeTurnServer += 1;
            remainingSeconds = 10;
            StartCoroutine(timeSubtraction());

        }
        else if (isServer && isClient && remainingSeconds <= 0 && madeTurnServer == 6)
        {
            ChinaGameBoard.Instance.gameObject.SetActive(false);
            currentPlayerTurn.gameObject.SetActive(false);
            gameOver.gameObject.SetActive(true);
            chinaFactsHolder.gameObject.SetActive(true);
            madeTurnServer += 1;
            roundDisplay.SetText("Rounds Finished!");
            remainingSeconds = 5;
            StartCoroutine(timeSubtraction());

        }
        else if (isServer && isClient && remainingSeconds <= 0 && madeTurnServer == 7)
        {
            ChinaGameBoard.Instance.gameObject.SetActive(false);
            chinaFactsHolder.gameObject.SetActive(false);
            winningScreen.gameObject.SetActive(true);
            user1Score.gameObject.SetActive(false);
            user2Score.gameObject.SetActive(false);
            gameOver.gameObject.SetActive(false);
            madeTurnServer += 1;
            remainingSeconds = 5;
            StartCoroutine(timeSubtraction());
            user1FinalScore.SetText($"User 1's Score  {ScoreSystem.instance._score}");
            user2FinalScore.SetText($"User 2's Score  {ScoreSystem.instance._opponentScore}");

            if (ScoreSystem.instance.score > ScoreSystem.instance.opponentScore)
            {

                winnerorloser.SetText("You Win!!");

            }
            else
            {
                winnerorloser.SetText("You lose.");

            }

        }
        else if (isServer && isClient && remainingSeconds <= 0 && madeTurnServer == 8)
        {
            SceneManager.LoadScene("demoOver");
        }

            chinaClientRounds();
    }

    [ClientRpc]
    public void chinaClientRounds()
    {
        //Client
        if (isClientOnly && ChinaGameBoard.Instance && remainingSeconds >= 0 && madeTurnClient == 1)
        {
            ChinaGameBoard.Instance.gameObject.SetActive(false);
            currentPlayerTurn.gameObject.SetActive(true);
            chinaFactsHolder.gameObject.SetActive(true);
            chinaFacts.SetText($"{chinafacts[Random.Range(0, facts.Length)]}");
            currentPlayerTurn.SetText("User 1's Turn !");
            madeTurnClient += 1;

        }
        else if (isClientOnly && remainingSeconds <= 0 && madeTurnClient == 2)
        {
            ChinaGameBoard.Instance.gameObject.SetActive(true);
            currentPlayerTurn.gameObject.SetActive(false);
            chinaFactsHolder.gameObject.SetActive(false);
            madeTurnClient += 1;

        }
        else if (isClientOnly && remainingSeconds <= 0 && madeTurnClient == 3 && madeTurnServer == 2)
        {
            ChinaGameBoard.Instance.gameObject.SetActive(false);
            currentPlayerTurn.gameObject.SetActive(true);
            chinaFactsHolder.gameObject.SetActive(true);
            chinaFacts.SetText($"{chinafacts[Random.Range(0, facts.Length)]}");
            currentPlayerTurn.SetText("User 1's Turn !");
            roundDisplay.SetText($"Round 2");
            madeTurnClient += 1;

        }
        else if (isClientOnly && remainingSeconds <= 0 && madeTurnClient == 4 && madeTurnServer == 3)
        {
            ChinaGameBoard.Instance.gameObject.SetActive(true);
            currentPlayerTurn.gameObject.SetActive(false);
            chinaFactsHolder.gameObject.SetActive(false);
            madeTurnClient += 1;

        }
        else if (isClientOnly && remainingSeconds <= 0 && madeTurnClient == 5 && madeTurnServer == 4)
        {
            ChinaGameBoard.Instance.gameObject.SetActive(false);
            currentPlayerTurn.gameObject.SetActive(true);
            chinaFactsHolder.gameObject.SetActive(true);
            chinaFacts.SetText($"{chinafacts[Random.Range(0, facts.Length)]}");
            currentPlayerTurn.SetText("User 1's Turn !");
            roundDisplay.SetText($"Round 3");
            madeTurnClient += 1;

        }
        else if (isClientOnly && remainingSeconds <= 0 && madeTurnClient == 6 && madeTurnServer == 5)
        {
            ChinaGameBoard.Instance.gameObject.SetActive(true);
            currentPlayerTurn.gameObject.SetActive(false);
            chinaFactsHolder.gameObject.SetActive(false);
            madeTurnClient += 1;

        }
        else if (isClientOnly && remainingSeconds <= 0 && madeTurnClient == 7 && madeTurnServer == 6)
        {
            ChinaGameBoard.Instance.gameObject.SetActive(false);
            gameOver.gameObject.SetActive(true);
            chinaFactsHolder.gameObject.SetActive(true);
            madeTurnClient += 1;
            roundDisplay.SetText("Rounds Finished!");

        }
        else if (isClientOnly && remainingSeconds <= 0 && madeTurnClient == 8 && madeTurnServer == 7)
        {
            ChinaGameBoard.Instance.gameObject.SetActive(false);
            chinaFactsHolder.gameObject.SetActive(false);
            winningScreen.gameObject.SetActive(true);
            user1Score.gameObject.SetActive(false);
            user2Score.gameObject.SetActive(false);
            gameOver.gameObject.SetActive(false);
            madeTurnClient += 1;
            user1FinalScore.SetText($"User 1's Score  {ScoreSystem.instance._score}");
            user2FinalScore.SetText($"User 2's Score  {ScoreSystem.instance._opponentScore}");

            if (ScoreSystem.instance.opponentScore > ScoreSystem.instance.score)
            {

                winnerorloser.SetText("You Win!!");

            }
            else
            {
                winnerorloser.SetText("You lose.");

            }

        }
        else if (isClientOnly && remainingSeconds <= 0 && madeTurnClient == 9 && madeTurnServer == 8)
        {
            SceneManager.LoadScene("demoOver");
        }

    }

    #endregion

    #region Timer System

    IEnumerator timeSubtraction()
    {

        subtractTime = true;

        yield return new WaitForSeconds(1);

        remainingSeconds -= 1;

        if (remainingSeconds < 10)
        {
            timerDisplay.GetComponent<TextMeshProUGUI>().text = "00:0" + remainingSeconds;
        }
        else
        {
            timerDisplay.GetComponent<TextMeshProUGUI>().text = "00:" + remainingSeconds;
        }

        subtractTime = false;

    }

    #endregion

    void Update()
    {

        if (subtractTime == false && remainingSeconds > 0)
        {
            StartCoroutine(timeSubtraction());
        }

        if(GameBoard.Instance && isServer && isClient)
        {
            rounds();
        }

        if (ChinaGameBoard.Instance && isServer && isClient)
        {
            chinaRounds();
        }

 

    }

}









