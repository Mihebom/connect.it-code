
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

public class ChinaGameBoard : NetworkBehaviour
{

    public static ChinaGameBoard Instance { get; private set; }

    public Row[] rows;

    public Tile[,] tiles { get; private set; }

    public int rowWidth => tiles.GetLength(0);

    public int rowHeight => tiles.GetLength(1);

    public List<Tile> selection = new List<Tile>();

    [SyncVar] private float TweenDuration = 0.25f;

    public static ChinaCounter[] chinaCounters;

    [SerializeField] private AudioClip selectSound;

    [SerializeField] private AudioSource _audioSource;

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
        chinaCounters = Resources.LoadAll<ChinaCounter>("ChinaBoardCounters/");

        Instance = this;

        tiles = new Tile[rows.Max(rowList => rowList.tiles.Length), rows.Length];

        for (var i = 0; i < rowHeight; i++)
        {
            for (var j = 0; j < rowWidth; j++)
            {

                var tile = rows[i].tiles[j];

                tile.x = j;
                tile.y = i;

                tile.chinaCounter = chinaCounters[Random.Range(0, chinaCounters.Length)];

                tiles[j, i] = tile;
            }

        }

    }

    #endregion

    #region Board Intearction

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

        var tile1Counter = tile1.chinaCounter;
        tile1.chinaCounter = tile2.chinaCounter;
        tile2.chinaCounter = tile1Counter;

    }

    private bool canPop()
    {
        for (var y = 0; y < rowHeight; y++)
            for (var x = 0; x < rowWidth; x++)
                if (tiles[x, y].chinaConnectedTiles().Skip(1).Count() >= 2) return true;

        return false;
    }

    public async void pop()
    {

        for (var y = 0; y < rowHeight; y++)

            for (var x = 0; x < rowWidth; x++)
            {

                var tile = tiles[x, y];

                var matchedTiles = tile.chinaConnectedTiles();

                if (matchedTiles.Skip(1).Count() < 2) continue;

                var scaleDownSequence = DOTween.Sequence();

                foreach (var matchedTile in matchedTiles) scaleDownSequence.Join(matchedTile.icon.transform.DOScale(Vector3.zero, TweenDuration));

                _audioSource.PlayOneShot(selectSound);

                if (isServer && isClient)
                {
                    if (matchedTiles.Count == 4)
                    {
                        ScoreSystem.instance.score += tile.chinaCounter.value * matchedTiles.Count * 2;

                    }
                    else if (matchedTiles.Count == 5)
                    {
                        ScoreSystem.instance.score += tile.chinaCounter.value * matchedTiles.Count * 3;
                    }
                    else
                    {
                        ScoreSystem.instance.score += tile.chinaCounter.value * matchedTiles.Count;
                    }
                }
                else
                {
                    if (matchedTiles.Count == 4)
                    {
                        opponentScoreSystem.instance.opponentScore += tile.chinaCounter.value * matchedTiles.Count * 2;

                    }
                    else if (matchedTiles.Count == 5)
                    {
                        opponentScoreSystem.instance.opponentScore += tile.chinaCounter.value * matchedTiles.Count * 3;
                    }
                    else
                    {
                        opponentScoreSystem.instance.opponentScore += tile.chinaCounter.value * matchedTiles.Count;
                    }
                }

              

                await scaleDownSequence.Play()
                                        .AsyncWaitForCompletion();

                var upScaleSequence = DOTween.Sequence();

                foreach (var matchedTile in matchedTiles)
                {

                    matchedTile.chinaCounter = chinaCounters[Random.Range(0, chinaCounters.Length)];

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

    [Server]
    public void serverUpdateBoardScore()
    {
        rpcUpdateBoardScore();
    }

    [ClientRpc]
    public void rpcUpdateBoardScore()
    {
        //Updates client score on server
        opponentScoreSystem.instance.opponentScoreText.SetText($"Score = { opponentScoreSystem.instance._opponentScore}");

        //Updates server score on client
        ScoreSystem.instance.scoreText.SetText($"Score = { ScoreSystem.instance._score}");
    }

    #endregion

}















