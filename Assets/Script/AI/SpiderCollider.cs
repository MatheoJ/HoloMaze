using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderCollider : MonoBehaviour
{
    private Vector3 spawnPoint;
    private void Start()
    {
        spawnPoint = transform.position;
    }
    /*private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "SpiderEnemy")
        {
            transform.position = spawnPoint;
        }
    }*/
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.name == "SpiderEnemy")
        {
            transform.position = spawnPoint;
        }
    }
}
