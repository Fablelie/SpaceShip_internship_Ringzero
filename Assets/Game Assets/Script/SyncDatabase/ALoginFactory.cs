using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public abstract class ALoginFactory : MonoBehaviour 
{
    protected UsingCon _using;

    protected virtual void Awake ( )
    {
        _using = GameObject.FindObjectOfType < UsingCon > ( );
    }

    public abstract void Init ( );
    public abstract void Destory ( );
    public abstract void Save ( );
    public abstract void Load ( );
}
