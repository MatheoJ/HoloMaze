using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOnMiniMap : MonoBehaviour
{
    public float scale = 0.1f;
    Transform mapTransform;
    Transform mainCameraTransform;
    
    // Start is called before the first frame update
    void Start()
    {
        mapTransform = GameObject.FindGameObjectWithTag("MapReproductible").transform;
        mainCameraTransform = GameObject.FindGameObjectWithTag("MainCamera").transform;
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
        Vector3 posRelativeToMap = mainCameraTransform.position - mapTransform.position;
        this.transform.localPosition = Utils.scaleVector3(posRelativeToMap, scale);
        this.transform.localRotation = this.gameObject.transform.localRotation;
    }
}
