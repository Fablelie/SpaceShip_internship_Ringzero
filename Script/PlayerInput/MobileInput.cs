using UnityEngine;
using System.Collections;

/// <summary>
/// Use in GameObject name Center.
/// Use this when have a mobile input.
/// </summary>
public class MobileInput : APlayerInput 
{
    private Vector3 _screenCenter;

    protected override void Awake()
    {
        float h = mainScreenCam.pixelHeight / 2;
        float b = (Screen.height - mainScreenCam.pixelHeight);
        _screenCenter = new Vector3(mainScreenCam.pixelWidth / 2,h+b, Input.mousePosition.z);
        base.Awake();
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    /// <summary>
    /// Calcula direction from input position on screen.
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    Vector3 CalculaDirection (Vector3 pos)
    {
        // find direction between input position and center point.
        Vector3 pointingDir = pos - _screenCenter;
        pointingDirOrigin = pointingDir;
        pointingDir.Normalize();
        return pointingDir;
    }

    /// <summary>
    /// Send direction back to APlayerInput.cs
    /// </summary>
    /// <returns></returns>
    protected override Vector3 GetInputDirection()
    {
        Vector3 posOnScreen = Input.mousePosition;
        Vector3 dir = CalculaDirection(posOnScreen);
        return dir;
    }

    /// <summary>
    /// Check player input
    /// </summary>
    /// <returns></returns>
    protected override bool IsMove()
    {
        if ( Input.GetMouseButton ( 0 ) )
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
