using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SimpleNav : MonoBehaviour
{
    public Transform player;
    private NavMeshAgent agent;
    public int detectionRange=20;
    public bool seesPlayer = false;
    public float angle = 45f;
    public LayerMask wallLayer;
    public Vector3 LKP;
    public Vector3 startingPosition;
    private bool LKPNotVisited = false;
    public float rotationSpeed = 1f;
    public float rotationAngle = 45f;
    private float startRotation;

    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        startingPosition = transform.position;
        startRotation= transform.eulerAngles.y;
        animator = GetComponent<Animator>();
        wallLayer = LayerMask.GetMask("Wall");
    }

    // Update is called once per frame
    void Update()
    {
        lookForPlayer();
        if(LKPNotVisited)
        {
            if (!NavMesh.SamplePosition(LKP, out NavMeshHit hit2, 0, NavMesh.AllAreas)) { LKPNotVisited = false; }
            //agent.destination = hit2.position;
            if (Vector3.Distance(transform.position, LKP) <= 1)
            {
                LKPNotVisited = false;
            }
            animator.SetBool("Walk", true);
        }
        else
        {
            agent.destination = startingPosition;
            if(Vector3.Distance(transform.position,startingPosition)<1f)
            {
                //Debug.Log("looking for player");
                //ransform.Rotate(0, 45*Mathf.Sin(Time.time/100), 0);
                transform.rotation= Quaternion.Euler(0, Mathf.Sin(Time.time * rotationSpeed) * rotationAngle+startRotation,0);
                animator.SetBool("Walk", false);
            }
        }
        Debug.DrawRay(transform.position, transform.forward * detectionRange, Color.red);

    }

    private void lookForPlayer()
    {
        if(Vector3.Distance(transform.position, player.position) < detectionRange)
        {
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            //Debug.Log("player in range");
            //Debug.Log(Vector3.Angle(transform.forward, directionToPlayer));
            if(Vector3.Angle(transform.forward, directionToPlayer) < angle/2)
            {
                //Debug.Log("player in angle");
                if (!Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hit, Vector3.Distance(transform.position, player.position), wallLayer))
                {
                    seesPlayer = true;
                    if (NavMesh.SamplePosition(player.position, out NavMeshHit hit2, 5f, NavMesh.AllAreas))
                    {
                        LKP = hit2.position;
                        LKPNotVisited = true;
                        agent.destination = LKP;
                    }
                    //Debug.Log("player seen");
                    return;
                }
            }
        }
        seesPlayer = false;
    }

    /*private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Rock"))
        {
            transform.localScale = new Vector3(1, 0.1f, 1);
            this.enabled = false;
        }
    }*/
    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log(collision.gameObject.name);
        if (collision.gameObject.CompareTag("Rock"))
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                // Vérifie si la normale de collision pointe vers le bas (arrivée par le dessus)
                if (contact.normal.y < -0.5f)
                {
                    transform.localScale = new Vector3(1, 0.1f, 1);
                    GetComponent<CapsuleCollider>().enabled = false;
                    GetComponent<Animator>().enabled = false;
                    this.enabled = false;
                }
            }
        }
    }

}
