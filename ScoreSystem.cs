
using UnityEngine;
using TMPro;

public sealed class ScoreSystem : MonoBehaviour
{

    public static ScoreSystem instance { get; private set; }

    private int _score;


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

    [SerializeField] private TextMeshProUGUI scoreText;

    private void Awake() => instance = this;
 
}
