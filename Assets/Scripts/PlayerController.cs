using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    float currentEnergy;
    float energyThreshold;
    float moveSpeed = 5f;
    public float initialMoveSpeed = 5f;
    float sprintSpeed;
    public float sprintFactor = 1.5f;
    public float rotationSpeed = 10f;  // Vitesse de rotation
    public Transform cameraTransform;    // Référence à la caméra

    private Rigidbody rb;
    private Vector3 moveInput;
    private Vector3 moveVelocity;
    private bool isSprinting = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        energyThreshold = EventManager.instance.GetEnergyThreshold();
    }

    void Update()
    {
        // Récupérer l'input du joueur
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");

        // Déterminer la direction du mouvement
        moveInput = new Vector3(moveHorizontal, 0f, moveVertical).normalized;

        currentEnergy = EventManager.instance.GetCurrentEnergy();
        moveSpeed = initialMoveSpeed;
        if (currentEnergy < energyThreshold)
        {
            moveSpeed = initialMoveSpeed / 2f;
        }
        else if (currentEnergy < energyThreshold / 2)
        {
            moveSpeed = initialMoveSpeed / 4f;
        }
        sprintSpeed = moveSpeed * sprintFactor;

        // Gestion du sprint
        if (Input.GetKey(KeyCode.LeftShift) && moveInput.magnitude > .5f)
        {
            isSprinting = true;
            EventManager.TriggerEvent("OnSprint");
        }
        else
        {
            isSprinting = false;
            EventManager.TriggerEvent("OnWalk");

        }

        // Calculer la direction de mouvement par rapport à la caméra
        Vector3 cameraForward = cameraTransform.forward;  // Direction de la caméra
        cameraForward.y = 0;  // Ignorer la composante verticale
        cameraForward.Normalize();  // Normaliser le vecteur

        Vector3 cameraRight = cameraTransform.right;  // Droite de la caméra
        cameraRight.y = 0;  // Ignorer la composante verticale
        cameraRight.Normalize();  // Normaliser le vecteur

        // Direction du mouvement basé sur la caméra
        Vector3 moveDirection = cameraForward * moveInput.z + cameraRight * moveInput.x;

        // Calcul de la vélocité du mouvement
        moveVelocity = moveDirection * (isSprinting ? sprintSpeed : moveSpeed);

        // Si le personnage se déplace, le faire pivoter vers la direction du mouvement
        if (moveDirection != Vector3.zero)
        {
            RotateTowardsMovementDirection(moveDirection);
        }
    }

    void FixedUpdate()
    {
        // Appliquer le mouvement au Rigidbody
        rb.MovePosition(rb.position + moveVelocity * Time.fixedDeltaTime);
    }

    // Fonction pour faire tourner le personnage vers la direction du mouvement
    private void RotateTowardsMovementDirection(Vector3 movementDirection)
    {
        // Calculer la rotation cible en fonction de la direction du mouvement
        Quaternion targetRotation = Quaternion.LookRotation(movementDirection, Vector3.up);

        // Appliquer une rotation douce avec Slerp (interpolation sphérique)
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}
