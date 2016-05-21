using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// User for select username and delete username.
/// Get & Set data from ArrayPrefs. ( PlayerPrefs )
/// Control camera for detect qr code.
/// </summary>
public class SelectUser : MonoBehaviour 
{
    /// <summary>
    /// prefab UI list view. 
    /// </summary>
    public GameObject prefab;

    /// <summary>
    /// Parent of list view.
    /// </summary>
    public Transform parent;

    /// <summary>
    /// Current username on player selected.
    /// </summary>
    public string username;

    /// <summary>
    /// Header show username player selected.
    /// </summary>
    public Text header;

    /// <summary>
    /// Panel of webcam. 
    /// Image for render webcam texture.
    /// </summary>
    public GameObject webcam;

    /// <summary>
    /// Show log debug something.
    /// </summary>
    public Text debugText;

    private string[] arr_UserName;
    
    /// <summary>
    /// Postion and size UI list view.
    /// </summary>
    public const float DEFAULT_POSY = -15;
    public const float NEXT_POS = 30;
    public const float HEIGHT = 30;

    private enum KeyType
    {
        STRING = 0,
        INT = 1,
        VECTOR3 = 2,
        COLOR = 3,
    }

    private enum ColorType
    {
        NULL = 0,
        BINDER = 1,
        BODY = 2,
        ENGINE = 3,
    }

    public enum PlayerPrefsKeys
    {
        USERNAME = 0,
        SCORE = 1, 
        POSITION = 2,
        ENGINE_COLOR = 3,
        BINDER_COLOR = 4,
        BODY_COLOR = 5,
    }

    void Start ( )
    {
        Draw_ListView ( );
    }

    /// <summary>
    /// Set current username on player selected.
    /// </summary>
    /// <param name="username"></param>
    public void SetCurrentUser ( string username )
    {
        this.username = username;
        PlayerPrefs.SetInt ( "CurrentDataIndex", CheckIndex ( this.username ) );
        header.text = "Your select : " + this.username;
    }

    /// <summary>
    /// Create new user.
    /// </summary>
    /// <param name="user"></param>
    public void CreateUser ( Text user )
    {
        if ( user.text != "" )
        {
            if ( PlayerPrefs.HasKey ( PlayerPrefsKeys.USERNAME.ToString ( ) ) )
                NewUser ( user.text );
            else
                FirstUser ( user.text );

            Draw_ListView ( );
        }

    }

    /// <summary>
    /// Get list username from ArrayPrefs.
    /// </summary>
    /// <returns></returns>
    public List<string> GetStringList ( )
    {
        string[] arr = PlayerPrefsX.GetStringArray ( PlayerPrefsKeys.USERNAME.ToString ( ) );
        List<string> list = new List<string>();
        list.AddRange ( arr );
        return list;
    }

    /// <summary>
    /// Set list username to ArrayPrefs.
    /// </summary>
    /// <param name="list"></param>
    public void SetStringArr ( List<string> list )
    {
        PlayerPrefsX.SetStringArray ( PlayerPrefsKeys.USERNAME.ToString ( ), list.ToArray ( ) );
    }

    /// <summary>
    /// Get list score from ArrayPrefs.
    /// </summary>
    /// <returns></returns>
    public List<int> GetIntList ( )
    {
        int[] arr = PlayerPrefsX.GetIntArray ( PlayerPrefsKeys.SCORE.ToString ( ) );
        List<int> list = new List<int>();
        list.AddRange ( arr );
        return list;
    }

    /// <summary>
    /// Set list score to ArrayPrefs.
    /// </summary>
    /// <param name="list"></param>
    public void SetIntArr ( List<int> list )
    {
        PlayerPrefsX.SetIntArray ( PlayerPrefsKeys.SCORE.ToString ( ), list.ToArray ( ) );
    }

    /// <summary>
    /// Get list position from ArrayPrefs.
    /// </summary>
    /// <returns></returns>
    public List<Vector3> GetVec3List ( )
    {
        Vector3[] arr = PlayerPrefsX.GetVector3Array ( PlayerPrefsKeys.POSITION.ToString ( ) );
        List<Vector3> list = new List<Vector3>();
        list.AddRange ( arr );
        return list;
    }

    /// <summary>
    /// Set list position to ArrayPrefs.
    /// </summary>
    /// <param name="list"></param>
    public void SetVec3Arr ( List<Vector3> list )
    {
        PlayerPrefsX.SetVector3Array ( PlayerPrefsKeys.POSITION.ToString ( ), list.ToArray ( ) );
    }

    /// <summary>
    /// Get list color from ArrayPrefs.
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public List<Color> GetColorList ( PlayerPrefsKeys t )
    {
        Color[] arr = PlayerPrefsX.GetColorArray ( t.ToString ( ) );
        List<Color> list = new List<Color>();
        list.AddRange ( arr );
        return list;
    }

    /// <summary>
    /// Set list color to ArrayPrefs.
    /// </summary>
    /// <param name="list"></param>
    /// <param name="t"></param>
    public void SetColorArr ( List<Color> list, PlayerPrefsKeys t )
    {
        PlayerPrefsX.SetColorArray ( t.ToString ( ), list.ToArray ( ) );
    }

