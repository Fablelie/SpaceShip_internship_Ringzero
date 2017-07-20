using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnCometManager : MonoBehaviour, ISpawnSource 
{
    /// <summary>
    /// Prefeb asteroid.
    /// </summary>
    public GameObject prefeb;

    /// <summary>
    /// Amount asteroid in scene.
    /// </summary>
    public int countObjectInScene;

    /// <summary>
    /// List asteroid.
    /// </summary>
    public List<GameObject> currentObjInScene;

    /// <summary>
    /// Spawn Points.
    /// </summary>
    public SpawnPointPair spawnPoint;

    [System.Serializable]
    public struct SpawnPointPair
    {
        /// <summary>
        /// Name spawnPoint.
        /// </summary>
        public string nameObj;

        /// <summary>
        /// GameObject spawnPoint.
        /// </summary>
        public GameObject center;

        /// <summary>
        /// Delay first time for asteroid.
        /// </summary>
        public float offsetspawnTime;

        /// <summary>
        /// Delay for spawn asteroid.
        /// </summary>
        public float spawnTime;

        /// <summary>
        /// Spawn per time.
        /// </summary>
        public int spawnAmount;

        /// <summary>
        /// Limit asteroid around spawn point.
        /// </summary>
        public int maxObj;

        /// <summary>
        /// Count for child object around spawn point.
        /// </summary>
        public int childObject;

    }

    void Awake()
    {
        StartCoroutine(FirstTime( spawnPoint.offsetspawnTime));
    }

    /// <summary>
    /// Check limit asteroid.
    /// </summary>
    /// <returns></returns>
    private bool IsListFull()
    {
        if (spawnPoint.childObject < spawnPoint.maxObj)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// Delay first time on start scene.
    /// </summary>
    /// <param name="waitingTime"></param>
    /// <returns></returns>
    private IEnumerator FirstTime( float waitingTime)
    {
        yield return new WaitForSeconds(waitingTime);

        StartCoroutine(Counter( spawnPoint.spawnTime));
    }

    /// <summary>
    /// Delay working like a Update() 
    /// </summary>
    /// <param name="waitingTime"></param>
    /// <returns></returns>
    private IEnumerator Counter( float waitingTime)
    {

        if (!IsListFull())
        {

            int currentAmount = spawnPoint.spawnAmount + spawnPoint.childObject;

            int amount = spawnPoint.spawnAmount;

            if (currentAmount > spawnPoint.maxObj)
                amount = spawnPoint.maxObj - spawnPoint.childObject;

            Spawning( amount );
        }

        yield return new WaitForSeconds(waitingTime);

        StartCoroutine(Counter( waitingTime));
    }

    /// <summary>
    /// Spawn asteroid.
    /// Count asteroid.
    /// Add asteroid to list.
    /// </summary>
    /// <param name="amount"></param>
    private void Spawning( int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            GameObject obj = Instantiate( prefeb );
            obj.transform.SetParent(transform);
            spawnPoint.childObject++;
            countObjectInScene++;
            currentObjInScene.Add( obj );

            obj.GetComponent<RotateComet>().ResetPosition( spawnPoint.center.transform );
        }
    }

    /// <summary>
    /// Remove comet from scene.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="obj"></param>
    public void Remove ( int index, GameObject obj )
    {
        spawnPoint.childObject--;
        countObjectInScene--;
        currentObjInScene.Remove(obj);
    }
}
