using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text.RegularExpressions;

/// <summary>
/// Use in GameObject name GameController.
/// Profile data for save data from database and load too. (It like a bridge) 
/// </summary>
public class ProfileData : MonoBehaviour
{
    /// <summary>
    /// if true draw GUI on top screen. (Save, Load)
    /// </summary>
    public bool showDebugGUI;

    public bool loadData;
    public bool saveData;

    /// <summary>
    /// Json pattern decode.
    /// </summary>
    private string sPattern = "\"(\\w+)\": {(\\d+|[\\d\\.]+,[\\d\\.]+,[\\d\\.]+)}";
    private Regex regex;

    /// <summary>
    /// vector3 for stroe rotation player.
    /// </summary>
    public float xPos;
    public float yPos;
    public float zPos;
   
    /// <summary>
    /// score.
    /// </summary>
    public int score;

    /// <summary>
    /// Color of _ColorG, _ColorB, _ColorR
    /// </summary>
    public Color colorEngine, colorBinder, colorBody;

    /// <summary>
    /// Image for get material.
    /// </summary>
    public Image ship;

    /// <summary>
    /// Image when you sync.
    /// </summary>
    public Image syncScreen;

    /// <summary>
    /// Text on you syncing.
    /// </summary>
    public Text textSyncing;

    /// <summary>
    /// Save passcode for show in option and Draw QR code. (Playerprefs sync)
    /// </summary>
    public string passCode = "";

    /// <summary>
    /// Material of ship.
    /// </summary>
    public Material playerMat;

    /// <summary>
    /// Number for count Sync ID in database (Facebook sync)
    /// </summary>
    public int currentSyncUpdate = 1;

    private UsingCon _using;

    private Game _sync;

    private bool _waitForLoad = false;
    private bool _fakeSync;

    private float _AlphaSync = 1;

    public const float ALPHA_MAX = 255;
    public const float ALPHA_MIN = 0;

    public float alpha = 0;

    /// <summary>
    /// Fake save for pseudo database. (facebook sync) 
    /// </summary>
    public void FetchLoad()
    {
        _waitForLoad = true;
    }

//    public void Load ( )
//    {
//#if ( UNITY_ANDROID || UNITY_IPHONE )
//        FetchLoad ( );
//#endif

//#if ( UNITY_EDITOR || UNITY_STANDALONE ) 
//        LoadDataFromPlayerPrefs ( );
//#endif
//    }

    void Awake()
    {
        regex = new Regex(sPattern, RegexOptions.Compiled);
        _using = GameObject.FindObjectOfType<UsingCon>();

        _sync = gameObject.GetComponent<Game>();

        playerMat = ship.material;
    }

    //void Start ( )
    //{
    //    _using.game.SetGamePause ( true );
    //    syncScreen.raycastTarget = true;
    //    syncScreen.color = new Color ( 0, 0, 0, 1 );
    //    textSyncing.color = new Color ( 1, 1, 1, 1 );
    //}

    void Update()
    {
        //syncScreen.color = new Color ( 0, 0, 0, alpha );
        if (_waitForLoad)
        {
            // First time on start scene call savedata for change server state to done.
            if (_sync.IsReady())
            {
                _fakeSync = true;
                SaveData(true);
                _fakeSync = false;
            }
            else if ( _sync.IsSyncing( ) )
            {
                //alpha += Time.deltaTime;
                //print( "Delay sync : " + alpha );
            }
            // When server state is done call load data.
            else if (_sync.IsDone())
            {
                _using.login.Load ( );
                _waitForLoad = false;

                StartCoroutine ( FadeOutSyncScreen ( ) );
            }
        }
    }
    
    /// <summary>
    /// Fade out image after facebook sync success.
    /// </summary>
    /// <returns></returns>
    IEnumerator FadeOutSyncScreen ( )
    {
        yield return new WaitForSeconds( 0 );
        if ( _AlphaSync > 0)
        { 
            _AlphaSync -= Time.deltaTime / 4;
            //print ( _AlphaSync );
            _AlphaSync = Mathf.Clamp( _AlphaSync, ALPHA_MIN, ALPHA_MAX );
            syncScreen.color = new Color ( 0, 0, 0, _AlphaSync );
            textSyncing.color = new Color ( 1, 1, 1, _AlphaSync );

            StartCoroutine ( FadeOutSyncScreen ( ) );
        }
        else
        {
            _using.game.SetGamePause ( false );  
            syncScreen.raycastTarget = false;
            textSyncing.gameObject.SetActive ( false );
            StopCoroutine ( FadeOutSyncScreen ( ) );
        }
    }

