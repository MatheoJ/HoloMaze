using System;
using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

public class MiniMapMoovableObject : MonoBehaviour
{
    public GameObject linkedObject;
    public float scale = 0.1f;
    Transform miniMapTransform;
    
    // Start is called before the first frame update
    void Start()
    {
        miniMapTransform = GameObject.FindGameObjectWithTag("MiniMap").transform;
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
        Vector3 posRelativeToMiniMap = miniMapTransform.InverseTransformPoint(this.gameObject.transform.position);
        linkedObject.transform.localPosition = Utils.scaleVector3(posRelativeToMiniMap, scale);
    }
}
