using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Facebook.Unity;

public class FacebookLogin : ALoginFactory
{
    private Regex regex;
    private Game _sync;

    protected override void Awake ( )
    {
        if ( !FB.IsInitialized )
        {
            FB.Init ( InitCallback, OnHideUnity );
        }
        else
        {
            FB.ActivateApp ( );
        }

        base.Awake ( );
    }
	
    private void InitCallback ( )
    {
        if ( FB.IsInitialized )
        {
            // Signal an app activation App Event
            FB.ActivateApp ( );
            // Continue with Facebook SDK
            // ...
        }
        else
        {
            Debug.Log ( "Failed to Initialize the Facebook SDK" );
        }
    }

    private void OnHideUnity ( bool isGameShown )
    {
        if ( !isGameShown )
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

    private void AuthCallback ( ILoginResult result )
    {
        if ( FB.IsLoggedIn )
        {
            // AccessToken class will have session details
            var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
            // Print current access token's User ID
            Debug.Log ( aToken.UserId );
            // Print current access token's granted permissions
            foreach ( string perm in aToken.Permissions )
            {
                Debug.Log ( perm );
            }

            // Loading Game Scene

            SceneManager.LoadScene ( "GamePlay" );
        }
        else
        {
            Debug.Log ( "User cancelled login" );
        }
    }

    public override void Init ( )
    {
        var perms = new List<string> ( ) { "public_profile" };
        FB.LogInWithReadPermissions ( perms, AuthCallback );
    }

    public override void Destory ( )
    {
        FB.LogOut ( );
        SceneManager.LoadScene ( "Menu" );
    }

    public override void Save ( )
    {
        _using.profile.xPos = _using.game.GetRotation().x;
        _using.profile.yPos = _using.game.GetRotation().y;
        _using.profile.zPos = _using.game.GetRotation().z;
        _using.profile.SaveColor();
        _using.profile.score = _using.game.GetScore();

        // Encode json.
        string sendData =
            "\"Data\": {" +
                "\"Pos\": {" + _using.profile.xPos + "," + _using.profile.yPos + "," + _using.profile.zPos + "}, " +
                "\"BodyC\": {" + _using.profile.colorBody.r + "," + _using.profile.colorBody.g + "," + _using.profile.colorBody.b + "}, " +
                "\"EngineC\": {" + _using.profile.colorEngine.r + "," + _using.profile.colorEngine.g + "," + _using.profile.colorEngine.b + "}, " +
                "\"BinderC\": {" + _using.profile.colorBinder.r + "," + _using.profile.colorBinder.g + "," + _using.profile.colorBinder.b + "}, " +
                "\"Score\": {" + _using.profile.score + "}" +
            "}";
        

        // Set SyncID from server.
        if ( _using.profile.currentSyncUpdate < _sync.GetSyncID())
            _using.profile.currentSyncUpdate = _sync.GetSyncID();

        // SyncData.
        _sync.SyncData ( sendData, _using.profile.currentSyncUpdate );

        
        _using.profile.currentSyncUpdate++;

        // Print debug.
        _using.game.DebugConsole("Save!!");
        _using.game.DebugConsole("Sync Update : " + _using.profile.currentSyncUpdate);
        _using.game.DebugConsole("Position : " + new Vector3(_using.profile.xPos, _using.profile.yPos, _using.profile.zPos));
        _using.game.DebugConsole("Score : " + _using.profile.score);
        
    }

    public override void Load ( )
    {
        _using.profile.currentSyncUpdate = _sync.GetSyncID();
        
        //SendData da = JsonUtility.FromJson<SendData>( _sync.GetSyncData( ) );
        //LoadColor(da.engineC, da.binderC,da.bodyC);

        // Set score.
        //_using.game.SetScore(da.score);

        // Set position. (rotation)
        //_using.player.MoveToRotation(da.pos);

        string input = _sync.GetSyncData();
        input = input.Substring(9);
        input = input.Substring(0, input.Length - 1);
        
        // Decode json.
        foreach (Match itemMatch in regex.Matches(input))
        {
            string name = itemMatch.Groups[1].ToString();
            string val = itemMatch.Groups[2].ToString();
            //Debug.Log(name + " -> " + val);

            if (name.Equals("Pos"))
            {
                // sync position (rotation)
                string[] words = val.Split(',');
                _using.profile.xPos = float.Parse(words[0]);
                _using.profile.yPos = float.Parse(words[1]);
                _using.profile.zPos = float.Parse(words[2]);
            }
            else if (name.Equals("BodyC"))
            {
                // sync body color.
                string[] words = val.Split(',');
                _using.profile.colorBody = new Color(float.Parse(words[0]), float.Parse(words[1]), float.Parse(words[2]));
            }
            else if (name.Equals("EngineC"))
            {
                // sync engine color.
                string[] words = val.Split(',');
                _using.profile.colorEngine = new Color(float.Parse(words[0]), float.Parse(words[1]), float.Parse(words[2]));
            }
            else if (name.Equals("BinderC"))
            {
                // sync binder color.
                string[] words = val.Split(',');
                _using.profile.colorBinder = new Color(float.Parse(words[0]), float.Parse(words[1]), float.Parse(words[2]));
            }
            else if (name.Equals("Score"))
            {
                // sync score color.
                _using.profile.score = int.Parse(val);
            }
        }
        
        // Set color ship.
        _using.profile.LoadColor( 
                new Color ( _using.profile.colorEngine.r, _using.profile.colorEngine.g, _using.profile.colorEngine.b ),
                new Color ( _using.profile.colorBinder.r, _using.profile.colorBinder.g, _using.profile.colorBinder.b ),
                new Color ( _using.profile.colorBody.r, _using.profile.colorBody.g, _using.profile.colorBody.b )
                );

        // Set score.
        _using.game.SetScore ( _using.profile.score );

        // Set position. (rotation)
        _using.player.MoveToRotation ( new Vector3 ( _using.profile.xPos, _using.profile.yPos, _using.profile.zPos ) );
        
        // Print debug to option
        _using.game.DebugConsole ( "Load!!" );
        _using.game.DebugConsole ( "Sync Update : " + _using.profile.currentSyncUpdate );
        _using.game.DebugConsole ( "Position : " + new Vector3 ( _using.profile.xPos, _using.profile.yPos, _using.profile.zPos ) );
        _using.game.DebugConsole ( "Score : " + _using.profile.score );
    }
}
