using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDestroyedAfterAnimation : MonoBehaviour
{
    //this method is for destroying the object holding this script the animation has ended
    //can set destroy delay for destroy (only active when input positive value)

    public float destroyDelay;

    public void DestroyAfterAnimation()
    {
        if (destroyDelay > 0)
        {
            Destroy(this.gameObject, destroyDelay);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
