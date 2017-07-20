using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// When create new list view set unity event into it.
/// </summary>
public class OnStartListView : MonoBehaviour 
{

    public Button listView;
    public Text listView_text;
    public Button del;
    
    private SelectUser su;

	// Use this for initialization
	void Start () 
    {
        su = GameObject.FindObjectOfType<SelectUser> ( );
        
        // Add unity event on click in button. 
	    listView.onClick.AddListener ( ( ) => su.SetCurrentUser ( listView_text.text ) );
        del.onClick.AddListener ( ( ) => su.Delete ( listView_text.text ) );
	}
}
