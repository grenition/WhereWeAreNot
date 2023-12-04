using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CollisionResources", menuName = "ScriptableObjects/Collision")]
public class CollisionResources : ScriptableObject
{
    public AudioClip[] collisionClips;
    public GameObject[] collisionEffects;


    public AudioClip GetRandomCollisionClip()
    {
        if (collisionClips.Length == 0)
            return null;
        return collisionClips[Random.Range(0, collisionClips.Length)];
    }
    public GameObject GetRandomCollisionEffect()
    {
        if (collisionEffects.Length == 0)
            return null;
        return collisionEffects[Random.Range(0, collisionEffects.Length)];
    }
}
