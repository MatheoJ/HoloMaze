using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class MiniMap3D : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private float scale = 0.1f;
    
    [SerializeField]
    InputActionReference moveMiniMap;
    
    private BoxCollider boxCollider;

    private void Awake()
    {
        moveMiniMap.action.performed += MoveMiniMapinFrontOfPlayer;
    }

    private void OnDestroy()
    {
        moveMiniMap.action.performed -= MoveMiniMapinFrontOfPlayer;
    }

    void InitMiniMap()
    {
        //Get the game object with the tag MiniMapReproductible
        GameObject miniMapReproductible = GameObject.FindGameObjectWithTag("MapReproductible");
        //Get all his children
        List<GameObject> childrenReproductible = new List<GameObject>();
        foreach (Transform child in miniMapReproductible.transform)
        {
            childrenReproductible.Add(child.gameObject);
        }
        
        //Duplicate all the children of the MiniMapReproductible and put them as children of the MiniMap3D
        
        foreach (GameObject child in childrenReproductible)
        {
            Utils.createDuplicate(child, this.gameObject, scale);
        }
        
        boxCollider = this.gameObject.GetComponent<BoxCollider>();
        //Set the box collider of the MiniMap3D to encapsulate all the children
        Bounds bounds = new Bounds(this.transform.GetChild(0).transform.position, Vector3.zero);
        foreach (Transform child in this.transform)
        {
            Renderer childRenderer = child.GetComponent<Renderer>();
            if (childRenderer != null)
            {
                bounds.Encapsulate(childRenderer.bounds);
            }
        }
        boxCollider.size = bounds.size;
        boxCollider.size = new Vector3(boxCollider.size.x, boxCollider.size.y + 1.0f, boxCollider.size.z);
        boxCollider.center = bounds.center - this.transform.position;
    }
    
    void Start()
    {
       InitMiniMap(); 
    }

    // Update is called once per frame
    void Update()
    {
        //If the player press the moveMiniMap button, the MiniMap3D will move
        
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "PickableObject")
        {
            GameObject pickableObject = other.gameObject;
            
            pickableObject.transform.parent = this.transform;
            GameObject miniMapReproductible = GameObject.FindGameObjectWithTag("MapReproductible");
            
            GameObject newMovableObjectInMap =  Instantiate(pickableObject, Vector3.zero, Quaternion.identity);
            newMovableObjectInMap.transform.parent = miniMapReproductible.transform;
            newMovableObjectInMap.transform.localScale = Utils.scaleInvVector3(pickableObject.transform.localScale, scale);
            newMovableObjectInMap.transform.localPosition = Utils.scaleInvVector3(pickableObject.transform.localPosition, scale);
            newMovableObjectInMap.transform.localRotation = pickableObject.transform.localRotation;
            
            
            Rigidbody oldRigidbody = pickableObject.GetComponent<Rigidbody>();
            
            newMovableObjectInMap.GetComponent<Rigidbody>().velocity = oldRigidbody.velocity;
            
            newMovableObjectInMap.AddComponent<MoovableObject>();
            newMovableObjectInMap.GetComponent<MoovableObject>().linkedObject = pickableObject;
            newMovableObjectInMap.GetComponent<MoovableObject>().scale = scale;
            
            pickableObject.tag = "PickableObjectMiniMap";
            Destroy(oldRigidbody);
        }
    }

    void MoveMiniMapinFrontOfPlayer(InputAction.CallbackContext context)
    {
        //Get the main camera and place the MiniMap3D in front of it
        GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        Vector3 forward = new Vector3(mainCamera.transform.forward.x, 0.0f, mainCamera.transform.forward.z);
        forward.Normalize();
        forward.y = -0.2f;
        this.transform.position = mainCamera.transform.position + forward * 2.0f;
    }
    
}
