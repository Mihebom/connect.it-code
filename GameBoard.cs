using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;
using Photon.Pun;

public sealed class GameBoard : MonoBehaviour 
{ 
   

    public static GameBoard Instance { get; private set; }

    public Row[] rows;

    public Tile[,] tiles { get; private set; }

    public int rowWidth => tiles.GetLength(0);

    public int rowHeight => tiles.GetLength(1);

    public TextMeshProUGUI timerDisplay;

    public int remainingSeconds = 30;

    public bool subtractTime = false;

    private readonly List<Tile> selection = new List<Tile>();

    private const float TweenDuration = 0.25f;

    PhotonView view;


    private void Awake()
    {
        Instance = this;
     
    }

    void Start()
    {

    
        view = GetComponent<PhotonView>();
       
        timerDisplay.GetComponent<TextMeshProUGUI>().text = "00:" + remainingSeconds;

        
        tiles = new Tile[rows.Max(row => row.tiles.Length), rows.Length];

        for (var i = 0; i < rowHeight; i++)
        {
            for(var j = 0; j < rowWidth; j++)
            {

                var tile = rows[i].tiles[j];

                tile.x = j;
                tile.y = i;

                tile.counter = CounterStore.counters[Random.Range(0, CounterStore.counters.Length)];
                
                tiles[j, i] = tile;
            }

        }


             
    }

    void Update()
    {
      


            if(subtractTime == false && remainingSeconds > 0) { 
        

                StartCoroutine(timeSubtraction());

            }

            if (remainingSeconds <= 0)
            {

            SceneManager.LoadScene("demoOver");
             
        }





    }

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

            var tile1Counter = tile1.counter;
            tile1.counter = tile2.counter;
            tile2.counter = tile1Counter;
     

        

    }



    private bool canPop()
    {

        for( var y = 0; y < rowHeight; y++)
            for(var x = 0; x < rowWidth; x++)
             if (tiles[x, y].connectedTiles().Skip(1).Count() >= 2) return true;

               
            

        
        
        return false;
    }

    private async void pop()
    {
        for (var y = 0; y < rowHeight; y++)
        {
            for (var x = 0; x < rowWidth; x++)
            {

                var tile = tiles[x, y];

                var matchedTiles = tile.connectedTiles();

                if (matchedTiles.Skip(1).Count() < 2) continue;

                var scaleDownSequence = DOTween.Sequence();

                foreach(var matchedTile in matchedTiles) scaleDownSequence.Join(matchedTile.icon.transform.DOScale(Vector3.zero, TweenDuration));

                

                await scaleDownSequence.Play()
                                        .AsyncWaitForCompletion();


                if (matchedTiles.Count == 4)
                {
                    ScoreSystem.instance.score += tile.counter.value * matchedTiles.Count * 2;

                } else if(matchedTiles.Count == 5)
                {
                    ScoreSystem.instance.score += tile.counter.value * matchedTiles.Count * 3;
                } else
                {
                    ScoreSystem.instance.score += tile.counter.value * matchedTiles.Count;
                }

                var upScaleSequence = DOTween.Sequence();

                foreach(var matchedTile in matchedTiles)
                {

                    matchedTile.counter = CounterStore.counters[Random.Range(0, CounterStore.counters.Length)];

                    upScaleSequence.Join(matchedTile.icon.transform.DOScale(Vector3.one, TweenDuration));


                }

                await upScaleSequence.Play()
                                     .AsyncWaitForCompletion();

                x = 0;
                y = 0;


            }

        }
    }

   
    IEnumerator<WaitForSeconds> timeSubtraction()
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




}
