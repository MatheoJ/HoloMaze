using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Transformers;


public class MiniMap3D : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private float scale = 0.01f;
    
    [SerializeField]
    InputActionReference moveMiniMap;

    [SerializeField]
    private GameObject PlayerPrefab;
    
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
            GameObject newGameObject = Utils.createDuplicate(child, this.gameObject, scale);
            
            //Get tag of the child 
            if (child.tag == "MovableInMiniMap")
            {
                newGameObject.AddComponent<MiniMapMoovableObject>();
                newGameObject.GetComponent<MiniMapMoovableObject>().linkedObject = child;
                newGameObject.GetComponent<MiniMapMoovableObject>().scale = 1/scale;
                newGameObject.GetComponent<XRGrabInteractable>().enabled = true;
                newGameObject.GetComponent<XRGeneralGrabTransformer>().enabled = true;
            }
        }
        
        //Instantiate the player in the MiniMap3D
        GameObject player = Instantiate(PlayerPrefab, Vector3.zero, Quaternion.identity);
        player.transform.parent = this.gameObject.transform;
        player.transform.localScale = Utils.scaleVector3(player.transform.localScale, scale);
        player.GetComponent<PlayerOnMiniMap>().scale = scale;
        
        setColliderSize();
    }
    
    void Start()
    {
       InitMiniMap(); 
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "PickableObject")
        {
            GameObject pickableObject = other.gameObject;
            pickableObject.tag = "PickableObjectMiniMap";
            
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
            
            
            Destroy(pickableObject.GetComponent<XRGrabInteractable>());
            Destroy(oldRigidbody);
        }
    }

    void MoveMiniMapinFrontOfPlayer(InputAction.CallbackContext context)
    {
        //Get the main camera and place the MiniMap3D in front of it
        GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        Vector3 forward = new Vector3(mainCamera.transform.forward.x, 0.0f, mainCamera.transform.forward.z);
        forward.Normalize();
        forward.y = -0.4f;
        this.transform.position = mainCamera.transform.position + forward * 2.0f;
    }

    void setColliderSize()
    {
        boxCollider = this.gameObject.GetComponent<BoxCollider>();

        // Get all renderers in this GameObject and its children (at any depth)
        Renderer[] renderers = this.GetComponentsInChildren<Renderer>();

        if (renderers.Length > 0)
        {
            // Initialize bounds with the first renderer's bounds
            Bounds bounds = renderers[0].bounds;

            // Encapsulate bounds of all renderers
            foreach (Renderer renderer in renderers)
            {
                bounds.Encapsulate(renderer.bounds);
            }

            // Convert bounds center from world space to local space
            Vector3 localCenter = this.transform.InverseTransformPoint(bounds.center);

            // Set the BoxCollider's center and size
            boxCollider.center = localCenter;
            boxCollider.size = bounds.size;

            // Optionally adjust the size (e.g., increase the 'y' size)
            boxCollider.size = new Vector3(
                boxCollider.size.x,
                boxCollider.size.y + 1.0f,
                boxCollider.size.z
            );
        }
        else
        {
            // Handle the case where no renderers are found
            boxCollider.size = Vector3.zero;
            boxCollider.center = Vector3.zero;
        }
    }
    
}
