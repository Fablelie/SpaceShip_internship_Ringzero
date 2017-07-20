using UnityEngine;
using System.Collections;

/// <summary>
/// Use in prefab name ParticleOnSave.
/// </summary>
public class ParticleOnSave : MonoBehaviour 
{
    ParticleSystem par;
	// Use this for initialization
	void Start () 
    {
        par = gameObject.GetComponent<ParticleSystem>();
	}
	
	// Update is called once per frame
	void Update () 
    {
	    if (par.isStopped)
        {
            Destroy(gameObject);
        }
	}
}
