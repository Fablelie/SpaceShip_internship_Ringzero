using UnityEngine;
using System.Collections;

/// <summary>
/// Use in prefabs supply.
/// In game we use just one prefab.
/// All action of supply such as rotate, crash with player, Reset postion.
/// </summary>
public class RotateSupply : MonoBehaviour 
{

    public static bool isOn = true;

    /// <summary>
    /// Object type.
    /// </summary>
    public EnumPooling.EnumSupply type;

    /// <summary>
    /// Score in this object.
    /// </summary>
    public int supplyScore = 1;

    /// <summary>
    /// Energy in this object.
    /// </summary>
    public int supplyEnergy = 0;

    /// <summary>
    /// Child object for show on minimap.
    /// </summary>
    public GameObject icon;
    
    private int[] _arr = { -1, 1 };
    private int _isClockwise;
    private float _xPos, _yPos, _rotateSpeed = 10f;
    
    private bool _rotate;
    private Renderer _renderer;

    private Vector3 _rotateDirection;

    private UsingCon _using;
    private Collider _col;
    private bool _isStop;
    private SpawnSupplyManager _ssm;

    private Renderer _iconRen;
    private int _indexParent;

    public const int SPHERE_RADIUS = 20;
    public const float IN_CAMERA_VIEW = 17.5f;
    public const float RENDERER_ZONE = 2;

    void Awake()
    {
        _ssm = GameObject.FindObjectOfType<SpawnSupplyManager>();
        _using = GameObject.FindObjectOfType<UsingCon>();
        _renderer = gameObject.GetComponent<Renderer>();
        _col = GetComponent<Collider>();
        _iconRen = icon.GetComponent<Renderer>();
        
        // Random for direction rotation.
        _isClockwise = Random.Range(0, _arr.Length);
        _rotateDirection = new Vector3(_arr[_isClockwise],
                                _arr[_isClockwise],
                                _arr[_isClockwise]);
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
        // Enabled when stay font camera.
        if (transform.position.z > IN_CAMERA_VIEW)
        {
            // Rotate object.
            if (isOn)
                transform.Rotate(_rotateDirection * Time.deltaTime * _rotateSpeed);
            _renderer.enabled = true;
            _col.enabled = true;
        }
        else
        {
            _renderer.enabled = false;
            _col.enabled = false;
        }
    }

    IEnumerator UpdateRenderer()
    {
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

    /// <summary>
    /// Check object in camera view.
    /// </summary>
    /// <returns></returns>
    bool CheckObjectInView()
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
        if (GeometryUtility.TestPlanesAABB(planes, _renderer.bounds))
            return true;
        else
            return false;
    }

    /// <summary>
    /// Reset object position.
    /// </summary>
    /// <param name="center"></param>
    /// <param name="minRadius"></param>
    /// <param name="maxRadius"></param>
    /// <param name="axis"></param>
    public void ResetPosition(Transform center, float minRadius, float maxRadius, EnumPooling.IgnoreAxis axis, int indexParent)
    {
        _indexParent = indexParent;

        float ang = Random.value * 360;
        Vector3 pos = new Vector3(0, 0, 0);
        float radius = Random.Range(minRadius, maxRadius);
        if (axis == EnumPooling.IgnoreAxis.Z)
        {
            // Ignore Z axis.
            pos.x = center.localPosition.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
            pos.y = center.localPosition.y + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
            pos.z = center.localPosition.z;
        }
        else if (axis == EnumPooling.IgnoreAxis.Y)
        {
            // Ignore Y axis.
            pos.x = center.localPosition.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
            pos.y = center.localPosition.y;
            pos.z = center.localPosition.z + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
        }
        else if (axis == EnumPooling.IgnoreAxis.X)
        {
            // Ignore X axis.
            pos.x = center.localPosition.x;
            pos.y = center.localPosition.y + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
            pos.z = center.localPosition.z + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
        }
        _isClockwise = Random.Range(0, _arr.Length);

        // Calculate direction between object and centerPoint(in game centerPoint stay in (0,0,0) I use vector3.zero) 
        Vector3 dir = (pos - Vector3.zero).normalized;

        // Push object object from centerPoint come to sphere egde.
        pos = dir * SPHERE_RADIUS;

        // Set object position.
        transform.localPosition = pos;

        gameObject.SetActive(true);
    
    }

    /// <summary>
    /// Increase score point & recalm object to pooling.
    /// </summary>
    void CollectCrade()
    {
        _using.game.IncreaseScorePoint(supplyScore);
        _ssm.Remove(_indexParent, gameObject);
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider c)
    {
        if (c.gameObject.transform.tag == "Player")
        {
            CollectCrade();
        }
    }

    
}
