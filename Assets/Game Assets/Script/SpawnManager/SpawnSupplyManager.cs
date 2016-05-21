using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Use in gameobject name SpawnSupplyManager.
/// Store spawnPoint position and other propoties.
/// Store count of supply in scene.
/// Instantiate supply.
/// Remove supply from scene.
/// </summary>
public class SpawnSupplyManager : MonoBehaviour, ISpawnSource
{
    public bool updateSpawnPoint;

    /// <summary>
    /// player position on egde sphere.
    /// </summary>
    public Transform player;

    /// <summary>
    /// Prefeb supply.
    /// </summary>
    public GameObject prefeb;

    /// <summary>
    /// Amount supply in scene.
    /// </summary>
    public int countObjectInScene;

    /// <summary>
    /// List supply.
    /// </summary>
    public List<GameObject> currentObjInScene;

    /// <summary>
    /// Spawn Points.
    /// </summary>
    public SpawnPointPair[] spawnPoint;

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
        public GameObject planet;

        /// <summary>
        /// Ignore axis for spawn side.
        /// </summary>
        public EnumPooling.IgnoreAxis ignoreAxis;

        /// <summary>
        /// Delay first time for supply.
        /// </summary>
        public float offsetspawnTime;

        /// <summary>
        /// Delay for spawn supply.
        /// </summary>
        public float spawnTime;

        /// <summary>
        /// Spawn per time.
        /// </summary>
        public int spawnAmount;

        /// <summary>
        /// Limit supply around spawn point.
        /// </summary>
        public int maxObj;

        /// <summary>
        /// Distance between spawn point and player position on egde sphere.
        /// </summary>
        public Distance distance;

        /// <summary>
        /// Count for child object around spawn point.
        /// </summary>
        public int childObject;

    }

    /// <summary>
    /// Near ratio.
    /// </summary>
    public const int RATIO_OF_NEAR = 60;

    /// <summary>
    /// Middle ratio.
    /// </summary>
    public const int RATIO_OF_MIDDLE = 30;

    /// <summary>
    /// Far ratio.
    /// </summary>
    public const int RATIO_OF_FAR = 10;

    /// <summary>
    /// Don't spawn supply if player stay in distane.
    /// </summary>
    public const int SPAWN_DISTANE = 20;

    void Awake()
    {
        // Start all spawn points.
        for(int i = 0; i < spawnPoint.Length; i++)
        {
            StartCoroutine(FirstTime(i, spawnPoint[i].offsetspawnTime));
        }
    }

    /// <summary>
    /// Update spawnPoint in scene.
    /// </summary>
    void OnValidate()
    {
        if(updateSpawnPoint)
        {
            UpdateSpawnPoint();
            updateSpawnPoint = false;
        }
    }

    /// <summary>
    /// Find & Stroe all spawn points in scene.
    /// </summary>
    void UpdateSpawnPoint()
    {
        GameObject[] obj = GameObject.FindGameObjectsWithTag("SpawnPoint");

        spawnPoint = new SpawnPointPair[obj.Length];

        for (int i = 0; i < obj.Length; i++ )
        {
            spawnPoint[i].planet = obj[i];
            spawnPoint[i].nameObj = obj[i].name;
        }
    }

    /// <summary>
    /// Check limit supply.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    private bool IsListFull(int index)
    {
        if (spawnPoint[index].childObject < spawnPoint[index].maxObj)
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
    /// <param name="index"></param>
    /// <param name="waitingTime"></param>
    /// <returns></returns>
    private IEnumerator FirstTime(int index, float waitingTime)
    {
        yield return new WaitForSeconds(waitingTime);

        StartCoroutine(Counter(index, spawnPoint[index].spawnTime));
    }

    /// <summary>
    /// Delay working like a Update() 
    /// And share supply to 3 phase.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="waitingTime"></param>
    /// <returns></returns>
    private IEnumerator Counter(int index, float waitingTime)
    {

        if (!IsListFull(index) && Vector3.Distance(player.position, spawnPoint[index].planet.transform.position) > SPAWN_DISTANE)
        {

            int currentAmount = spawnPoint[index].spawnAmount + spawnPoint[index].childObject;

            int amount = spawnPoint[index].spawnAmount;

            if (currentAmount > spawnPoint[index].maxObj)
                amount = spawnPoint[index].maxObj - spawnPoint[index].childObject;

            //calc nubers based on apperence ratio
            int near = (amount * RATIO_OF_NEAR) / 100;
            int middle = (amount * RATIO_OF_MIDDLE) / 100;
            int far = (amount * RATIO_OF_FAR) / 100;
            //add leftover based on ind rounding
            int leftover = amount - (near + middle +far);

            

            Spawning(index, far, spawnPoint[index].distance.farMin, spawnPoint[index].distance.farMax);

            Spawning(index, middle, spawnPoint[index].distance.middleMin, spawnPoint[index].distance.middleMax);

            Spawning(index, near, spawnPoint[index].distance.closeMin, spawnPoint[index].distance.closeMax);

            Spawning(index, leftover, spawnPoint[index].distance.farMin, spawnPoint[index].distance.farMax);

        }

        yield return new WaitForSeconds(waitingTime);

        StartCoroutine(Counter(index, waitingTime));
    }

    /// <summary>
    /// Spawn supply.
    /// Count supply.
    /// Add supply to list.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="amount"></param>
    /// <param name="minDistance"></param>
    /// <param name="maxDistance"></param>
    private void Spawning(int index, int amount, float minDistance, float maxDistance)
    {
        for (int i = 0; i < amount; i++)
        {
            GameObject obj = Instantiate(prefeb);
            obj.transform.SetParent(transform);
            spawnPoint[index].childObject++;
            countObjectInScene++;
            currentObjInScene.Add(obj);

            obj.GetComponent<RotateSupply>().ResetPosition(spawnPoint[index].planet.transform, minDistance, maxDistance, spawnPoint[index].ignoreAxis, index);
        }
    }

    /// <summary>
    /// Remove supply from scene.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="obj"></param>
    public void Remove(int index, GameObject obj)
    {
        spawnPoint[index].childObject--;
        countObjectInScene--;
        currentObjInScene.Remove(obj);
    }

}

[System.Serializable]
public class Distance
{
    [Range(0, 5)]
    public float closeMin = 0;
    [Range(0, 5)]
    public float closeMax = 5;

    [Range(5, 20)]
    public float middleMin = 5;
    [Range(5, 20)]
    public float middleMax = 20;

    [Range(20, 40)]
    public float farMin = 20;
    [Range(20, 40)]
    public float farMax = 40;

}


/// <summary>
/// Enum type.
/// </summary>
[System.Serializable]
public class EnumPooling
{
    public enum EnumComet
    {
        LX,
        L,
        M,
        S,
    };

    public enum EnumSupply
    {
        Supply = 0,
        Energy = 1,
        Machine = 2,
        Material = 3,
        Trap = 4,
        
    };

    public enum EnumParticle
    {
        test,
        testRed,
        Crash,
        Explode,
    };

    public enum IgnoreAxis
    {
        X,
        Y,
        Z
    };
}
