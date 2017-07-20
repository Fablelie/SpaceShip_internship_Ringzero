using UnityEngine;
using System.Collections;

/// <summary>
/// Use in prefabs Particle.
/// Action particle.
/// </summary>
public class ParticleAction : MonoBehaviour
{
    private ParticleSystem _par;
 
	// Use this for initialization
	void Start () 
    {
        _par = gameObject.GetComponent<ParticleSystem>();
	}

    void Update()
    {
        if(!_par.isPlaying)
        {
            Destroy ( gameObject );
        }
    }
	
}
