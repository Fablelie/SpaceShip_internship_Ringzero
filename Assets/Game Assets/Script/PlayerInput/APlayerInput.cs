using UnityEngine;
using System.Collections;

/// <summary>
/// .
/// Receive player input for rotate ship and rotate world.
/// </summary>
public abstract class APlayerInput : MonoBehaviour 
{
    private UsingCon _using;

    /// <summary>
    /// Ship.
    /// </summary>
    public GameObject shipObj;

    /// <summary>
    /// thruster texture.
    /// </summary>
    public Transform thruster;
    
    /// <summary>
    /// particle ignition.
    /// </summary>
    public Transform thrusterParticle;
    
    /// <summary>
    /// particle jet.
    /// </summary>
    public Transform particle;
    
    /// <summary>
    /// rotate speed of ship.
    /// </summary>
    public float rotateSpeed;

    /// <summary>
    /// Waiting time when crash.
    /// </summary>
    public float crashTime = 2f;
    
    /// <summary>
    /// Direction for use rotate.
    /// </summary>
    private Vector3 _currentDirection;

    /// <summary>
    /// ParticleSystem from Transform particle value.
    /// </summary>
    private ParticleSystem _par;

    /// <summary>
    /// ParticleSystem from Transform thruster particle value.
    /// </summary>
    private ParticleSystem _thrusterPar;

    private float _roSpeed;

    /// <summary>
    /// max & min thruster position in ship.
    /// </summary>
    public const float MAX_THRUSTER_POS = -1.09f;
    public const float MIN_THRUSTER_POS = 0.9f;

    /// <summary>
    /// Multiply speed rotation.
    /// </summary>
    public const float ROTATE_SPEED_MULTIPLY = 10;

    /// <summary>
    /// speed show & hide thruster.
    /// </summary>
    public const float SPEED_SHOW_THRUSTER = 10;
    public const float SPEED_HIDE_THRUSTER = 4;

    /// <summary>
    /// Center point.
    /// </summary>
    public GameObject centerObj;

    /// <summary>
    /// Radius for supply mapping with edge sphere.
    /// </summary>
    public float radiusSpawnSupply = 20;

    /// <summary>
    /// Speed rotate.
    /// </summary>
    public float speed = 10;

    public Rigidbody rigidbody;

    /// <summary>
    /// Direction point.
    /// </summary>
    public Vector3 pointingDir;
    public Vector3 pointingDirOrigin;

    /// <summary>
    /// Pivot object for center follow rotate.
    /// </summary>
    public Transform PivotObject;

    /// <summary>
    /// Value smooth rotate.
    /// </summary>
    public float damping = 3;

    /// <summary>
    /// main camera.
    /// </summary>
    public Camera mainScreenCam;

    protected virtual void Awake ( ) 
    {
        _using = GameObject.FindObjectOfType<UsingCon>();

        rigidbody = GetComponent<Rigidbody>();
        _using = GameObject.FindObjectOfType<UsingCon>();
        rigidbody.centerOfMass = Vector3.zero;

         _currentDirection = shipObj.transform.right;
        thruster.localPosition = new Vector3(0, MIN_THRUSTER_POS, 0);

        _par = particle.gameObject.GetComponent<ParticleSystem>();
        _thrusterPar = thrusterParticle.gameObject.GetComponent<ParticleSystem>();
    }

    protected virtual void Update ( )
    {
        // Animate thruster & play particle.
        if ( IsMove ( ) && !_using.game.GetCrash ( ) && !_using.game.GetGamePause ( ) )
        {
            ThrusterStart ( true );
        }
        else if (thruster.localPosition.y < MIN_THRUSTER_POS)
        {
            ThrusterStart ( false );
        }

        // Set direction by input.
        Vector3 playerDir = GetInputDirection();

        RotateShip ( playerDir ); 

        pointingDir = playerDir;

        // smooth rotate center follow pivotObject 
        centerObj.transform.rotation = Quaternion.Lerp ( centerObj.transform.rotation, PivotObject.rotation, Time.deltaTime * damping );
        
        // Set rotation to GameContorller.cs
        _using.game.SetRotation(centerObj.transform.rotation.eulerAngles);
    }

    protected virtual void FixedUpdate ( )
    {
        // check player move and not crash for rotate pivotObject.
        if ( IsMove ( ) && !_using.game.GetCrash ( ) && !_using.game.GetGamePause ( ) )
        {
            RotateWorld ( );

            // Increase energy for edit energy bar.
            _using.game.IncreaseEnergy(speed);
        }
    }

