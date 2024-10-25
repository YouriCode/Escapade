using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    float currentEnergy;
    float energyThreshold;
    public float moveSpeed = 5f;
    public float initialMoveSpeed = 5f;
    float sprintSpeed;
    public float speedFactor = 1.5f;
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
    private float fallStartY; // Position Y au début de la chute
    public float maxFallHeight = 10f; // Hauteur de chute maximale avant de subir des dégâts
    public float fallDamageMultiplier = 2f; // Multiplicateur de dégâts en fonction de la hauteur de chute
    private float maxEnergy;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        maxEnergy = EventManager.instance.GetMaxEnergy();
        energyThreshold = maxEnergy / 3;
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
            moveSpeed = initialMoveSpeed / 1.5f;
        }
        else if (currentEnergy < energyThreshold / 2)
        {
            moveSpeed = initialMoveSpeed / 3f;
        }
        sprintSpeed = moveSpeed * speedFactor;

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
        CheckFallDamage();

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
    public Transform groundCheck;  // Référence à l'objet GroundCheck
    public float groundDistance = 0.2f;  // Distance pour détecter le sol

    private void CheckGroundStatus()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundLayer);

        // Si le joueur quitte le sol, enregistrer sa position
        if (!isGrounded)
        {
            if (fallStartY == 0) // Si c'est la première fois qu'il quitte le sol
            {
                fallStartY = transform.position.y;
            }
        }
        else
        {
            // Réinitialiser la position de chute lorsque le joueur touche le sol
            fallStartY = 0;
        }
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

    private void CheckFallDamage()
    {
        
        if (!isGrounded && transform.position.y < fallStartY) // Le joueur est en chute
        {
            float fallHeight = fallStartY - transform.position.y; // Calculer la hauteur de chute
            if (fallHeight > maxFallHeight) // Vérifier si la chute est trop haute
            {
                float damage = (fallHeight - maxFallHeight) * fallDamageMultiplier; // Calcul des dégâts
                // Appliquer les dégâts au joueur (à adapter selon ta logique)
                EventManager.instance.UpdateEnergy(EventManager.instance.GetCurrentEnergy() - damage, maxEnergy); // Méthode fictive à adapter
            }
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
