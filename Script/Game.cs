using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

// Include Facebook namespace
using Facebook.Unity;

[RequireComponent(typeof(Sync))]

public class Game : MonoBehaviour {

    public Text LblFacebookid, LblFacebooktoken, LblSyncstatus;
    public Text LblResult, LblMessage;
    //public Text InputData, InputSyncint;

    public InputField InputData, InputSyncint;

    Sync sync;

    // AWAKE FROM UNITY
    void Awake()
    {
        // CHECK IF FACEBOOK IS INITIALIZED
        if (!FB.IsInitialized)
        {
            // Initialize the Facebook SDK
            FB.Init(InitCallback, OnHideUnity);
        }
        else
        {
            // Already initialized, signal an app activation App Event
            InitCallback();
        }
    }

    private void InitCallback()
    {
        if (FB.IsInitialized)
        {
            // START FACEBOOK
            FB.ActivateApp();

            // CHECK IF USER IS LOGED IN
            if (FB.IsLoggedIn)
            {
                // AccessToken class will have session details
                var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;

                if (LblFacebookid != null)
                    LblFacebookid.text = "Facebook id:" + aToken.UserId;

                if (LblFacebooktoken != null)
                    LblFacebooktoken.text = "Facebook token:" + aToken.TokenString;
            }
            else
            {
                LoadMainSceneIfPosible();
            }

        }
        else
        {
            LoadMainSceneIfPosible();
        }
    }

    public void LoadMainSceneIfPosible()
    {
        //Scene s = SceneManager.GetSceneByName("Menu");
            //if(s != null && s.buildIndex != -1)
                //SceneManager.LoadScene("Menu");
    }

    // PAUSING OUR GAME WHILE FACEBOOK DOES ITS STUFF
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

    // PLAYER CLICKS ON LOG OUT
    public void BtnLogout()
    {
        // LOGIN OUT THE PLAYER
        FB.LogOut();

        LoadMainSceneIfPosible();
    }

    // PLAYERS CLICKS ON SYNC (NOT SUPPOSED TO DO THAT, SHOULD BE CALLED BY GAME LOGIC E.G. DURING NEW IMPORTANT GAME EVENTS)
    public void BtnSync()
    {
        // CHECK IF USER IS LOGED IN
        if (FB.IsLoggedIn)
        {
            // AccessToken class will have session details
            var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;

            Data data = new Data(aToken.UserId, aToken.TokenString, InputData.text, int.Parse(InputSyncint.text));

            // CHECK IF SYNC IS READY
            if (sync.syncstatus == Sync.SyncStatus.Ready)
            {
                if (LblResult != null)
                    LblResult.text = "Result: start syncing";
                sync.SyncProfile(data);
            }
            else
            {
                if (LblResult != null)
                    LblResult.text = "Result: sync not ready";
            }
        }
        else
        {
            LoadMainSceneIfPosible();
        }

    }

    public void SyncData(string dataSync, int syncID)
    {
        // CHECK IF USER IS LOGED IN
        if (FB.IsLoggedIn)
        {
            // AccessToken class will have session details
            var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;

            Data data = new Data(aToken.UserId, aToken.TokenString, dataSync, syncID);

            // CHECK IF SYNC IS READY
            if (sync.syncstatus == Sync.SyncStatus.Ready)
            {
                if (LblResult != null)
                    LblResult.text = "Result: start syncing";
                sync.SyncProfile(data);
            }
            else
            {
                if (LblResult != null)
                    LblResult.text = "Result: sync not ready";
            }
        }
        else
        {
            LoadMainSceneIfPosible();
        }
    }

	void Start () {
        sync = GetComponent<Sync>();
        sync.syncstatus = Sync.SyncStatus.Ready;
	}

    public int GetSyncID()
    {
        return sync.datareturned.Player_update_sync;
    }

    public string GetSyncData()
    {
        return sync.datareturned.Player_data;
    }
	
	void Update () {

        if (LblSyncstatus != null)
            switch(sync.syncstatus)
            {
                case Sync.SyncStatus.Done:
                    LblSyncstatus.text = "Sync status: Done";
                    break;
                case Sync.SyncStatus.Ready:
                    LblSyncstatus.text = "Sync status: Ready";
                    break;
                case Sync.SyncStatus.Syncing:
                    LblSyncstatus.text = "Sync status: Syncing";
                    break;
            }


        if (sync.syncstatus == Sync.SyncStatus.Done)
        {
            // WE RECEIVED THE DATA
            // GETTING OUR RESULTS
            if (LblResult != null)
                //LblResult.text = "Result: " + sync.datareturned.Result;
                LblResult.text = "Player sync id : " + sync.datareturned.Player_update_sync + System.Environment.NewLine+ "Data : " + sync.datareturned.Player_data;

            if (LblMessage != null)
                LblMessage.text = sync.datareturned.Message;

            if (sync.datareturned.Result.Trim() == "newdata")
            {
                if (InputData != null)
                    InputData.text = sync.datareturned.Player_data;

                if (InputSyncint != null)
                    InputSyncint.text = sync.datareturned.Player_update_sync.ToString();
            }

            // READY TO SYNC WHEN NEEDED
            sync.syncstatus = Sync.SyncStatus.Ready;
      
        }
	

        
	}
    public string GetSyncStatus()
    {
        return sync.syncstatus.ToString();
    }

    public bool IsDone(){
       return sync.syncstatus == Sync.SyncStatus.Done;
    }

    public bool IsReady()
    {
        return sync.syncstatus == Sync.SyncStatus.Ready;
    }

    public bool IsSyncing ( )
    {
        return sync.syncstatus == Sync.SyncStatus.Syncing;
    }
}