    void OnValidate()
    {
        if (loadData)
        {
            _using.login.Load ( );
            loadData = false;
        }
        if (saveData)
        {
            _using.login.Save ( );
            saveData = false;
        }
    }

    void OnGUI()
    {
        if (showDebugGUI)
        {
            // draw debug button Save.
            if (GUI.Button(new Rect(0, 0, 50, 25), "Save"))
            {
                _using.login.Save ( );

//#if ( UNITY_EDITOR || UNITY_STANDALONE )
//                SaveDataToPlayerPrefs ( );   
//#endif

//#if ( UNITY_ANDROID || UNITY_IPHONE )
//                SaveData ( );
//#endif
                GameObject.FindObjectOfType<FindArea> ( ).CreateSaveRing ( );
            }

            // draw debug button Load.
            if (GUI.Button(new Rect(50, 0, 50, 25), "Load"))
            {
                _using.login.Load ( );
//#if ( UNITY_EDITOR || UNITY_STANDALONE )
//                LoadDataFromPlayerPrefs ( );
//#endif

//#if ( UNITY_ANDROID || UNITY_IPHONE )
//                LoadData ( );
//#endif
            }
            // draw debug button exclude rotate supply.
            if (GUI.Button(new Rect(100, 0, 50, 25), (RotateSupply.isOn) ? "rotate" : "not rotate"))
                RotateSupply.isOn = !RotateSupply.isOn;

            // draw debug button close input multi touch.
            if (GUI.Button(new Rect(200, 0, 100, 25), (Input.multiTouchEnabled) ? "multiTouch" : "not multiTouch"))
                Input.multiTouchEnabled = !Input.multiTouchEnabled;
        }
    }

    /// <summary>
    /// Stroe player color in value or database.
    /// </summary>
    public void SaveColor()
    {
        colorEngine = playerMat.GetColor("_ColorG");
        colorBinder = playerMat.GetColor("_ColorB");
        colorBody = playerMat.GetColor("_ColorR");
    }

    /// <summary>
    /// Set player color from value or database.
    /// </summary>
    public void LoadColor ( Color c1, Color c2, Color c3 )
    {
        // color engine.
        playerMat.SetColor("_ColorG", c1);

        // color Binder.
        playerMat.SetColor("_ColorB", c2);

        // color body.
        playerMat.SetColor("_ColorR", c3);
    }

    /// <summary>
    /// For load data from server
    /// </summary>
    //public void LoadData()
    //{
    //    currentSyncUpdate = _sync.GetSyncID();
        
    //    //SendData da = JsonUtility.FromJson<SendData>( _sync.GetSyncData( ) );
    //    //LoadColor(da.engineC, da.binderC,da.bodyC);

    //    // Set score.
    //    //_using.game.SetScore(da.score);

    //    // Set position. (rotation)
    //    //_using.player.MoveToRotation(da.pos);

    //    string input = _sync.GetSyncData();
    //    input = input.Substring(9);
    //    input = input.Substring(0, input.Length - 1);
        
    //    // Decode json.
    //    foreach (Match itemMatch in regex.Matches(input))
    //    {
    //        string name = itemMatch.Groups[1].ToString();
    //        string val = itemMatch.Groups[2].ToString();
    //        //Debug.Log(name + " -> " + val);

    //        if (name.Equals("Pos"))
    //        {
    //            // sync position (rotation)
    //            string[] words = val.Split(',');
    //            xPos = float.Parse(words[0]);
    //            yPos = float.Parse(words[1]);
    //            zPos = float.Parse(words[2]);
    //        }
    //        else if (name.Equals("BodyC"))
    //        {
    //            // sync body color.
    //            string[] words = val.Split(',');
    //            colorBody = new Color(float.Parse(words[0]), float.Parse(words[1]), float.Parse(words[2]));
    //        }
    //        else if (name.Equals("EngineC"))
    //        {
    //            // sync engine color.
    //            string[] words = val.Split(',');
    //            colorEngine = new Color(float.Parse(words[0]), float.Parse(words[1]), float.Parse(words[2]));
    //        }
    //        else if (name.Equals("BinderC"))
    //        {
    //            // sync binder color.
    //            string[] words = val.Split(',');
    //            colorBinder = new Color(float.Parse(words[0]), float.Parse(words[1]), float.Parse(words[2]));
    //        }
    //        else if (name.Equals("Score"))
    //        {
    //            // sync score color.
    //            score = int.Parse(val);
    //        }
    //    }
        
