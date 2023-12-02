using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToNavAgent : MonoBehaviour
{
    [SerializeField] Transform navAgentPosition;

    // Update is called once per frame
    void Update()
    {
        transform.position = navAgentPosition.position;
    }
}
