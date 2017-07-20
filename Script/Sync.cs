using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/*
json
****

{ 
    Player_facebook_id: 'a7664093-502e-4d2b-bf30-25a2b26d6021',
	Player_facebook_token: 'a7664093-502e-4d2b-bf30-25a2b26d6021',
    Player_update_sync:56,
    Player_data: 'sfsdsf'
}
*/

public struct Data
{
    public string Player_facebook_id, Player_facebook_token, Player_data;
    public int Player_update_sync;

    public Data(string player_facebook_id, string player_facebook_token, string player_data, int player_update_sync)
    {
        Player_facebook_id = player_facebook_id;
        Player_facebook_token = player_facebook_token;
        Player_data = player_data;
        Player_update_sync = player_update_sync;
    }
}

public struct DataReturned
{
    public string Result, Message, Player_data;
    public int Player_update_sync;

    public DataReturned(string result, string message, string player_data, int player_update_sync)
    {
        Result = result;
        Message = message;
        Player_data = player_data;
        Player_update_sync = player_update_sync;
    }
}

public class Sync  : MonoBehaviour {

    public string IP = "52.77.61.245";
    public string Result, Message, Player_facebook_id, Player_facebook_token,Player_data;
    public int Player_update_sync;
    public DataReturned datareturned;

    public enum SyncStatus { Ready, Syncing, Done };
    public SyncStatus syncstatus;

    public void SyncProfile()
    {
        Data data = new Data(Player_facebook_id, Player_facebook_token, Player_data, Player_update_sync);

        // Start sync
        syncstatus = SyncStatus.Syncing;
        StartCoroutine(StarSync(data));
    }

    public void SyncProfile(Data data)
    {
        // Start sync
        syncstatus = SyncStatus.Syncing;
        StartCoroutine(StarSync(data));
    }

    IEnumerator StarSync(Data data)
    {
        string url = "http://" + IP + "/PlayerAPI.php";

        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("Content-Type", "application/json");

        string json = JsonUtility.ToJson(data);
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(json);

        WWW www = new WWW(url, bytes, headers);
        yield return www;

        if (www.error != null)
        {
            //Debug.Log("request error: " + www.error);

            datareturned = new DataReturned("error", "request_error", "", 0);
        }
        else
        {
            //Debug.Log("request success");
            //Debug.Log("returned data" + www.text);

            datareturned = JsonUtility.FromJson<DataReturned>(www.text);
        }

        SyncDone();
    }

    // Sync is completed
	void SyncDone () {

        syncstatus = SyncStatus.Done;
        
        Result = datareturned.Result;
        Message = datareturned.Message;

        if (Result == "newdata")
        {
            Player_data = datareturned.Player_data;
            Player_update_sync = datareturned.Player_update_sync;
        }
         
	}
	

    // Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
