using System.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class BullController : MonoBehaviour
{
    public Animator animator;

    //movement
    NavMeshAgent navMeshAgent;
    public float speed = 9;

    public float viewRadius = 20;
    public float viewangle = 180;
    public LayerMask playerMask;
    public LayerMask obstacleMask;
    public float waitAfterCharge = 1f;

    public float rotateIfBlocked = 45;
    public float spaceToBlocked = 2;

    //player
    public int damage = 4;
    public float damageRadius = 2;

    private bool _isWaiting = true;
    private bool _isCharging = false;
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

            if (!_isWaiting && !_isCharging)
            {
                animator.SetBool("animCharging", true);
                StartCoroutine(Charge());
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

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
                    if (!Physics.Raycast(transform.position, dirToPlayer, dstToPlayer, obstacleMask)) //No obstacles
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
        _isCharging = true;

        if (_punto != null)
        {
            // Mira hacia el punto al iniciar la carga
            Vector3 lookDir = (_punto.transform.position - transform.position).normalized;
            lookDir.y = 0; // opcional, para evitar inclinaciones
            transform.rotation = Quaternion.LookRotation(lookDir);

            // Ahora la dirección de carga es SIEMPRE hacia adelante
            Vector3 direction = transform.forward;

            navMeshAgent.isStopped = true;
            navMeshAgent.updateRotation = false;

            bool hasHit = false;
            bool hasHitPlayer = false;
            float chargeTime = 3f;
            float timer = 0f;

            while (!hasHit && timer < chargeTime)
            {
                navMeshAgent.Move(direction * speed * Time.deltaTime);

                // detectar al jugador
                Collider[] playerInRange = Physics.OverlapSphere(transform.position, damageRadius, playerMask);
                if (playerInRange.Length != 0)
                {
                    hasHit = true;
                    hasHitPlayer = true;

                    HealthComponent health = playerInRange[0].gameObject.GetComponentInChildren<HealthComponent>();
                    health.ReceiveDamage(damage);
                }

                // raycast para colisión
                if (Physics.Raycast(transform.position, direction, out RaycastHit hit, 1f))
                {
                    hasHit = true;   
                }

                timer += Time.deltaTime;
                yield return null;
            }

            // fin de la carga
            navMeshAgent.isStopped = true;
            animator.SetBool("animSpin", true);

            yield return new WaitForSeconds(waitAfterCharge);
            Destroy(_punto);
            yield return StartCoroutine(Rotate180());

            animator.SetBool("animSpin", false);
            animator.SetBool("animCharging", false);
            yield return new WaitForSeconds(waitAfterCharge);

        }

        _isWaiting = true;
        _isCharging = false;
    }



    private IEnumerator Rotate180()
    {
        float rotated = 0f;
        float rotationSpeed = 180f; // grados por segundo 

        while (rotated < 180f)
        {
            float step = rotationSpeed * Time.deltaTime;
            transform.Rotate(Vector3.up, step);
            rotated += step;
            yield return null;
        }

        bool isBloked = true;

        do
        {
            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, spaceToBlocked, obstacleMask))
            {
                transform.Rotate(Vector3.up, rotateIfBlocked); // si se queda mirando a un obstaculo rotamos 
            }
            else 
            {
                isBloked = false;
            }
           // Debug.DrawRay(transform.position, transform.forward * spaceToBlocked, Color.red);
        }
        while (isBloked);

        
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * spaceToBlocked);
    }

}
