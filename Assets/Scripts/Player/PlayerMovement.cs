using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    private int currentWaypointIndex = 0;
    private Animator animator;

    private bool isMoving = false;

    [Header("Waypoints")]
    public Transform waypointsParent;
    public Transform[] waypoints;

    [Header("Mouvement")]
    public float moveSpeed = 15f;
    public float speedBoostMultiplier = 3f;

    public Button rollDiceButton;

    public Action OnMoveFinished;

    void Start()
    {
        animator = GetComponent<Animator>();

        waypoints = new Transform[waypointsParent.childCount];
        for(int i = 0; i < waypointsParent.childCount; i++)
        {
            waypoints[i] = waypointsParent.GetChild(i);
        }
        transform.position = waypoints[0].position; // position départ
    }

    // void Update()
    // {
    //     // Test : avancer de 3 cases quand on appuie sur Espace
    //     if (Input.GetKeyDown(KeyCode.Space))
    //     {
    //         MovePlayer(10);
    //     }
    // }

    public void MovePlayer(int steps)
    {
        if (isMoving) return;           // ✅ ignore si déjà en mouvement
        StartCoroutine(Move(steps));
    }

    private IEnumerator Move(int steps)
    {
        isMoving = true;

        if (animator) animator.SetBool("IsRunning", true);

        for (int i = 0; i < steps; i++)
        {
            // Vérifie si la prochaine case existe donc si on est arrivé à la fin
            if (currentWaypointIndex + 1 >= waypoints.Length)
            {
                break;
            }

            currentWaypointIndex++;
            Vector3 target = waypoints[currentWaypointIndex].position;

            // 🔄 Calcul et applique la rotation du pion vers la prochaine case
            Vector3 direction = (target - transform.position).normalized; // vecteur direction du pion vers le waypoint
            if (direction != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction); // rotation qui regarde dans cette direction
                transform.rotation = lookRotation; // applique la rotation au pion
            }

            // On déplace le pion vers la prochaine case
            while (Vector3.Distance(transform.position, target) > 0.1f)
            {
                float currentSpeed = moveSpeed;

                // Si flèche droite enfoncée, accélère
                if (Input.GetKey(KeyCode.RightArrow)) {
                    currentSpeed *= speedBoostMultiplier;
                    if (animator) animator.speed = speedBoostMultiplier;
                } else if (animator) animator.speed = 1f;

                transform.position = Vector3.MoveTowards(transform.position, target, currentSpeed * Time.deltaTime);
                
                yield return null;
            }

            transform.position = target;
            yield return null;
        }

        isMoving = false;

        /* On prévient que le déplacement est fini et déclenche toutes les autres méthodes
        qui contiennent OnMoveFinished */
        OnMoveFinished?.Invoke();

        if (animator) animator.SetBool("IsRunning", false); // 🛑 stop anim
    }
}
