using UnityEngine;

public class FlipbookController : MonoBehaviour
{
    private MaterialPropertyBlock propertyBlock;
    private Renderer rend;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        propertyBlock = new MaterialPropertyBlock();
        
        // Set the start time when this instance is created
        propertyBlock.SetFloat("_StartTime", Time.time);
        rend.SetPropertyBlock(propertyBlock);
    }
}