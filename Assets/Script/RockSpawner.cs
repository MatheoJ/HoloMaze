using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockSpawner : MonoBehaviour
{
    public GameObject rockPrefab;
    
    public GameObject miniMap;

    private float xpos = 0.05f;
    // Start is called before the first frame update
    void Start()
    {
        miniMap = GameObject.FindGameObjectWithTag("MiniMap");
        StartCoroutine(SpawnRocks());
    }

    private IEnumerator SpawnRocks()
    {
        Debug.Log("In ThrowBalls()");
        while (true) // keep throwing balls
        {
            SpawnRock(); 

            // Wait for the specified interval before throwing again
            yield return new WaitForSeconds(2f);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    
    void SpawnRock()
    {
        Vector3 spawnPosition = miniMap.transform.position + new Vector3(xpos, 4, 0);
        xpos += 0.05f;
        Instantiate(rockPrefab, spawnPosition, Quaternion.identity);
    }
    
}
