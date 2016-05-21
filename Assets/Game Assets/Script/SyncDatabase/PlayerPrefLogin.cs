using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System;
using System.Collections.Generic;

/// <summary>
/// PlayerPrefLogin create for support in game don't have a database. ( save & load offline )
/// </summary>
public class PlayerPrefLogin : ALoginFactory 
{
    /// <summary>
    /// Keep username.
    /// </summary>
    public Text playerPrefsText;

    /// <summary>
    /// Keep passcode when player have a pass code.
    /// </summary>
    public Text password;

    private DataValue dataValue;

    /// <summary>
    /// Time delay on save.
    /// </summary>
    public const float DELAY_TIME = .5f;

    /// <summary>
    /// Struct for keep data from PlayerPrefs. ( Current data )
    /// </summary>
    public struct DataValue
    {
        public string username;
        public int score;
        public Vector3 position;
        public Color engine;
        public Color binder;
        public Color body;

    }

    protected override void Awake ( )
    {
        base.Awake ( );
    }

    /// <summary>
    /// Delay before save data.
    /// </summary>
    /// <returns></returns>
    IEnumerator SaveDelay ( )
    {
        yield return new WaitForSeconds ( DELAY_TIME );
        
        SyncToDataValue ( _using.profile.score );
        
        SelectUser selectUser = new SelectUser();

        // Set data to PlayerPrefs. ( ArrayPrefs )
        selectUser.SetIntArr ( DataMatching < int > ( dataValue.score, selectUser.GetIntList ( ) ) );
        selectUser.SetVec3Arr ( DataMatching < Vector3 > ( dataValue.position, selectUser.GetVec3List ( ) ) );
        selectUser.SetColorArr ( DataMatching < Color > ( dataValue.engine, selectUser.GetColorList ( SelectUser.PlayerPrefsKeys.ENGINE_COLOR ) ), SelectUser.PlayerPrefsKeys.ENGINE_COLOR );
        selectUser.SetColorArr ( DataMatching < Color > ( dataValue.binder, selectUser.GetColorList ( SelectUser.PlayerPrefsKeys.BINDER_COLOR ) ), SelectUser.PlayerPrefsKeys.BINDER_COLOR );
        selectUser.SetColorArr ( DataMatching < Color > ( dataValue.body, selectUser.GetColorList ( SelectUser.PlayerPrefsKeys.BODY_COLOR ) ), SelectUser.PlayerPrefsKeys.BODY_COLOR );
         

        // Make passcode by current data.
        string pass = _using.mathHelp.MakePass ( _using.profile.colorBody, _using.profile.colorBinder, _using.profile.colorEngine, new Vector3 ( _using.profile.xPos, _using.profile.yPos, _using.profile.zPos ), _using.profile.score );
        _using.profile.passCode = pass;
        Debug.Log("PassCode : " + pass);

        // Print debug to option
        _using.game.DebugConsole ( "Save!! : To PlayerPrefs" );
        _using.game.DebugConsole ( "Position : " + new Vector3 ( _using.profile.xPos, _using.profile.yPos, _using.profile.zPos ) );
        _using.game.DebugConsole ( "Score : " + _using.profile.score );

        _using.game.DebugConsole ( "PassCode : " + _using.profile.passCode );
    }

    /// <summary>
    /// Save data to dataValue.
    /// </summary>
    /// <param name="score"></param>
    private void SyncToDataValue ( int score )
    {
        // Position
        _using.profile.xPos = _using.game.GetRotation().x;
        _using.profile.yPos = _using.game.GetRotation().y;
        _using.profile.zPos = _using.game.GetRotation().z;
        
        // save current color from ship
        _using.profile.SaveColor ( );

        dataValue.score = score;
        dataValue.position = new Vector3 ( _using.profile.xPos, _using.profile.yPos, _using.profile.zPos );
        dataValue.body = _using.profile.colorBody;
        dataValue.engine = _using.profile.colorEngine;
        dataValue.binder = _using.profile.colorBinder;
    }

    /// <summary>
    /// Sync data by passcode.
    /// This use in old version. ( Now we use ArrayPrefs instead PlayerPrefs )
    /// </summary>
    /// <param name="s"></param>
    private void SyncFromPasscode ( string s )
    {
        _using.mathHelp.FromPass ( s );

        // Score
        PlayerPrefs.SetInt ( PlayerPrefs.GetString ( "UserName" ) + "_Score", _using.mathHelp.score );

        // Position
        PlayerPrefs.SetFloat ( PlayerPrefs.GetString ( "UserName" ) + "_PosX", _using.mathHelp.rotate.x );
        PlayerPrefs.SetFloat ( PlayerPrefs.GetString ( "UserName" ) + "_PosY", _using.mathHelp.rotate.y );
        PlayerPrefs.SetFloat ( PlayerPrefs.GetString ( "UserName" ) + "_PosZ", _using.mathHelp.rotate.z );
                    
        // Body color
        PlayerPrefs.SetFloat ( PlayerPrefs.GetString ( "UserName" ) + "_BodyColor_R", _using.mathHelp.body.r );
        PlayerPrefs.SetFloat ( PlayerPrefs.GetString ( "UserName" ) + "_BodyColor_G", _using.mathHelp.body.g );
        PlayerPrefs.SetFloat ( PlayerPrefs.GetString ( "UserName" ) + "_BodyColor_B", _using.mathHelp.body.b );
                
        // Engine color
        PlayerPrefs.SetFloat ( PlayerPrefs.GetString ( "UserName" ) + "_EngineColor_R", _using.mathHelp.engine.r );
        PlayerPrefs.SetFloat ( PlayerPrefs.GetString ( "UserName" ) + "_EngineColor_G", _using.mathHelp.engine.g );
        PlayerPrefs.SetFloat ( PlayerPrefs.GetString ( "UserName" ) + "_EngineColor_B", _using.mathHelp.engine.b );

        // Binder color
        PlayerPrefs.SetFloat ( PlayerPrefs.GetString ( "UserName" ) + "_BinderColor_R", _using.mathHelp.binder.r );
        PlayerPrefs.SetFloat ( PlayerPrefs.GetString ( "UserName" ) + "_BinderColor_G", _using.mathHelp.binder.g );
        PlayerPrefs.SetFloat ( PlayerPrefs.GetString ( "UserName" ) + "_BinderColor_B", _using.mathHelp.binder.b );
    }
    
