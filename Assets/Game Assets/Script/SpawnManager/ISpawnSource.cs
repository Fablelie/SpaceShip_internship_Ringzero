using UnityEngine;
using System.Collections;


/// <summary>
/// If the Player collides with an object the ship will tell
/// the source about the destruction. wahtever it is the
/// Supplyplenet or the Cometpool.
/// </summary>
public interface ISpawnSource
{
    void Remove ( int index, GameObject obj );
}
