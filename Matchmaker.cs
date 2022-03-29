using System.Collections;
using UnityEngine;
using PlayFab;
using PlayFab.MultiplayerModels;
using TMPro;
using UnityEngine.SceneManagement;
using System.Net;
using System.Collections.Generic;

public class Matchmaker : MonoBehaviour
{
    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject leaveQueueButton;
    [SerializeField] private TMP_Text queueStatusText;

    private string ticketId;
    private Coroutine pollTicketCoroutine;
 
    private static string QueueName = "DefaultQueue";

    public void StartMatchmaking()
    {
        playButton.SetActive(false);
        leaveQueueButton.SetActive(true);
        queueStatusText.text = "Submitting Ticket";
        queueStatusText.gameObject.SetActive(true);

        PlayFabMultiplayerAPI.CreateMatchmakingTicket(
            new CreateMatchmakingTicketRequest
            {
                Creator = new MatchmakingPlayer
                {
                    Entity = new EntityKey
                    {
                        Id = PlayFabManager.EntityId,
                        Type = "title_player_account",
                    },
                    Attributes = new MatchmakingPlayerAttributes
                    {
                        DataObject = new
                        {

                            latencies = new object[]
                            {
                                new
                                {
                                    region = "NorthEurope",
                                    latency = 100,
                                },

                            },

                        }

                    }



                },




                GiveUpAfterSeconds = 120,

                QueueName = QueueName
            },
            OnMatchmakingTicketCreated,
            OnMatchmakingError
        ) ;

        
    }
 
    public void LeaveQueue()
    {
        leaveQueueButton.SetActive(false);
        queueStatusText.gameObject.SetActive(false);

        PlayFabMultiplayerAPI.CancelMatchmakingTicket(
            new CancelMatchmakingTicketRequest
            {
                QueueName = QueueName,
                TicketId = ticketId
            },
            OnTicketCanceled,
            OnMatchmakingError
        );
    }

    private void OnTicketCanceled(CancelMatchmakingTicketResult result)
    {
        playButton.SetActive(true);
    }

    private void OnMatchmakingTicketCreated(CreateMatchmakingTicketResult result)
    {
        ticketId = result.TicketId;
        pollTicketCoroutine = StartCoroutine(PollTicket(result.TicketId));

        leaveQueueButton.SetActive(true);
        queueStatusText.text = "Ticket Created";
    }

    private void OnMatchmakingError(PlayFabError error)
    {
        Debug.LogError(error.GenerateErrorReport());
    }

    private IEnumerator PollTicket(string ticketId)
    {
        while (true)
        {
            PlayFabMultiplayerAPI.GetMatchmakingTicket(
                new GetMatchmakingTicketRequest
                {
                    TicketId = ticketId,
                    QueueName = QueueName
                },
                OnGetMatchMakingTicket,
                OnMatchmakingError
            );

            yield return new WaitForSeconds(6);
        }
    }

    private void OnGetMatchMakingTicket(GetMatchmakingTicketResult result)
    {
        queueStatusText.text = $"Status: {result.Status}";

        switch (result.Status)
        {
            case "Matched":
                StopCoroutine(pollTicketCoroutine);
                StartMatch(result.MatchId);
                break;
            case "Canceled":
                StopCoroutine(pollTicketCoroutine);
                leaveQueueButton.SetActive(false);
                queueStatusText.gameObject.SetActive(false);
                playButton.SetActive(true);
                break;
        }
    }
   
    private void StartMatch(string matchId)
    {
        queueStatusText.text = $"Starting Match";

        PlayFabMultiplayerAPI.GetMatch(
            new GetMatchRequest
            {
                MatchId = matchId,
                QueueName = QueueName
            },
            OnGetMatch,
            OnMatchmakingError
        );

        if (this.gameObject.scene.name == "FranceMatchMaking")
        {
            SceneManager.LoadScene("FranceLobby");
        }
        else if(this.gameObject.scene.name == "NigeriaMatchMaking")
        {
            SceneManager.LoadScene("NigeriaLobby");
        } else if(this.gameObject.scene.name == "PakistanMatchMaking")
        {
            SceneManager.LoadScene("PakistanLobby");
        }
        else if(this.gameObject.scene.name == "ChinaMatchMaking")
        {
            SceneManager.LoadScene("ChinaLobby");
        }
    }

    private void OnGetMatch(GetMatchResult result)
    {
        queueStatusText.text = $"{PlayFabManager.Instance.displayName} vs {PlayFabManager.Instance.displayName}";

    }

 

  

}
