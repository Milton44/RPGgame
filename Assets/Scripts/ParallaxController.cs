using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxController : MonoBehaviour
{
    public Transform[] transformBackground;
    public float parallaxvelocity;
    public Transform seguirTrans;
    private Vector3 destino, seguirTransAnterior;

    void Start()
    {
        transformBackground[0].gameObject.transform.parent.SetParent(seguirTrans);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //parentTrans.position = new Vector3(seguirTrans.position.x, seguirTrans.position.y, transform.position.z);
        BackgroundParallax();
        
    }
    public void BackgroundParallax()
    {
        Vector3 posAnterior = seguirTransAnterior;
        foreach (Transform transformBack in transformBackground)
        {
            Vector3 parallax = (seguirTrans.position - posAnterior) * (parallaxvelocity/transformBack.localPosition.z);
            parallax.x *= -1;
            destino = transformBack.position + parallax;
            destino.z = transformBack.position.z;
            transformBack.position = destino;
        }
        seguirTransAnterior = seguirTrans.position;
    }
}
