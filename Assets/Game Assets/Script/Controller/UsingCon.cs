using UnityEngine;
using System.Collections;

/// <summary>
/// Use in gameobject name _UsingController.
/// Find all controller prepare use it.
/// </summary>
public class UsingCon : MonoBehaviour 
{
    public GameController game;
    public ProfileData profile;
    public APlayerInput player;
    public ALoginFactory login;
    public MathHelper mathHelp;

	void Awake () 
    {
        game = GameObject.FindObjectOfType<GameController>();
        player = GameObject.FindObjectOfType<APlayerInput>();
        profile = GameObject.FindObjectOfType<ProfileData>();
        mathHelp = GameObject.FindObjectOfType<MathHelper>();
        
        //login = GameObject.FindObjectOfType<ALoginFactory>();

#if ( UNITY_EDITOR || UNITY_STANDALONE || UNITY_ANDROID) 
        login = GameObject.FindObjectOfType < PlayerPrefLogin > ( );
#endif
        
//#if ( UNITY_ANDROID || UNITY_IPHONE )
//        login = GameObject.FindObjectOfType < FacebookLogin > ( );
//#endif
	}
}
