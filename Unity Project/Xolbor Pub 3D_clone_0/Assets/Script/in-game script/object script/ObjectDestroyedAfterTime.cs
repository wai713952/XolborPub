using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDestroyedAfterTime : MonoBehaviour
{
    public float timeToDestroy;
    private void Start()
    {
        Invoke("DestroyAfterTime", timeToDestroy);
    }
    private void DestroyAfterTime()
    {
        Destroy(this.gameObject);
    }

}
