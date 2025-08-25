using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class BullController : MonoBehaviour
{
    //public Animator animator;

    //movement
    NavMeshAgent navMeshAgent;
    public float speed = 9;

    public float viewRadius = 20;
    public float viewangle = 180;
    public LayerMask playerMask;
    public LayerMask obstacleMasck;
    public float waitAfterCharge = 1f;

    //player
    public int damage = 4;
    public float damageRadius = 2;

    private bool _isWaiting = true;
    GameObject _punto = null;


    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.isStopped = true;

        StartCoroutine(CheckPlayerCoroutine());
    }

    private IEnumerator CheckPlayerCoroutine() 
    {
        while (true)
        {
            CheckPlayer();

            if (!_isWaiting)
            {
                StartCoroutine(Charge());
            }
            yield return new WaitForSeconds(0.1f);
        }

    }
    /*void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, viewRadius);
    }*/

    void CheckPlayer()
    {
        Collider[] playerInRange = Physics.OverlapSphere(transform.position, viewRadius, playerMask);

        if (playerInRange.Length == 0)
        {
            _isWaiting = true;
        }
        else
        {
            for (int i = 0; i < playerInRange.Length; i++)
            {
                //distance and angle to de player
                Transform player = playerInRange[i].transform;
                Vector3 dirToPlayer = (player.position - transform.position).normalized;
                float dstToPlayer = Vector3.Distance(transform.position, player.position);

                if (dstToPlayer > viewRadius) //fuera de radio
                {
                    _isWaiting = true;

                }
                else if (Vector3.Angle(transform.forward, dirToPlayer) < viewangle / 2)
                {
                    if (!Physics.Raycast(transform.position, dirToPlayer, dstToPlayer, obstacleMasck)) //No obstacles
                    {
                        _isWaiting = false;
                        if (_punto == null) 
                        { 
                            _punto = new GameObject("point0");
                            _punto.transform.position = player.transform.position;
                        }
                       
                    }
                    else
                    {
                        _isWaiting = true;
                    }
                }
            }
        }

    }

    private IEnumerator Charge() 
    {
        if (_punto != null)
        {
            navMeshAgent.SetDestination(_punto.transform.position);
            navMeshAgent.isStopped = false;
            navMeshAgent.speed = speed;

            float distancia = Vector3.Distance(gameObject.transform.position, _punto.transform.position);

            if (distancia < 1)
            {
                navMeshAgent.isStopped = true;
                navMeshAgent.speed = 0;


                Collider[] playerInRange = Physics.OverlapSphere(transform.position, damageRadius, playerMask);
                if (playerInRange.Length != 0)
                {
                    HealthComponent health = playerInRange[0].gameObject.GetComponentInChildren<HealthComponent>();
                    health.ReceiveDamage(damage);
                }

                //StartCoroutine(WaitCharge());
                yield return new WaitForSeconds(waitAfterCharge);

                Destroy(_punto);
                _isWaiting = true;
            }
        }
        else 
        {
            _isWaiting = true;
        }
    }

    private IEnumerator WaitCharge()
    {
        //anim aturdido
        yield return new WaitForSeconds(waitAfterCharge);

        //anim idle

    }

    void Rotate() 
    { 
        
    }
}
