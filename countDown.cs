using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;


public class countDown : MonoBehaviour
{


    public static countDown instance { get; private set; }

    public TextMeshProUGUI timerDisplay;
    public int remainingSeconds = 30;
    public bool subtractTime = false;

    void Start()
    {


        timerDisplay.GetComponent<TextMeshProUGUI>().text = "00:" + remainingSeconds;

    }

    void Update()
    {

        if(subtractTime == false && remainingSeconds > 0)
        {

            StartCoroutine(timeSubtraction());

        }

        if(remainingSeconds <= 0)
        {

            SceneManager.LoadScene("MainMenu");

        }

    }

    IEnumerator timeSubtraction()
    {

        subtractTime = true;
        yield return new WaitForSeconds(1);
        remainingSeconds -= 1;
        if(remainingSeconds < 10)
        {
            timerDisplay.GetComponent<TextMeshProUGUI>().text = "00:0" + remainingSeconds;

        }else
        {
           timerDisplay.GetComponent<TextMeshProUGUI>().text = "00:" + remainingSeconds;
        }
        subtractTime = false;

    }

    private void Awake() => instance = this;




}
