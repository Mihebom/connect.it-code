
using UnityEngine;
using TMPro;

public class opponentScoreSystem : MonoBehaviour
{
    public static opponentScoreSystem instance { get; private set; }

    private int _opponentScore;


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

    [SerializeField] private TextMeshProUGUI opponentScoreText;

    private void Awake() => instance = this;
}
