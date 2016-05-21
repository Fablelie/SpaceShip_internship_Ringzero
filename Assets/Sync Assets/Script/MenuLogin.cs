using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

// Include Facebook namespace
using Facebook.Unity;

/// <summary>
/// User in old version ( facebook sync )
/// </summary>
public class MenuLogin : MonoBehaviour {

    public Sync sync;

    public GameObject loginBtn, facebookBtn, playerPrefsBtn;
    public Text playerPrefsText;

    public GameObject[] btn;

    public const int WIDTH = 480;
    public const int HEIGHT = 800;

    // Awake function from Unity's MonoBehavior
    void Awake()
    {
#if ( UNITY_STANDALONE ) 
        Screen.SetResolution ( WIDTH, HEIGHT, false );
#endif
        if (!FB.IsInitialized)
        {
            // Initialize the Facebook SDK
            FB.Init(InitCallback, OnHideUnity);
        }
        else
        {
            // Already initialized, signal an app activation App Event
            FB.ActivateApp();
        }
    }

    private void InitCallback()
    {
        if (FB.IsInitialized)
        {
            // Signal an app activation App Event
            FB.ActivateApp();
            // Continue with Facebook SDK
            // ...
        }
        else
        {
            Debug.Log("Failed to Initialize the Facebook SDK");
        }
    }

    private void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
        {
            // Pause the game - we will need to hide
            Time.timeScale = 0;
        }
        else
        {
            // Resume the game - we're getting focus again
            Time.timeScale = 1;
        }
    }

    // Players clicks on Sync
    public void BtnSync()
    {
        sync.SyncProfile();
    }

    // Players clicks on Sync
    public void Login()
    {
#if ( UNITY_EDITOR || UNITY_STANDALONE || UNITY_ANDROID) 

        playerPrefsBtn.SetActive ( true );
        loginBtn.SetActive ( false );
#endif
        
//#if ( UNITY_ANDROID || UNITY_IPHONE )
//        facebookBtn.SetActive ( true );
//        loginBtn.SetActive ( false );
//#endif

    }

    public void FacebookLogin ( )
    {
        var perms = new List<string>() { "public_profile" };
        FB.LogInWithReadPermissions(perms, AuthCallback);
    }

    public void PlayerPrefsLogin ( )
    {
        //if ( !PlayerPrefs.HasKey ( playerPrefsText.text + "_Score" ) )
        //{
            PlayerPrefs.SetString ( "UserName", playerPrefsText.text );

            SceneManager.LoadScene("GamePlay");
        //}
        //else
        //{
            //SceneManager.LoadScene("GamePlay");
        //}
    }

    private void AuthCallback(ILoginResult result)
    {
        if (FB.IsLoggedIn)
        {
            // AccessToken class will have session details
            var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
            // Print current access token's User ID
            Debug.Log(aToken.UserId);
            // Print current access token's granted permissions
            foreach (string perm in aToken.Permissions)
            {
                Debug.Log(perm);
            }

            // Loading Game Scene

            SceneManager.LoadScene("GamePlay");
        }
        else
        {
            Debug.Log("User cancelled login");
        }
    }

    public void DisableAll ( int index )
    {
        for ( int i = 0; i < btn.Length; i++ )
        {
            if ( i == index )
                btn[i].SetActive ( true );
            else
                btn[i].SetActive ( false );
        }

    }
}
