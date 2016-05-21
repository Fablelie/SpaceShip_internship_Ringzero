using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using Facebook.Unity;

public class LoginClient : MonoBehaviour 
{
    
    public Sync sync;
    
    public GameObject loginBtn, facebookBtn, playerPrefsBtn;
    //public Text playerPrefsText;

    private UsingCon _using;

    public const int WIDTH = 480;
    public const int HEIGHT = 800;
    public const string PASSWORD = "S10x100y200z92Br255Bg255Bb175BIr0BIg10BIb176Er10Eg255Eb10%";//"10100200922552551750101761025510";

    protected virtual void Awake ( )
    {
#if ( UNITY_STANDALONE ) 
        Screen.SetResolution ( WIDTH, HEIGHT, false );
#endif
        //var i = System.Text.Encoding.UTF8.GetBytes ( PASSWORD );
        ////string code = System.Convert.ToBase64String ( i );
        //print ( "Encode : " + code );

        //var j = System.Convert.FromBase64String ( code );
        //print ( "Decode : " + System.Text.Encoding.UTF8.GetString( j ) );
        
    }

    public void ClickStartBtn ( )
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

}
