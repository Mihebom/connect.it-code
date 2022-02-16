using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Threading.Tasks;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;
using Mirror;
using UnityEngine.UI;



public class GameBoard : NetworkBehaviour
{

    public static GameBoard Instance { get; private set; }

    public Row[] rows;

    public Tile[,] tiles { get; private set; }

    public int rowWidth => tiles.GetLength(0);

    public int rowHeight => tiles.GetLength(1);

    public List<Tile> selection = new List<Tile>();

    [SyncVar] private float TweenDuration = 0.25f;

    [SerializeField] private AudioClip selectSound;

    [SerializeField] private AudioSource _audioSource;

    public static Counter[] counters;

    public void Update()
    {
        if (isServer && isClient)
        {
            serverUpdateBoardScore();
        }
    }

    #region Board Creation

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    void Start()
    {
        //Determines which country board is currently being played on and loads counters based on that country
        if (this.gameObject.scene.name == "FranceBoard")
        {
            counters = Resources.LoadAll<Counter>("FranceBoardCounters/");
        }
        
        if(this.gameObject.scene.name == "ChinaBoard")
        {
            counters = Resources.LoadAll<Counter>("ChinaBoardCounters/");
           
        }

        Instance = this;

        //creates tiles that will hold each board counter
        tiles = new Tile[rows.Max(rowList => rowList.tiles.Length), rows.Length];

        //populates each tile with a counter
        for (var i = 0; i < rowHeight; i++)
        {
            for (var j = 0; j < rowWidth; j++)
            {

                var tile = rows[i].tiles[j];

                tile.x = j;
                tile.y = i;

                tile.counter = counters[Random.Range(0, counters.Length)];
               
                tiles[j, i] = tile;
            }

        }

    }

    #endregion

    #region Board Intearction


    //This function takes in two tiles and switches their positions
    public async void Select(Tile tile)
    {

        if (!selection.Contains(tile)) selection.Add(tile);

        if (selection.Count < 2) return;

        Debug.Log($"Selected tiles at ({selection[0].x}, {selection[0].y}) and ({selection[1].x}, {selection[1].y}) ");

        await Swap(selection[0], selection[1]);

        if (canPop())
        {
            pop();
        }
        else
        {
            await Swap(selection[0], selection[1]);
        }

        selection.Clear();

    }


    //This method defines the swapping animations for each tiles, and rearranging their position in the 2D array
    public async Task Swap(Tile tile1, Tile tile2)
    {

        var icon1 = tile1.icon;
        var icon2 = tile2.icon;
        var icon1Transform = icon1.transform;
        var icon2Transform = icon2.transform;
        var sequence = DOTween.Sequence();

        sequence.Join(icon1Transform.DOMove(icon2Transform.position, TweenDuration))
                .Join(icon2Transform.DOMove(icon1Transform.position, TweenDuration));

        await sequence.Play()
                      .AsyncWaitForCompletion();

        icon1Transform.SetParent(tile2.transform);
        icon2Transform.SetParent(tile1.transform);

        tile1.icon = icon2;
        tile2.icon = icon1;

        var tile1Counter = tile1.counter;
        tile1.counter = tile2.counter;
        tile2.counter = tile1Counter;

    }


    //checks whether the tiles match or not
    private bool canPop()
    {
        for (var y = 0; y < rowHeight; y++)
            for (var x = 0; x < rowWidth; x++)
                if (tiles[x, y].connectedTiles().Skip(1).Count() >= 2) return true;

        return false;
    }



    //If tiles are able to be matched, "pop" them and calculate a score for the users
    public async void pop()
    {

        
        for (var y = 0; y < rowHeight; y++)

            for (var x = 0; x < rowWidth; x++)
            {

                var tile = tiles[x, y];

                var matchedTiles = tile.connectedTiles();

                if (matchedTiles.Skip(1).Count() < 2) continue;

                var scaleDownSequence = DOTween.Sequence();

                foreach (var matchedTile in matchedTiles) scaleDownSequence.Join(matchedTile.icon.transform.DOScale(Vector3.zero, TweenDuration));

                _audioSource.PlayOneShot(selectSound);

                if (isServer && isClient)
                {
                    if (matchedTiles.Count == 4)
                    {
                        ScoreSystem.instance.score += tile.counter.value * matchedTiles.Count * 2;

                    }
                    else if (matchedTiles.Count == 5)
                    {
                        ScoreSystem.instance.score += tile.counter.value * matchedTiles.Count * 3;
                    }
                    else
                    {
                        ScoreSystem.instance.score += tile.counter.value * matchedTiles.Count;
                    }


                }
                else
                {
                    if (matchedTiles.Count == 4)
                    {
                        ScoreSystem.instance.opponentScore += tile.counter.value * matchedTiles.Count * 2;

                    }
                    else if (matchedTiles.Count == 5)
                    {
                        ScoreSystem.instance.opponentScore += tile.counter.value * matchedTiles.Count * 3;
                    }
                    else
                    {
                        ScoreSystem.instance.opponentScore += tile.counter.value * matchedTiles.Count;
                    }

                }

                await scaleDownSequence.Play()
                                   .AsyncWaitForCompletion();

                var upScaleSequence = DOTween.Sequence();

                foreach (var matchedTile in matchedTiles)
                {

                    matchedTile.counter = counters[Random.Range(0, counters.Length)];

                    upScaleSequence.Join(matchedTile.icon.transform.DOScale(Vector3.one, TweenDuration));

                }

                await upScaleSequence.Play()
                                     .AsyncWaitForCompletion();

                x = 0;
                y = 0;

            }
    }

    #endregion

    #region Score Update


    //Updates user server and client score on both player screens
    [Server]
    public void serverUpdateBoardScore()
    {
        rpcUpdateBoardScore();
    }



    //Sends client score to server, and server score to client
    [ClientRpc]
    public void rpcUpdateBoardScore()
    {
        ScoreSystem.instance.opponentScoreText.SetText($"Score = {ScoreSystem.instance._opponentScore}");

        //Updates server score on client
        ScoreSystem.instance.scoreText.SetText($"Score = {ScoreSystem.instance._score}");
    }

    #endregion

}

