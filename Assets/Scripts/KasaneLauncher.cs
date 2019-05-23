using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(KasaneDropZone))]
public class KasaneLauncher : MonoBehaviour
{

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        

        if (transform.childCount != 0)
        {

            var KasaneDropZone = this.GetComponent<KasaneDropZone>();
            if (KasaneDropZone != null) KasaneDropZone.enabled = true;
        }
    }
}