    /// <summary>
    /// Draw UI list view.
    /// </summary>
    public void Draw_ListView ( )
    {
        CleanChild ( );

        arr_UserName = PlayerPrefsX.GetStringArray ( PlayerPrefsKeys.USERNAME.ToString ( ) );
        //print( "Count of array : " + arr_UserName.Length );
        float posY = DEFAULT_POSY;
        for ( int i = 0; i < arr_UserName.Length; i++ )
        {
            GameObject obj = Instantiate ( prefab );
            obj.transform.SetParent ( parent );
            obj.GetComponent<RectTransform>().anchoredPosition = new Vector2 ( 0, posY );
            obj.GetComponent<RectTransform>().sizeDelta = new Vector2 ( 0, 30 );
            obj.transform.FindChild ( "UserName" ).FindChild ( "Text" ).GetComponent<Text>().text = arr_UserName[i];
            posY -= NEXT_POS;

            //print ("Loop count : " + i );
        }
    }

    /// <summary>
    /// Delete all keys PlayerPrefs.
    /// </summary>
    public void DeleteAll ( )
    {
        PlayerPrefs.DeleteAll ( );
        Draw_ListView ( );
    }

    /// <summary>
    /// Clean UI list view before draw.
    /// </summary>
    public void CleanChild ( )
    {
        if ( parent.childCount > 0 )
            foreach ( Transform child in parent )
                Destroy ( child.gameObject );
    }

    /// <summary>
    /// Delete one PlayerPrefs on selected.
    /// </summary>
    /// <param name="text"></param>
    public void Delete ( string text )
    {
        arr_UserName = PlayerPrefsX.GetStringArray ( PlayerPrefsKeys.USERNAME.ToString ( ) );
        int index = 0;
        for ( int i = 0; i < arr_UserName.Length; i++ )
        {
            if ( arr_UserName[i].Equals ( text ) )
            {
                index = i;
                Del_Process ( index );
                Draw_ListView ( );

                if ( username == text )
                {
                    username = "";
                    PlayerPrefs.DeleteKey ( "CurrentDataIndex" );
                    header.text = "Select user....";
                }
                break;
            }
        }
    }

    /// <summary>
    /// Delete all key in same index with username.
    /// </summary>
    /// <param name="i"></param>
    private void Del_Process ( int i )
    {
        try
        { 
            Del_Filter ( PlayerPrefsKeys.USERNAME.ToString ( ), KeyType.STRING, ColorType.NULL, i );
            Del_Filter ( PlayerPrefsKeys.SCORE.ToString ( ), KeyType.INT, ColorType.NULL, i );
            Del_Filter ( PlayerPrefsKeys.POSITION.ToString ( ), KeyType.VECTOR3, ColorType.NULL, i );
            Del_Filter ( PlayerPrefsKeys.BINDER_COLOR.ToString ( ), KeyType.COLOR, ColorType.BINDER, i );
            Del_Filter ( PlayerPrefsKeys.BODY_COLOR.ToString ( ), KeyType.COLOR, ColorType.BODY, i );
            Del_Filter ( PlayerPrefsKeys.ENGINE_COLOR.ToString ( ), KeyType.COLOR, ColorType.ENGINE, i );
        }
        catch
        {

        }

    }

    /// <summary>
    /// Get data in ArrayPrefs for delete.
    /// </summary>
    /// <param name="playerPrefsKey"></param>
    /// <param name="type"></param>
    /// <param name="color"></param>
    /// <param name="index"></param>
    private void Del_Filter ( string playerPrefsKey, KeyType type, ColorType color, int index )
    {
        if ( type == KeyType.INT && color == ColorType.NULL )
        {
            int[] arr = PlayerPrefsX.GetIntArray ( playerPrefsKey );
            Del ( playerPrefsKey, arr, index );
        }
        else if ( type == KeyType.STRING && color == ColorType.NULL )
        {
            string[] arr = PlayerPrefsX.GetStringArray ( playerPrefsKey );
            Del ( playerPrefsKey, arr, index );
        }
        else if ( type == KeyType.VECTOR3 && color == ColorType.NULL )
        {
            Vector3[] arr = PlayerPrefsX.GetVector3Array ( playerPrefsKey );
            Del ( playerPrefsKey, arr, index );
        }
        else if ( type == KeyType.COLOR && color != ColorType.NULL )
        {
            Color[] arr = PlayerPrefsX.GetColorArray ( playerPrefsKey );
            Del ( playerPrefsKey, arr, color, index );
        }
    }
    
    /// <summary>
    /// Remove score data index in ArrayPrefs. 
    /// </summary>
    /// <param name="playerPrefsKey"></param>
    /// <param name="interger"></param>
    /// <param name="index"></param>
    private void Del ( string playerPrefsKey, int[] interger, int index )
    {
        List<int> list = new List<int> ( );
        list.AddRange ( interger );
        list.RemoveAt ( index );

        PlayerPrefsX.SetIntArray ( playerPrefsKey, list.ToArray ( ) );
    }
    
