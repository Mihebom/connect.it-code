using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class ProfileData
{
    public string username;
    public int level;
    public int xp;
}

public class Profile : MonoBehaviour
{
    public InputField usernameField;

    public static ProfileData myProfile = new ProfileData();



    public Text textElement;



    public void setUsername()
    {
        if(string.IsNullOrEmpty(usernameField.text))
        {
            myProfile.username = "RANDOM_USER_" + Random.Range(100, 1000);
        }
        else
        {
            myProfile.username = usernameField.text;
            
        }
        textElement.text = myProfile.username;

    }

    public void test()
    {
        Debug.Log(myProfile.username);
    }
}