    /// <summary>
    /// Do this after camera detect qr code. 
    /// Save passcode from qr and load scene "GamePlay".
    /// </summary>
    /// <param name="s"></param>
    public void QRDecoding ( string passcode )
    {
        PlayerPrefs.SetString ( "Passcode", passcode );
        SceneManager.LoadScene ( "GamePlay" );
    }

    /// <summary>
    /// Init on click start game button.
    /// Check player input passcode and set it into PlayerPrefs for use it on Load method.
    /// </summary>
    public override void Init ( )
    {
        if ( password != null && password.text != "" )
        {
            try 
            {
                PlayerPrefs.SetString ( "Passcode", password.text ); 
            }
            catch (System.FormatException e)
            {
                Debug.LogWarning("Print waring to user. "+e.Message);
            }  
        }

        SceneManager.LoadScene("GamePlay");
    }

    /// <summary>
    /// Destory it like a logout .
    /// 
    /// </summary>
    public override void Destory ( )
    {
        SceneManager.LoadScene ( "Menu" );
    }

    /// <summary>
    /// Save data into ArrayPrefs.
    /// </summary>
    public override void Save ( )
    {
        _using.profile.score = _using.game.GetScore ( );
        StartCoroutine ( SaveDelay ( ) );
    }

    /// <summary>
    /// Sync data from ArrayPrefs into player data.
    /// Or Sync data from passcode into player data.
    /// Load data on start scene "GamePlay"
    /// </summary>
    public override void Load ( )
    {
        SelectUser selectUser = new SelectUser ( );
         
        dataValue.username = FindIndex < string > ( selectUser.GetStringList ( ) );
        dataValue.score = FindIndex < int > ( selectUser.GetIntList ( ) );
        dataValue.position = FindIndex < Vector3 > ( selectUser.GetVec3List ( ) );
        dataValue.engine = FindIndex < Color > ( selectUser.GetColorList ( SelectUser.PlayerPrefsKeys.ENGINE_COLOR ) );
        dataValue.binder = FindIndex < Color > ( selectUser.GetColorList ( SelectUser.PlayerPrefsKeys.BINDER_COLOR ) );
        dataValue.body = FindIndex < Color > ( selectUser.GetColorList ( SelectUser.PlayerPrefsKeys.BODY_COLOR ) );

        if ( PlayerPrefs.HasKey ( "Passcode" ) )
        {
            string s = PlayerPrefs.GetString ( "Passcode" );
            PlayerPrefs.DeleteKey ( "Passcode" );
            
            if ( _using.mathHelp.FromPass ( s ) )
            { 
                dataValue.score = _using.mathHelp.score;
                dataValue.position = _using.mathHelp.rotate;
                dataValue.engine = _using.mathHelp.engine;
                dataValue.binder = _using.mathHelp.binder;
                dataValue.body = _using.mathHelp.body;
            }
            else
            {
                Debug.LogWarning ( "Passcode is worng!! " );
                SceneManager.LoadScene("Menu");
            }
        }

        try
        { 
            // Set score.
            _using.profile.score = dataValue.score;
            _using.game.SetScore ( _using.profile.score );

            // Set position.
            _using.profile.xPos = dataValue.position.x;
            _using.profile.yPos = dataValue.position.y;
            _using.profile.zPos = dataValue.position.z;
            _using.player.MoveToRotation ( dataValue.position );

            // Set ship color.
            _using.profile.LoadColor ( dataValue.engine, dataValue.binder, dataValue.body );

            // Print debug to option
            _using.game.DebugConsole( "Load!!" );
            _using.game.DebugConsole( "Position : " + new Vector3 ( _using.profile.xPos, _using.profile.yPos, _using.profile.zPos ) );
            _using.game.DebugConsole( "Score : " + _using.profile.score );
        }
        catch { }
    }

    /// <summary>
    /// Pull data from index in List ( from ArrayPrefs ) to value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="t"></param>
    /// <returns></returns>
    private T FindIndex<T> ( List<T> t )
    {
        int currentIndex = PlayerPrefs.GetInt ( "CurrentDataIndex" );
        
        var value = t[currentIndex];

        return value;
    }

    /// <summary>
    /// Push new data to index in List. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="t"></param>
    /// <param name="ListT"></param>
    /// <returns></returns>
    private List<T> DataMatching<T> ( T t, List<T> ListT)
    {
        int currentIndex = PlayerPrefs.GetInt ( "CurrentDataIndex" );

        ListT[currentIndex] = t;

        return ListT;
    }

}
