using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NNController : MonoBehaviour
{
    private Transform transf;

    private Vector3 movDir = Vector3.zero;      // Vecteur de deplacement
    private float rotSpeed = 300f;              // Vecteur de rotation
    private float speed = 0.1f;                 // Facteur de vitesse
    private float initialVelocity = 0.0f;       // Vitesse initiale
    private float finalVelocity = 2f;           // Vitesse Maximale
    public float currentVelocity = 0.0f;       // Vitesse actuelle
    private float accelerationRate = 1.5f;        // Taux d'acceleration
    //private float decelerationRate = 3f;      // Taux de deceleration

    // Gere chacuns des rayons de detection
    public float maxDistance = 30f;
    public float distForward = 0f;
    public float distLeft = 0f;
    public float distRight = 0f;
    public float distDiagLeft = 0f;
    public float distDiagRight = 0f;

    // Gere le fitness
    public float fitness = 0f;
    private Vector3 lastPosition;               // Permet le calcul de la distance parcourue
    private float distanceTraveled;
    public float[] results;                     // Resultats du feedforward. Contient deux float [-1, 1]. Le premier pour l'acceleration, le deuxieme pour l'orientation
    public bool active = true;                  // Passe a false si le slime heurte un mur

    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        lastPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // Tant que le slime est en vie
        if (active)
        {
            CharacterController controller = gameObject.GetComponent<CharacterController>();
            // Si le neural network nous dit quoi faire... Bah le slime le fait
            if (results.Length != 0)
            {
                // Si on appuis sur la flèche du haut, augmente notre vitesse. Sinon, la diminue
                if (results[0] > 0f)
                {
                    //anim.SetBool("IsWalking", true);
                }
                else
                {
                    //anim.SetBool("IsWalking", false);
                }
                currentVelocity += (accelerationRate * Time.deltaTime) * results[0];
                currentVelocity = Mathf.Clamp(currentVelocity, initialVelocity, finalVelocity); // Ramene notre vitesse entre sa valeur minimale et maximale

                movDir = new Vector3(0, 0, currentVelocity);
                movDir *= speed;
                movDir = transform.TransformDirection(movDir);

                controller.Move(movDir); // Deplace le slime
                transform.Rotate(0, results[1] * rotSpeed * Time.deltaTime, 0); // Tourne le slime
            }
            InteractRaycast();

            // Gestion du fitness
            distanceTraveled += Vector3.Distance(transform.position, lastPosition);
            lastPosition = transform.position;
            fitness += distanceTraveled / 1000;
            fitness -= 0.01f;

            //Debug.Log(fitness);
        }
    }

    // Si le slime touche un mur, PAF il est mort
    // Par contre si il touche un checkpoint, il gagne en fitness
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Mur")
        {
            //Debug.Log("VLAN !");
            active = false;
            fitness += 2f;
            //anim.SetBool("IsWalking", false);
        }
        if (other.gameObject.tag == "Checkpoint")
        {
            fitness += 5f;
        }
    }

    // Gere les rayons pour detecter les murs
    void InteractRaycast()
    {
        transf = GetComponent<Transform>();
        Vector3 playerPosition = transform.position;

        // Cree les directions de chaque rayons
        Vector3 forwardDirection = transf.forward;
        Vector3 leftDirection = transf.right * -1;
        Vector3 rightDirection = transf.right;
        Vector3 diagLeft = transf.TransformDirection(new Vector3(maxDistance / 5, 0f, maxDistance / 5));
        Vector3 diagRight = transf.TransformDirection(new Vector3(-maxDistance / 5, 0f, maxDistance / 5));

        // Cree les rayons
        Ray frontRay = new Ray(playerPosition, forwardDirection);
        Ray leftRay = new Ray(playerPosition, leftDirection);
        Ray rightRay = new Ray(playerPosition, rightDirection);
        Ray diagLeftRay = new Ray(playerPosition, diagLeft);
        Ray diagRightRay = new Ray(playerPosition, diagRight);

        // Gere les collisions des rayons et retourne la distance si ils rencontre un mur
        RaycastHit hit;
        if (Physics.Raycast(frontRay, out hit, maxDistance) && hit.transform.tag == "Mur")
        {
            distForward = hit.distance;
        }
        if (Physics.Raycast(leftRay, out hit, maxDistance) && hit.transform.tag == "Mur")
        {
            distLeft = hit.distance;
        }
        if (Physics.Raycast(rightRay, out hit, maxDistance) && hit.transform.tag == "Mur")
        {
            distRight = hit.distance;
        }
        if (Physics.Raycast(diagLeftRay, out hit, maxDistance) && hit.transform.tag == "Mur")
        {
            distDiagLeft = hit.distance;
        }
        if (Physics.Raycast(diagRightRay, out hit, maxDistance) && hit.transform.tag == "Mur")
        {
            distDiagRight = hit.distance;
        }

        // Affiche les rayons
        /*Debug.DrawRay(transform.position, forwardDirection * maxDistance, Color.green);
        Debug.DrawRay(transform.position, leftDirection * maxDistance, Color.green);
        Debug.DrawRay(transform.position, rightDirection * maxDistance, Color.green);
        Debug.DrawRay(transform.position, diagLeft * maxDistance, Color.green);
        Debug.DrawRay(transform.position, diagRight * maxDistance, Color.green);*/
    }
}
