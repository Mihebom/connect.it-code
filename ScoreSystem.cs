using Mirror;
using UnityEngine;
using TMPro;
using System.Linq;

//Written by Matthew Ihebom

public sealed class ScoreSystem : NetworkBehaviour
{
    public static ScoreSystem instance { get; private set; }

    [SyncVar] public int _score;

    public int _opponentScore;

    public int score
    {
        get => _score;

        set
        {
            if (_score == value) return;

            _score = value;

            scoreText.SetText($"Score = {_score}");
        }
    }

    public int opponentScore
    {
        get => _opponentScore;

        set
        {
            if (_opponentScore == value) return;

            _opponentScore = value;

            opponentScoreText.SetText($"Score = {_opponentScore}");
        }
    }

    [SerializeField] public TextMeshProUGUI scoreText;

    [SerializeField] public TextMeshProUGUI opponentScoreText;

    public void Awake() => instance = this;

}
