using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Mirror;


public class RoundSystem : NetworkBehaviour
{

    public TextMeshProUGUI roundDisplay;

    public TextMeshProUGUI timerDisplay;

    public TextMeshProUGUI currentPlayerTurn;

    public TextMeshProUGUI factsText;

    public TextMeshProUGUI user1Score;

    public TextMeshProUGUI user2Score;

    public TextMeshProUGUI user1FinalScore;

    public TextMeshProUGUI user2FinalScore;

    public TextMeshProUGUI winnerorloser;

    public GameObject factsHolder;

    public GameObject winningScreen;

    public GameObject gameOver;

    [SyncVar] public int madeTurnServer = 1;

    [SyncVar] public int madeTurnClient = 1;

    [SyncVar] public int currentRound = 1;

    [SyncVar] public int remainingSeconds = 10;

    private string[] facts = new string[7];

    public bool subtractTime = false;

    public int winningScore; //final score

    //Instance of PlayFabManager class
    PlayFabManager instance = new();

    void Start()
    {
        #region France Facts
        if (this.gameObject.scene.name == "FranceBoard")
        {

            facts[0] = "France is the most-visited country in the world.";

            facts[1] = "France is smaller than texas.";

            facts[2] = "France has the largest art museum.";

            facts[3] = "The French eat 25,000 tons of snails each year.";

            facts[4] = "France produces over 1,500 types of cheese.";

            facts[5] = "Supermarkets in France can't throw away food.";

            facts[6] = "France had a King - That lasted only 20 minutes.";
        }
        #endregion

        #region China Facts
        if (this.gameObject.scene.name == "ChinaBoard")
        {
            facts[0] = "China is the third largest country in the world.";

            facts[1] = "Toilet paper was invented in China.";

            facts[2] = "Red symbolizes happiness in China.";

            facts[3] = "Fortune cookies are not a Chinese custom.";

            facts[4] = "There is only one time zone in China.";

            facts[5] = "Ping Pong is China's National Sport.";

            facts[6] = "Tea was discovered in China.";
        }

        #endregion

        #region Nigeria Facts
        if (this.gameObject.scene.name == "NigeriaBoard")
        {
            facts[0] = "Nigeria is a member of the British Commonwealth.";

            facts[1] = "The name Nigeria is derived from Niger, which is the longest river in West Africa.";

            facts[2] = "About 75% of the total population uses social media on a regular basis.";

            facts[3] = "Lagos is the largest city, but it’s not the capital.";

            facts[4] = "The movie industry is known as Nollywood.";

            facts[5] = "Oil is one of the biggest exports of the country.";

            facts[6] = "There are more than 250 Ethnic Groups in Nigeria.";
        }
        #endregion


        #region Pakistan Facts
        if (this.gameObject.scene.name == "PakistanBoard")
        {
            facts[0] = "Pakistan is the world's first Islamic country to attain nuclear power.";

            facts[1] = "Sylvester Stallone’s Rambo III was shot in Pakistan.";

            facts[2] = "Lassi and black tea with milk and sugar are the most common drinks.";

            facts[3] = "Pakistan is the world’s fifth most populated country - after China, India, the USA and Indonesia.";

            facts[4] = "Pakistan has the only fertile desert in the world – the Tharparkar desert – located in Sindh province.";

            facts[5] = "Around 40% - almost half! - of the world’s footballs are hand-sewn in Sialkot in Pakistan.";

            facts[6] = "In Pakistan, cars drive on the left side of the road as they do in the UK and Australia or neighbouring country India.";
        }
        #endregion
        timerDisplay.GetComponent<TextMeshProUGUI>().text = "00:" + remainingSeconds;

    }

    #region Country Round System

    private void currentServerTurn(bool gBoard, bool cpTurn, bool ffholder)
    {
        GameBoard.Instance.gameObject.SetActive(gBoard);
        currentPlayerTurn.gameObject.SetActive(cpTurn);
        factsHolder.gameObject.SetActive(ffholder);
        factsText.SetText($"{facts[Random.Range(0, facts.Length)]}");
        madeTurnServer += 1;
        remainingSeconds = 10;
        StartCoroutine(timeSubtraction());
    }

    private void currentClientTurn(bool gBoard, bool cpTurn, bool ffholder)
    {
        GameBoard.Instance.gameObject.SetActive(gBoard);
        currentPlayerTurn.gameObject.SetActive(cpTurn);
        factsHolder.gameObject.SetActive(ffholder);
        factsText.SetText($"{facts[Random.Range(0, facts.Length)]}");
        madeTurnClient += 1;
    }