    //    // Set color ship.
    //    LoadColor( 
    //            new Color (colorEngine.r, colorEngine.g, colorEngine.b),
    //            new Color (colorBinder.r, colorBinder.g, colorBinder.b),
    //            new Color (colorBody.r, colorBody.g, colorBody.b)
    //            );

    //    // Set score.
    //    _using.game.SetScore(score);

    //    // Set position. (rotation)
    //    _using.player.MoveToRotation(new Vector3(xPos, yPos, zPos));
        
    //    // Print debug to option
    //    _using.game.DebugConsole("Load!!");
    //    _using.game.DebugConsole("Sync Update : " + currentSyncUpdate);
    //    _using.game.DebugConsole("Position : " + new Vector3(xPos, yPos, zPos));
    //    _using.game.DebugConsole("Score : " + score);

    //}

    //public void LoadDataFromPlayerPrefs ( )
    //{
    //    //print ( PlayerPrefs.GetString ( "UserName" ) + ", " + PlayerPrefs.HasKey ( PlayerPrefs.GetString ( "UserName" ) + "_Score" ));

    //    // if this is new user
    //    if ( !PlayerPrefs.HasKey ( PlayerPrefs.GetString ( "UserName" ) + "_Score" ) )
    //    {
    //        SyncToPlayerPrefs ( _using.game.GetScore ( ) );
    //    }

    //    score = PlayerPrefs.GetInt ( PlayerPrefs.GetString ( "UserName" ) + "_Score" );

    //    // Set score.
    //    _using.game.SetScore(score);

    //    xPos = PlayerPrefs.GetFloat ( PlayerPrefs.GetString ( "UserName" ) + "_PosX" );
    //    yPos = PlayerPrefs.GetFloat ( PlayerPrefs.GetString ( "UserName" ) + "_PosY" );
    //    zPos = PlayerPrefs.GetFloat ( PlayerPrefs.GetString ( "UserName" ) + "_PosZ" );
        
    //    // Set position. (rotation)
    //    _using.player.MoveToRotation(new Vector3(xPos, yPos, zPos));

    //    // Set color ship.
    //    LoadColor   ( 
    //            new Color   (   PlayerPrefs.GetFloat ( PlayerPrefs.GetString ( "UserName" ) + "_EngineColor_R" ), 
    //                            PlayerPrefs.GetFloat ( PlayerPrefs.GetString ( "UserName" ) + "_EngineColor_G" ), 
    //                            PlayerPrefs.GetFloat ( PlayerPrefs.GetString ( "UserName" ) + "_EngineColor_B" )
    //                        ),
    //            new Color   (   PlayerPrefs.GetFloat ( PlayerPrefs.GetString ( "UserName" ) + "_BinderColor_R" ), 
    //                            PlayerPrefs.GetFloat ( PlayerPrefs.GetString ( "UserName" ) + "_BinderColor_G" ), 
    //                            PlayerPrefs.GetFloat ( PlayerPrefs.GetString ( "UserName" ) + "_BinderColor_R" )
    //                        ),
    //            new Color   (   PlayerPrefs.GetFloat ( PlayerPrefs.GetString ( "UserName" ) + "_BodyColor_R" ),
    //                            PlayerPrefs.GetFloat ( PlayerPrefs.GetString ( "UserName" ) + "_BodyColor_G" ), 
    //                            PlayerPrefs.GetFloat ( PlayerPrefs.GetString ( "UserName" ) + "_BodyColor_B" )
    //                        )
    //                );

    //    // Print debug to option
    //    _using.game.DebugConsole( "Load!! : Use PlayerPrefs" );
    //    _using.game.DebugConsole( "Position : " + new Vector3 ( xPos, yPos, zPos ) );
    //    _using.game.DebugConsole( "Score : " + score );
    //}

    //public void SaveDataToPlayerPrefs ( )
    //{
    //    score = _using.game.GetScore ( );
    //    SyncToPlayerPrefs ( score );

    //    // Print debug to option
    //    _using.game.DebugConsole( "Save!! : To PlayerPrefs" );
    //    _using.game.DebugConsole( "Position : " + new Vector3 ( xPos, yPos, zPos ) );
    //    _using.game.DebugConsole( "Score : " + score );
    //}

