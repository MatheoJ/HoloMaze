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

    private int MiniMapLayer;

    private void Awake()
    {
        MiniMapLayer = LayerMask.NameToLayer("MiniMapLayer");
        InitMiniMap(); 
        moveMiniMap.action.performed += MoveMiniMapinFrontOfPlayer;
    }

    private void OnDestroy()
    {
        moveMiniMap.action.performed -= MoveMiniMapinFrontOfPlayer;
    }
    
    private void GoThroughChildRecursive(GameObject go)
    {
        foreach (Transform child in go.transform)
        {
            child.gameObject.layer = MiniMapLayer;
            
            Debug.Log("Child name: " + child.gameObject.name + "layer : " + child.gameObject.layer);
            
            if(child.gameObject.tag == "CrystalBall")
            {
                child.gameObject.GetComponent<XRGrabInteractable>().enabled = false;
            }

            Transform _HasChildren = child.GetComponentInChildren<Transform>();
            if (_HasChildren != null)
                GoThroughChildRecursive(child.gameObject);
        }
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
            
            //Deal with all the thing that are movable in the minimap
            if (child.tag == "MovableInMiniMap")
            {
                newGameObject.AddComponent<MiniMapMoovableObject>();
                newGameObject.GetComponent<MiniMapMoovableObject>().linkedObject = child;
                newGameObject.GetComponent<MiniMapMoovableObject>().scale = 1/scale;
                newGameObject.GetComponent<XRGrabInteractable>().enabled = true;
                newGameObject.GetComponent<XRGeneralGrabTransformer>().enabled = true;
            }
            else if (child.tag == "Destroyable")
            {
                newGameObject.AddComponent<Destroyable>();
                newGameObject.GetComponent<Destroyable>().setLinkedObject(child);
            }
        }
        
        //Instantiate the player in the MiniMap3D
        GameObject player = Instantiate(PlayerPrefab, Vector3.zero, Quaternion.identity);
        player.transform.parent = this.gameObject.transform;
        player.transform.localPosition = Vector3.zero;
        player.transform.localScale = Utils.scaleVector3(player.transform.localScale, scale);
        player.GetComponent<PlayerOnMiniMap>().scale = scale;
        
        
        GoThroughChildRecursive(this.gameObject);
    }
    
    void Start()
    {
      
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "PickableObject")
        {
            GameObject pickableObject = other.gameObject;
            pickableObject.tag = "PickableObjectMiniMap";
            pickableObject.GetComponent<XRGrabInteractable>().enabled = false;

            
            pickableObject.transform.parent = this.transform;
            GameObject miniMapReproductible = GameObject.FindGameObjectWithTag("MapReproductible");
            
            GameObject newMovableObjectInMap =  Instantiate(pickableObject, Vector3.zero, Quaternion.identity);
            newMovableObjectInMap.transform.parent = miniMapReproductible.transform;
            newMovableObjectInMap.transform.localScale = Utils.scaleInvVector3(pickableObject.transform.localScale, scale);
            newMovableObjectInMap.transform.localPosition = Utils.scaleInvVector3(pickableObject.transform.localPosition, scale);
            newMovableObjectInMap.transform.localRotation = pickableObject.transform.localRotation;
            newMovableObjectInMap.tag = "Rock";
            
            Destroyable destroyable = pickableObject.GetComponent<Destroyable>();
            if (destroyable != null)
                destroyable.setLinkedObject(newMovableObjectInMap);
            
            destroyable = newMovableObjectInMap.GetComponent<Destroyable>();
            if (destroyable != null)
                Destroy(destroyable);
            
            
            Rigidbody oldRigidbody = pickableObject.GetComponent<Rigidbody>();
            
            newMovableObjectInMap.GetComponent<Rigidbody>().velocity = oldRigidbody.velocity;
            newMovableObjectInMap.AddComponent<MoovableObject>();
            newMovableObjectInMap.GetComponent<MoovableObject>().linkedObject = pickableObject;
            newMovableObjectInMap.GetComponent<MoovableObject>().scale = scale;           
            
            pickableObject.layer = MiniMapLayer;
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
