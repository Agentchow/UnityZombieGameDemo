using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyPosition : MonoBehaviour
{
    [SerializeField]
    Transform transformTarget;
    void Update()
    {
        transform.position = transformTarget.position;
    }
}