    /// <summary>
    /// Play & stop particle thruster.
    /// </summary>
    /// <param name="isMove"></param>
    void ThrusterStart ( bool isMove )
    {
        if ( isMove )
        {
            //particle
            if (_par.isStopped)
            {
                // Play particle.
                _par.Play();
                _thrusterPar.Play();
            }

            //thruster size 
            if (thruster.localPosition.y > MAX_THRUSTER_POS)
            {
                // Show thruster.
                thruster.localPosition = new Vector3(0, thruster.localPosition.y - (Time.deltaTime * SPEED_SHOW_THRUSTER), 0);
            }
            else
            {
                // Hide thruster.
                thruster.localPosition = new Vector3(0, thruster.localPosition.y + (Time.deltaTime * SPEED_HIDE_THRUSTER), 0);
            }
        }
        else
        {
            if (_par.isPlaying)
            {
                // Stop particle.
                _par.Stop();
                _thrusterPar.Stop();
            }

            // Hide thruster.
            thruster.localPosition = new Vector3(0, thruster.localPosition.y + (Time.deltaTime * SPEED_HIDE_THRUSTER), 0);
        }
    }

    /// <summary>
    /// Rotate ship to target direction or around self.
    /// </summary>
    /// <param name="dir"></param>
    void RotateShip ( Vector3 dir )
    {
        if (_using.game.GetCrash())
        {
            // Rotate Around self.
            shipObj.transform.Rotate(shipObj.transform.forward, ROTATE_SPEED_MULTIPLY * rotateSpeed * Time.deltaTime, Space.Self);
            //_currentDirection = shipObj.transform.up;
        }
        else if (!_using.game.GetGamePause())
        { 
            float AngleRad = Mathf.Atan2(dir.y - shipObj.transform.position.y, dir.x - shipObj.transform.position.x);
             // Get Angle in Degrees
             float AngleDeg = (180 / Mathf.PI) * AngleRad;
             // Rotate Object
             shipObj.transform.rotation = Quaternion.Euler(0, 0, AngleDeg);

            // Rotate from input positon.
            //ApplyDirection( dir, rotateSpeed);
            //shipObj.transform.up = (dir - shipObj.transform.position).normalized;

        }
    }

    /// <summary>
    /// Rotate pivotObject when player input.
    /// </summary>
    void RotateWorld ( )
    {
        pointingDir *= Time.deltaTime;
        pointingDir *= speed;
        pointingDirOrigin = new Vector3(pointingDir.y, -pointingDir.x, 0);
         
        PivotObject.Rotate(pointingDirOrigin, Space.World);
    }

    /// <summary>
    /// After load data from database use this for reset rotation centerpoint & PivotObject.
    /// </summary>
    /// <param name="vec3"></param>
    public void MoveToRotation(Vector3 vec3)
    {
        PivotObject.rotation = Quaternion.Euler(vec3);
        centerObj.transform.rotation = PivotObject.rotation;
    }

    /// <summary>
    /// In ParralaxBG.cs use this function for move UV map.
    /// </summary>
    /// <returns></returns>
    public Vector3 GetCurrentDirection()
    {
        //return _currentDirection;
        return thruster.up;
    }

    /// <summary>
    /// Rotate direction ship only 
    /// </summary>
    /// <param name="target"></param>
    /// <param name="speed"></param>
    //void ApplyDirection(Vector3 targetDirection, float speed)
    //{
    //    float step = speed * Time.deltaTime;

    //    //Vector3 targetDirection = (target - shipObj.transform.position).normalized;
    //    Vector3 newDirection = Vector3.RotateTowards(_currentDirection, targetDirection, step, 0);

    //    newDirection.z = 0;
    //    newDirection = newDirection.normalized;

    //    if (newDirection == Vector3.zero)
    //        newDirection = _currentDirection;

    //    _currentDirection = newDirection;
    //}

    /// <summary>
    /// When crash with comet.
    /// </summary>
    /// <param name="dir"></param>
    public void OnCrash(Vector3 dir)
    {
        StartCoroutine(RotateAround(crashTime));
    }

    /// <summary>
    /// Set time for rotate around self.
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    IEnumerator RotateAround(float time)
    {
        _using.game.SetCrash(true);

        yield return new WaitForSeconds(time);

        _using.game.SetCrash(false);
    }

    protected abstract bool IsMove ( );
    protected abstract Vector3 GetInputDirection();

}
