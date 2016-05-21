using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System;


public class PasscodeLogin : ALoginFactory 
{
    public Text passcode;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void Init()
    {
        bool loadScene = false;
        if ( passcode != null && passcode.text != "" )
        {
            loadScene = false;
            
            try 
            { 
                PlayerPrefs.SetString ( "Passcode", passcode.text );
                loadScene = true;
            }
            catch (System.FormatException e)
            {
                Debug.LogWarning("Print waring to user. "+e.Message);
            }
        }

        if ( loadScene )
        {
            SceneManager.LoadScene ( "GamePlay" );
        }
    }

    public void QRcodeReader ( string s )
    {
        PlayerPrefs.SetString ( "Passcode", s );
        SceneManager.LoadScene ( "GamePlay" );
    }

    public override void Destory()
    {
        SceneManager.LoadScene ( "Menu" );
    }

    public override void Save()
    {
        _using.profile.score = _using.game.GetScore ( );
        StartCoroutine ( SaveDelay ( ) );
    }

    public override void Load()
    {
        if ( PlayerPrefs.HasKey ( "Passcode" ) )
        {
            string s = PlayerPrefs.GetString ( "Passcode" );
            if ( _using.mathHelp.CheckPass ( s ) )
                _using.mathHelp.FromPass ( s );
            
            PlayerPrefs.DeleteKey ( "Passcode" );
        }
    }

    IEnumerator SaveDelay ( )
    {
        yield return new WaitForSeconds ( .5f );
        
        string pass = _using.mathHelp.MakePass ( _using.profile.colorBody, _using.profile.colorBinder, _using.profile.colorEngine, new Vector3 ( _using.profile.xPos, _using.profile.yPos, _using.profile.zPos ), _using.profile.score );
        
        _using.profile.passCode = pass;
        
        Debug.Log("PassCode : " + pass);
        
        _using.game.DebugConsole ( "Save!! : To Passcode" );
        _using.game.DebugConsole ( "PassCode : " + _using.profile.passCode );
        
    }

}
