using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyable : MonoBehaviour
{
    
    [SerializeField]
    GameObject linkedObject;
    
    public void setLinkedObject(GameObject linkedObject)
    {
        this.linkedObject = linkedObject;
    }
    
    public void destroyObject()
    {
        MoovableObject moovableObject = linkedObject.GetComponent<MoovableObject>();
        if (moovableObject != null)
        {
           Destroy(moovableObject);
        }
        
        Destroy(linkedObject);
        Destroy(this.gameObject);
    }
}
