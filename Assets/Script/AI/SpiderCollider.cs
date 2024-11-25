using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderCollider : MonoBehaviour
{
    private Vector3 spawnPoint;
    private void Start()
    {
        spawnPoint = new Vector3(-16.6100006f, 1.37f, -27.9400005f);// transform.position;
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);
        if (other.gameObject.name == "SpiderEnemy")
        {
            transform.position = spawnPoint;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name);
        if(collision.gameObject.name == "SpiderEnemy"|| collision.gameObject.tag=="Enemy")
        {
            transform.position = spawnPoint;
        }
    }
    public void BackToSpawn()
    {
        transform.position = spawnPoint;
    }
}