    /// <summary>
    /// Remove username data index in ArrayPrefs.
    /// </summary>
    /// <param name="playerPrefsKey"></param>
    /// <param name="s"></param>
    /// <param name="index"></param>
    private void Del ( string playerPrefsKey, string[] s, int index )
    {
        List<string> list = new List<string> ( );
        list.AddRange ( s );
        list.RemoveAt ( index );

        PlayerPrefsX.SetStringArray ( playerPrefsKey, list.ToArray ( ) );
    }
    
    /// <summary>
    /// Remove postion data index in ArrayPrefs.
    /// </summary>
    /// <param name="playerPrefsKey"></param>
    /// <param name="vec"></param>
    /// <param name="index"></param>
    private void Del ( string playerPrefsKey, Vector3[] vec, int index )
    {
        List<Vector3> list = new List<Vector3> ( );
        list.AddRange ( vec );
        list.RemoveAt ( index );

        PlayerPrefsX.SetVector3Array ( playerPrefsKey, list.ToArray ( ) );
    }
    
    /// <summary>
    /// Remove color data index in ArrayPrefs.
    /// </summary>
    /// <param name="playerPrefsKey"></param>
    /// <param name="c"></param>
    /// <param name="t"></param>
    /// <param name="index"></param>
    private void Del ( string playerPrefsKey, Color[] c, ColorType t, int index )
    {
        List<Color> list = new List<Color> ( );
        list.AddRange ( c );
        list.RemoveAt ( index );

        PlayerPrefsX.SetColorArray ( playerPrefsKey, list.ToArray ( ) );
    }

    /// <summary>
    /// Check username index.
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    private int CheckIndex ( string s )
    {
        int index;
        arr_UserName = PlayerPrefsX.GetStringArray ( PlayerPrefsKeys.USERNAME.ToString ( ) );
        for ( int i = 0; i < arr_UserName.Length; i++ )
        {
            if ( arr_UserName[i].Equals ( s ) )
            {
                index = i;
                return index;
            }
        }
        return -1;
    }

    /// <summary>
    /// Open camera.
    /// </summary>
    public void OpenCam ( )
    {
        if ( username != "" )
        {
            webcam.SetActive ( true );
        }
        else
        { 
            debugText.text = "Select user!!";
            Invoke ( "ChangeTextDebug", 2 );
        }
    }

    private void ChangeTextDebug ( )
    {
        debugText.text = "";
    }

    /// <summary>
    /// Close camera.
    /// </summary>
    public void CloseCam ( )
    {
        webcam.SetActive ( false );
    }

    /// <summary>
    /// In case add first user
    /// This script will create new key.
    /// </summary>
    /// <param name="str"></param>
    private void FirstUser ( string str )
    {
        List<string> s = new List<string>();
        s.Add ( str );
        SetStringArr ( s );

        List<int> i = new List<int>();
        i.Add ( 0 );
        SetIntArr ( i );

        List<Vector3> vec = new List<Vector3>();
        vec.Add ( new Vector3 ( 0, 0, 0 ) );
        SetVec3Arr ( vec );

        List<Color> b = new List<Color>();
        b.Add ( new Color ( 1, 0, 0 ) );
        SetColorArr ( b, PlayerPrefsKeys.BODY_COLOR );

        List<Color> bin = new List<Color>();
        bin.Add ( new Color ( 0.5f, 0.5f, 0 ) );
        SetColorArr ( bin, PlayerPrefsKeys.BINDER_COLOR );

        List<Color> e = new List<Color>();
        e.Add ( new Color ( 0, 0, 0.5f ) );
        SetColorArr ( e, PlayerPrefsKeys.ENGINE_COLOR );
    }

    /// <summary>
    /// In case not first user.
    /// This script will get data from ArrayPrefs and add new data to list.
    /// </summary>
    /// <param name="str"></param>
    private void NewUser ( string str )
    {
        List<string> s = GetStringList ( );
        s.Add ( str );
        SetStringArr ( s );

        List<int> i = GetIntList ( );
        i.Add ( 0 );
        SetIntArr ( i );

        List<Vector3> vec = GetVec3List ( );
        vec.Add ( new Vector3 ( 0, 0, 0 ) );
        SetVec3Arr ( vec );

        List<Color> b = GetColorList ( PlayerPrefsKeys.BODY_COLOR );
        b.Add ( new Color ( 1, 0, 0 ) );
        SetColorArr ( b, PlayerPrefsKeys.BODY_COLOR );

        List<Color> bin = GetColorList ( PlayerPrefsKeys.BINDER_COLOR );
        bin.Add ( new Color ( 0.5f, 0.5f, 0 ) );
        SetColorArr ( bin, PlayerPrefsKeys.BINDER_COLOR );

        List<Color> e = GetColorList ( PlayerPrefsKeys.ENGINE_COLOR );
        e.Add ( new Color ( 0, 0, 0.5f ) );
        SetColorArr ( e, PlayerPrefsKeys.ENGINE_COLOR );
    }

}
