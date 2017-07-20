using UnityEngine;
using System.Collections;

public class TestTurnPlanet : MonoBehaviour {

    public Transform cam;

    private Renderer _ren;

    public int RENDERER_ZONE = 0;

	// Use this for initialization
	void Start () {
        if (this.cam == null)
            this.cam = Camera.main.transform;

        _ren = GetComponent<Renderer>();
	}
	
	// Update is called once per frame
	void Update () {
        transform.rotation = cam.rotation;

        // Enabled when stay font camera.
        if(transform.parent.position.z <= RENDERER_ZONE)
        {
            _ren.enabled = false;
        }
        else
        {
            _ren.enabled = true;
        }
	}
}
