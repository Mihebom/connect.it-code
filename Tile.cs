using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class Tile : MonoBehaviour
{

    public int x;

    public int y;

    private Counter _counter;

    public Counter counter
    {
        get => _counter;

        set
        {

            if (_counter == value) return;

            _counter = value;

            icon.sprite = _counter.sprite;
        }

    }
    public Image icon;
 
    public Button button;

    public Tile left => x > 0 ? GameBoard.Instance.tiles[x - 1, y] : null;
    public Tile top => y > 0 ? GameBoard.Instance.tiles[x, y - 1] : null;
    public Tile right => x < GameBoard.Instance.rowWidth - 1 ? GameBoard.Instance.tiles[x + 1, y] : null;
    public Tile bottom => y < GameBoard.Instance.rowHeight - 1 ? GameBoard.Instance.tiles[x, y + 1] : null;


    public Tile[] neighbour => new Tile[]
       {
        left,
        top,
        right,
        bottom,
       };


    private void Start() => button.onClick.AddListener(() => GameBoard.Instance.Select(this));

    


    public List<Tile> connectedTiles(List<Tile> exclude = null)
    {
        var result = new List<Tile> { this, };

        if (exclude == null)
        {

            exclude = new List<Tile> { this, };
        }
        else
        {

            exclude.Add(this);
        }

        foreach (var neighbours in neighbour)
        {

            if (neighbours == null || exclude.Contains(neighbours) || neighbours.counter != counter) continue;

            result.AddRange(neighbours.connectedTiles(exclude));
        }

        return result;

    }

   


}
