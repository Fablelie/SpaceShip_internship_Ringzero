using UnityEngine;
using System.Collections;

/// <summary>
/// Use in gameobject name Collect child of SpaceShip.
/// Find the near planet from player and change color background.
/// </summary>
public class FindArea : MonoBehaviour 
{
    /// <summary>
    /// All planets in scene.
    /// </summary>
    public Transform[] planets;
    
    /// <summary>
    /// Background.
    /// </summary>
    public GameObject bg;
    
    /// <summary>
    /// List of color.
    /// </summary>
    public Color[] colors = new Color[]{
        Color.blue,
        Color.red,
        Color.yellow,
        Color.cyan,
        Color.green,
        Color.magenta
    };

    /// <summary>
    /// Color between planet.
    /// </summary>
    public Color neutralColor = Color.gray;

    public float maxColorDistance = 10;

    public ParticleSystem par;

    private float _distance = float.MaxValue;
    private int _index;
    private Material _m;
    private UsingCon _using;
    private float _dis = 0;

	// Use this for initialization
	void Start () 
    {
        _using = GameObject.FindObjectOfType<UsingCon>();
        _m = bg.GetComponent<Renderer>().material;
        StartCoroutine(UpdateDistance());
	}

    /// <summary>
    /// Find least distance between player with planets and change color background.
    /// </summary>
    /// <returns></returns>
	IEnumerator UpdateDistance()
    {

        for (int i = 0; i < planets.Length; i++ )
        {
            _dis = Vector3.Distance(planets[i].position, transform.position);
            
            if(_dis < _distance)
            {
                _distance = _dis;
                _index = i;
            }
        }

        float val = (_distance - 5) / maxColorDistance;
        val /= 2;
        val += 0.5f;
        val = Mathf.Clamp(val, 0, 1);
        Color c = Color.Lerp(colors[_index], neutralColor, val);
        _m.color = c;
        _using.game.SetArea((GameController.Area)_index);
        _distance = float.MaxValue;
        
        yield return new WaitForEndOfFrame();
        
        StartCoroutine(UpdateDistance());
    }

    void OnTriggerEnter(Collider c)
    {
        if (c.tag == "Station")
        {
            //Play animation only when synced
//#if ( UNITY_ANDROID || UNITY_IPHONE )
//            if (_using.profile.SaveData())
//            {
//                CreateSaveRing ( c );
//            }
//#endif

//#if ( UNITY_EDITOR || UNITY_STANDALONE ) 
//            _using.profile.SaveDataToPlayerPrefs ( );
//            CreateSaveRing ( c );
//#endif

            _using.login.Save ( );
            CreateSaveRing ( c );
        }
    }

    public void CreateSaveRing ( Collider c )
    {
        Vector3 dir = (gameObject.transform.position - c.transform.position).normalized;
        Vector3 pos = c.transform.position + (dir * c.transform.gameObject.GetComponent<SphereCollider>().radius);

        GameObject obj = Instantiate(par.gameObject, pos, Quaternion.identity) as GameObject;
        obj.transform.parent = _using.player.centerObj.transform;
    }

    public void CreateSaveRing ( )
    {
        GameObject obj = Instantiate(par.gameObject, gameObject.transform.position, Quaternion.identity) as GameObject;
        obj.transform.parent = _using.player.centerObj.transform;
    }

}
