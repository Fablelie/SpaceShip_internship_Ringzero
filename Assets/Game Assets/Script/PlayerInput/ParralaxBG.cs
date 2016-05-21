using UnityEngine;
using System.Collections;

/// <summary>
/// Use in gameobject name BG.
/// Move UV Parralax background when starting rotation.
/// </summary>
public class ParralaxBG : MonoBehaviour
{
    /// <summary>
    /// Divide speed when move UV.
    /// </summary>
    public float parralax = 2f;

    private Vector2 _offset; 
    private MeshRenderer _mr;
    private Material _mat;
    private Vector3 _dir;

    private UsingCon _using;

    void Start()
    {
        _using = GameObject.FindObjectOfType<UsingCon>();
        _mr = gameObject.GetComponent<MeshRenderer>();
        _mat = _mr.material;
    }

	void Update () 
    {
        if (_using.game.IsMoving() && !_using.game.GetGamePause() && !_using.game.GetCrash())
        {
            // Get Ship direction.
            _dir = _using.player.GetCurrentDirection();
            
            // move UV map.
            _offset.x = _dir.x * Time.deltaTime / parralax;
            _offset.y = _dir.y * Time.deltaTime / parralax;
            _mat.mainTextureOffset += _offset;
        }
	}
}
