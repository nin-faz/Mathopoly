using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Transform waypointsParent;
    public Transform[] waypoints; // assigné dans l’inspector
    private int currentWaypointIndex = 0;
    private Animator animator;

    public float moveSpeed = 15f;

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

    void Update()
    {
        // Test : avancer de 3 cases quand on appuie sur Espace
        if (Input.GetKeyDown(KeyCode.Space))
        {
            MovePlayer(10);
        }
    }

    public void MovePlayer(int steps)
    {
        StartCoroutine(Move(steps));
    }

    private IEnumerator Move(int steps)
    {
        animator.SetBool("IsRunning", true); // 🏃 démarre l’anim

        for (int i = 0; i < steps; i++)
        {
            if (currentWaypointIndex + 1 >= waypoints.Length)
            {
                break; // on ne sort pas du tableau
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

            while (Vector3.Distance(transform.position, target) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
                yield return null;
            }
        }

        animator.SetBool("IsRunning", false); // 🛑 stop anim
    }
}
