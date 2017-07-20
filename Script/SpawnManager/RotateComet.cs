using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Use in prefabs Comet.
/// In game we use just one prefab.
/// All action of comet such as rotate, crash with player, Reset postion.
/// </summary>
public class RotateComet : MonoBehaviour
{
    /// <summary>
    /// Damage when player crash with this.
    /// </summary>
    public int damage = 10; // percent

    public GameObject prefabParticle;

    /// <summary>
    /// Child object for show on minimap.
    /// </summary>
    public GameObject icon;
    
    private float _rotateSpeed;
    private int[] _arr = { -1, 1 };
    private int _isClockwise;
    private float _xPos, _yPos;

    private Renderer _renderer;
    private Renderer _iconRen;
    private SpawnCometManager _scm;
    
    private Collider _col;
    private UsingCon _using;

    public const int SPHERE_RADIUS = 20;
    public const float RENDERER_ZONE = 2;
    public const float IN_CAMERA_VIEW = 17.5f;
    public const int FAKE_INDEX = 0;

	// Use this for initialization
    void Awake() 
    {
        _using = GameObject.FindObjectOfType<UsingCon>();

        _scm = GameObject.FindObjectOfType<SpawnCometManager>();
        
        _renderer = gameObject.GetComponent<Renderer>();
        _iconRen = icon.GetComponent<Renderer>();
        _col = GetComponent<Collider>();
        _rotateSpeed = Random.Range(10f, 30f);
        _isClockwise = Random.Range(0, _arr.Length);

    }

    void OnEnable()
    {
        StartCoroutine(UpdateRenderer());
    }

    void OnDisable()
    {
        StopCoroutine(UpdateRenderer());
    }

    void Update()
    {
        // enabled self when stay camera view.
        if (transform.position.z > IN_CAMERA_VIEW)
        {
            _renderer.enabled = true;
            transform.LookAt(_using.player.centerObj.transform);
            _col.enabled = true;
            //transform.Rotate(0, 0, _arr[_isClockwise] * (Time.deltaTime * _rotateSpeed));
        }
        else
        {
            _renderer.enabled = false;
            _col.enabled = false;
        }

        
    }

    IEnumerator UpdateRenderer()
    {
        // enabled icon comet in minimap when stay in font side camera.
        if (transform.position.z <= RENDERER_ZONE)
        {
            _iconRen.enabled = false;
        }
        else
        {
            _iconRen.transform.rotation = Quaternion.identity;
            _iconRen.enabled = true;
        }

        yield return new WaitForSeconds(.5f);
        StartCoroutine(UpdateRenderer());
    }

    void OnTriggerEnter(Collider c)
    {
        // Check tag.
        if(c.gameObject.transform.tag == "Player")
        {
            Vector3 dir = (c.gameObject.transform.position - transform.position).normalized;
            _using.player.OnCrash(dir);

            // Decrease score point if in game have a score.
            if (_using.game != null)
            {
                _using.game.DecreaseScorePoint(damage);
            }
            
            _scm.Remove ( FAKE_INDEX, gameObject );
            Destroy ( gameObject );

            Instantiate ( prefabParticle, transform.position, Quaternion.identity );
    
        }
    }

    /// <summary>
    /// Reset object position.
    /// </summary>
    /// <param name="center"></param>
    public void ResetPosition( Transform center )
    {
        _rotateSpeed = Random.Range(10f, 30f);
        _isClockwise = Random.Range(0, _arr.Length);
        
        // Push object to sphere egde.
        transform.localPosition = Random.onUnitSphere * SPHERE_RADIUS;
    }

}
