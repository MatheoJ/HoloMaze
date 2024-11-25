using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushBack : MonoBehaviour
{
    [SerializeField]
    private HeadCollisionDetector _detector;
    [SerializeField]
    private CharacterController _characterController;
    [SerializeField]
    public float pushBackStrength = 1.0f;
    [SerializeField]
    public float smoothFactor = 0.1f;

    private Vector3 currentPushBackVelocity = Vector3.zero;

    private Vector3 CalculatePushBackDirection(List<RaycastHit> colliderHits)
    {
        Vector3 combinedNormal = Vector3.zero;
        foreach (RaycastHit hitPoint in colliderHits)
        {
            combinedNormal += new Vector3(hitPoint.normal.x, 0, hitPoint.normal.z);
        }
        return combinedNormal;
    }

    private Vector3 spawnPoint;
    private void Start()
    {
        spawnPoint = transform.position;
    }

    private void Update()
    {
        if (_detector.DetectedColliderHits.Count > 0)
        {
            //_detector.DetectedColliderHits
            for(int i=0; i< _detector.DetectedColliderHits.Count; i++)
            {
                if (_detector.DetectedColliderHits[i].transform.gameObject.name == "SpiderEnemy")
                {
                    _characterController.transform.position = spawnPoint;
                }
            }
            Vector3 pushBackDirection = CalculatePushBackDirection(_detector.DetectedColliderHits);

            // Interpolate the pushback direction for smoothness
            currentPushBackVelocity = Vector3.Lerp(currentPushBackVelocity,
                                                   pushBackDirection.normalized * pushBackStrength,
                                                   smoothFactor);

            Debug.DrawRay(transform.position, currentPushBackVelocity, Color.magenta);

            // Apply smooth pushback
            _characterController.Move(currentPushBackVelocity * Time.deltaTime);
        }
        else
        {
            // Gradually reduce the pushback to zero when no collisions are detected
            currentPushBackVelocity = Vector3.Lerp(currentPushBackVelocity, Vector3.zero, smoothFactor);
        }
    }
}