    //public void SyncToPlayerPrefs ( int score )
    //{
    //    // Score
    //    PlayerPrefs.SetInt ( PlayerPrefs.GetString ( "UserName" ) + "_Score", score );
            
    //    // Position
    //    xPos = _using.game.GetRotation().x;
    //    yPos = _using.game.GetRotation().y;
    //    zPos = _using.game.GetRotation().z;

    //    PlayerPrefs.SetFloat ( PlayerPrefs.GetString ( "UserName" ) + "_PosX", xPos );
    //    PlayerPrefs.SetFloat ( PlayerPrefs.GetString ( "UserName" ) + "_PosY", yPos );
    //    PlayerPrefs.SetFloat ( PlayerPrefs.GetString ( "UserName" ) + "_PosZ", zPos );
            
    //    // save current color from ship
    //    SaveColor ( );

    //    // Body color
    //    PlayerPrefs.SetFloat ( PlayerPrefs.GetString ( "UserName" ) + "_BodyColor_R", colorBody.r );
    //    PlayerPrefs.SetFloat ( PlayerPrefs.GetString ( "UserName" ) + "_BodyColor_G", colorBody.g );
    //    PlayerPrefs.SetFloat ( PlayerPrefs.GetString ( "UserName" ) + "_BodyColor_B", colorBody.b );
            
    //    // Engine color
    //    PlayerPrefs.SetFloat ( PlayerPrefs.GetString ( "UserName" ) + "_EngineColor_R", colorEngine.r );
    //    PlayerPrefs.SetFloat ( PlayerPrefs.GetString ( "UserName" ) + "_EngineColor_G", colorEngine.g );
    //    PlayerPrefs.SetFloat ( PlayerPrefs.GetString ( "UserName" ) + "_EngineColor_B", colorEngine.b );

    //    // Binder color
    //    PlayerPrefs.SetFloat ( PlayerPrefs.GetString ( "UserName" ) + "_BinderColor_R", colorBinder.r );
    //    PlayerPrefs.SetFloat ( PlayerPrefs.GetString ( "UserName" ) + "_BinderColor_G", colorBinder.g );
    //    PlayerPrefs.SetFloat ( PlayerPrefs.GetString ( "UserName" ) + "_BinderColor_B", colorBinder.b );
    //}

    public bool SaveData()
    {
        return SaveData(false);
    }

    /// <summary>
    /// For save data into value and put it to the server
    /// </summary>
    public bool SaveData(bool force)
    {
        //if we still waiting for the first sync we will not save.
        if (!force && _waitForLoad)
            return false;

        xPos = _using.game.GetRotation().x;
        yPos = _using.game.GetRotation().y;
        zPos = _using.game.GetRotation().z;
        SaveColor();
        score = _using.game.GetScore();

        //SendData da = new SendData();
        //da.pos = new Vector3(xPos,yPos,zPos);
        //da.bodyC = colorBody;
        //da.engineC = colorEngine;
        //da.binderC = colorBinder;
        //da.score = score;
        
        //string sendData =  JsonUtility.ToJson(da);

        // Encode json.
        string sendData =
            "\"Data\": {" +
                "\"Pos\": {" + xPos + "," + yPos + "," + zPos + "}, " +
                "\"BodyC\": {" + colorBody.r + "," + colorBody.g + "," + colorBody.b + "}, " +
                "\"EngineC\": {" + colorEngine.r + "," + colorEngine.g + "," + colorEngine.b + "}, " +
                "\"BinderC\": {" + colorBinder.r + "," + colorBinder.g + "," + colorBinder.b + "}, " +
                "\"Score\": {" + score + "}" +
            "}";
        

        // Set SyncID from server.
        if (currentSyncUpdate < _sync.GetSyncID())
            currentSyncUpdate = _sync.GetSyncID();

        // SyncData.
        _sync.SyncData(sendData, currentSyncUpdate);

        
        currentSyncUpdate++;

        if (!_fakeSync)
        {
            // Print debug.
            _using.game.DebugConsole("Save!!");
            _using.game.DebugConsole("Sync Update : " + currentSyncUpdate);
            _using.game.DebugConsole("Position : " + new Vector3(xPos, yPos, zPos));
            _using.game.DebugConsole("Score : " + score);
            _using.game.DebugConsole("Code : " + _using.mathHelp.MakePass ( colorBody, colorBinder, colorEngine, new Vector3 ( xPos, yPos, zPos ), score ) );
        }

        return true;
    }
}
