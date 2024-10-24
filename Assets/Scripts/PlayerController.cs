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
    public float jumpForce = 7f;            // Force de saut
    public float rotationSpeed = 10f;       // Vitesse de rotation
    public Transform cameraTransform;       // Référence à la caméra
    public LayerMask groundLayer;           // Layer pour détecter le sol
    private bool isGrounded;
    private Rigidbody rb;
    private Vector3 moveInput;
    private Vector3 moveVelocity;
    private bool isSprinting = false;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        energyThreshold = EventManager.instance.GetMaxEnergy() / 2;
        animator = GetComponent<Animator>();
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

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }

        // Vérifier si le joueur est au sol
        CheckGroundStatus();

        // Gérer les animations
        HandleAnimations();
    }

    void FixedUpdate()
    {
        // Appliquer le mouvement au Rigidbody
        rb.MovePosition(rb.position + moveVelocity * Time.fixedDeltaTime);
    }

    private void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isGrounded = false;  // Le joueur n'est plus au sol après avoir sauté
        EventManager.TriggerEvent("OnJump");
    }

    // Vérifier si le joueur est au sol
    private void CheckGroundStatus()
    {
        // Raycast pour vérifier si le joueur touche le sol
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f);
    }

    // Gérer les animations du joueur
    private void HandleAnimations()
    {
        // if (!isGrounded)
        // {
        //     animator.SetBool("isJumping", true);
        //     animator.SetBool("isWalking", false);
        //     animator.SetBool("isSprinting", false);
        // }
        if (isSprinting)
        {
            animator.SetBool("isSprinting", true);
            animator.SetBool("isWalking", false);
            animator.SetBool("isJumping", false);
        }
        else if (moveInput.magnitude > 0)
        {
            animator.SetBool("isWalking", true);
            animator.SetBool("isSprinting", false);
            animator.SetBool("isJumping", false);
        }
        else
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isSprinting", false);
            animator.SetBool("isJumping", false);
        }
    }

    // Fonction pour faire tourner le personnage vers la direction du mouvement
    private void RotateTowardsMovementDirection(Vector3 movementDirection)
    {
        if (movementDirection != Vector3.zero)
        {
            // Calculer la rotation cible en fonction de la direction du mouvement
            Quaternion targetRotation = Quaternion.LookRotation(movementDirection, Vector3.up);

            // Ajouter un offset de rotation pour corriger l'orientation (ici, 45 degrés sur l'axe Y)
            Quaternion offsetRotation = Quaternion.Euler(0, -45, 0); // Ajuste l'offset si nécessaire

            // Appliquer la rotation avec l'offset
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation * offsetRotation, rotationSpeed * Time.deltaTime);
        }
    }

}
