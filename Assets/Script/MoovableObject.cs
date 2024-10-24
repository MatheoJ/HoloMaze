using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoovableObject : MonoBehaviour
{
    
    
    public GameObject linkedObject;
    public float scale = 0.1f;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        MoveLinkedObject();
    }

    void MoveLinkedObject()
    {
        linkedObject.transform.localPosition = Utils.scaleVector3(this.gameObject.transform.localPosition, scale);
        linkedObject.transform.localRotation = this.gameObject.transform.localRotation;
    }
}