    [Server]
    public void rounds()
    {
        //Server
        if (isServer && isClient && remainingSeconds <= 0 && madeTurnServer == 1)
        {
            currentServerTurn(false, true, true);
            currentPlayerTurn.SetText("User 2's Turn !");

        }
        else if (isServer && isClient && remainingSeconds <= 0 && madeTurnServer == 2)
        {
            //Round 2
            currentServerTurn(true, false, false);
            roundDisplay.SetText($"Round {currentRound += 1}");

        }
        else if (isServer && isClient && remainingSeconds <= 0 && madeTurnServer == 3)
        {
            currentServerTurn(false, true, true);
            currentPlayerTurn.SetText("User 2's Turn !");

        }
        else if (isServer && isClient && remainingSeconds <= 0 && madeTurnServer == 4)
        {
            //Round 3
            currentServerTurn(true, false, false);
            roundDisplay.SetText($"Round {currentRound += 1}");

        }
        else if (isServer && isClient && remainingSeconds <= 0 && madeTurnServer == 5)
        {
            currentServerTurn(false, true, true);
            currentPlayerTurn.SetText("User 2's Turn !");

        }
        else if (isServer && isClient && remainingSeconds <= 0 && madeTurnServer == 6)
        {
            currentServerTurn(false, false, true);
            gameOver.gameObject.SetActive(true);
            roundDisplay.SetText("Rounds Finished!");
            remainingSeconds = 5;
            StartCoroutine(timeSubtraction());

        }
        else if (isServer && isClient && remainingSeconds <= 0 && madeTurnServer == 7)
        {
            GameBoard.Instance.gameObject.SetActive(false);
            factsHolder.gameObject.SetActive(false);
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
                winningScore += ScoreSystem.instance.score; //updates current winning score for host 
                if (this.gameObject.scene.name == "FranceBoard")
                {
                    instance.SubmitScore("France", winningScore);
                } 
                else if(this.gameObject.scene.name == "NigeriaBoard")
                {
                    instance.SubmitScore("Nigeria", winningScore);
                }
                else if (this.gameObject.scene.name == "PakistanBoard")
                {
                    instance.SubmitScore("Pakistan", winningScore);
                }
                else if (this.gameObject.scene.name == "ChinaBoard")
                {
                    instance.SubmitScore("China", winningScore);
                }

                
                winnerorloser.SetText("You Win!!");
            }
            else
            {
                
                winnerorloser.SetText("You lose.");
            }

            instance.awardPlayer();

        }
        else if (isServer && isClient && remainingSeconds <= 0 && madeTurnServer == 8)
        {
            
        }

        clientRounds();
    }

    [ClientRpc]
    public void clientRounds()
    {
        //Client
        if (isClientOnly && GameBoard.Instance && remainingSeconds >= 0 && madeTurnClient == 1)
        {
            currentClientTurn(false, true, true);
            currentPlayerTurn.SetText("User 1's Turn !");

        }
        else if (isClientOnly && remainingSeconds <= 0 && madeTurnClient == 2)
        {
            currentClientTurn(true, false, false);

        }
        else if (isClientOnly && remainingSeconds <= 0 && madeTurnClient == 3 && madeTurnServer == 2)
        {
            currentClientTurn(false, true, true);
            currentPlayerTurn.SetText("User 1's Turn !");
            roundDisplay.SetText($"Round 2");

        }
        else if (isClientOnly && remainingSeconds <= 0 && madeTurnClient == 4 && madeTurnServer == 3)
        {
            currentClientTurn(true, false, false);

        }
        else if (isClientOnly && remainingSeconds <= 0 && madeTurnClient == 5 && madeTurnServer == 4)
        {
            currentClientTurn(false, true, true);
            currentPlayerTurn.SetText("User 1's Turn !");
            roundDisplay.SetText($"Round 3");

        }
        else if (isClientOnly && remainingSeconds <= 0 && madeTurnClient == 6 && madeTurnServer == 5)
        {
            currentClientTurn(true, false, false);

        }
        else if (isClientOnly && remainingSeconds <= 0 && madeTurnClient == 7 && madeTurnServer == 6)
        {
            currentClientTurn(false, true, true);
            roundDisplay.SetText("Rounds Finished!");

        }
        else if (isClientOnly && remainingSeconds <= 0 && madeTurnClient == 8 && madeTurnServer == 7)
        {
            GameBoard.Instance.gameObject.SetActive(false);
            factsHolder.gameObject.SetActive(false);
            winningScreen.gameObject.SetActive(true);
            user1Score.gameObject.SetActive(false);
            user2Score.gameObject.SetActive(false);
            gameOver.gameObject.SetActive(false);
            madeTurnClient += 1;
            user1FinalScore.SetText($"User 1's Score  {ScoreSystem.instance._score}");
            user2FinalScore.SetText($"User 2's Score  {ScoreSystem.instance._opponentScore}");

            if (ScoreSystem.instance.opponentScore > ScoreSystem.instance.score)
            {
                winningScore += ScoreSystem.instance.opponentScore; //updates current winning score for client
                if (this.gameObject.scene.name == "FranceBoard")
                {
                    instance.SubmitScore("France", winningScore);
                }
                else if (this.gameObject.scene.name == "NigeriaBoard")
                {
                    instance.SubmitScore("Nigeria", winningScore);
                }
                else if (this.gameObject.scene.name == "PakistanBoard")
                {
                    instance.SubmitScore("Pakistan", winningScore);
                }
                else if (this.gameObject.scene.name == "ChinaBoard")
                {
                    instance.SubmitScore("China", winningScore);
                }
                winnerorloser.SetText("You Win!!");
                
            }
            else
            {
              
                winnerorloser.SetText("You lose.");
                
            }

            instance.awardPlayer();

        }
        else if (isClientOnly && remainingSeconds <= 0 && madeTurnClient == 9 && madeTurnServer == 8)
        {
          
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

        if (GameBoard.Instance && isServer && isClient)
        {
            rounds();
            

        }



    }

}











