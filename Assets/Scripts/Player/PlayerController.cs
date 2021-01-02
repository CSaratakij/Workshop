using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("General Setting")]
    [SerializeField]
    int maxHealth = 3;

    [SerializeField]
    float invinsibleTime = 1.0f;

    [SerializeField]
    float flickeringRate = 0.2f;

    [Header("Movement Setting")]
    [SerializeField]
    float moveSpeed = 10.0f;

    [SerializeField]
    float collideSpeed = 6.0f;

    [SerializeField]
    float jumpSpeed = 5.0f;

    [SerializeField]
    float gravity = -9.8f;

    [SerializeField]
    float collideForceGravity = -9.8f;

    [SerializeField]
    float gravityMultipiler = 1.0f;

    [Header("Rotate Setting")]
    [SerializeField]
    [Range(0.0f, 1.0f)]
    float rotateRate = 0.3f;

    [Header("Dependencies")]
    [SerializeField]
    GameObject models;

    enum MoveType
    {
        Controlable,
        CollideForce
    }

    int currentHealth;
    float currentInvincibleTime = 0.0f;

    bool isInvincible = false;
    bool isFlickering = false;
    bool isStartCollideForce = false;

    MoveType moveType;

    Vector3 inputVector;
    Vector3 nonZeroInputVector;
    Vector3 velocity;
    Vector3 collideForceDirection;

    Material[] materials;
    CharacterController characterController;

    void Awake()
    {
        Initialize();
    }

    void OnEnable()
    {
        ResetStatus();
    }

    void Update()
    {
        InputHandler();
        MovementHandler();
    }

    void LateUpdate()
    {
        RotateHandler();
        FlickeringHandler();
    }

    void OnCollisionEnter(Collision other)
    {
        /* Debug.Log($"Hit {other.gameObject.name}"); */
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item")) {
            Debug.Log("Hit the item...");
        }
        else if (other.CompareTag("Coin")) {
            Debug.Log("Hit the coin...");
            other.gameObject.SetActive(false);
        }
        else if (other.CompareTag("Enemy")) {
            bool shouldHitByEnemy = !isInvincible && (Time.time > currentInvincibleTime);

            if (shouldHitByEnemy) {
                Debug.Log("Hit enemy...");

                int resultHealth = currentHealth - 1;

                resultHealth = (resultHealth < 0) ? 0 : resultHealth;
                currentHealth = resultHealth;

                bool isDead = (resultHealth <= 0);

                if (isDead) {
                    gameObject.SetActive(false);
                }
                else {
                    isInvincible = true;

                    currentInvincibleTime = (Time.time + invinsibleTime);
                    InvokeRepeating("Flickering", 0.0f, flickeringRate);

                    UpdateCollideForceDirection(other.transform);
                    ChangeMoveType(MoveType.CollideForce);
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy")) {
            Debug.Log("Leave enemy...");
        }
    }

    void Initialize()
    {
        characterController = GetComponent<CharacterController>();
        materials = models.GetComponent<Renderer>().materials;

        moveType = MoveType.Controlable;
        collideForceDirection = Vector3.ClampMagnitude(transform.forward + transform.up, 1.0f);

        currentHealth = maxHealth;
        nonZeroInputVector = Vector3.forward;
    }

    void InputHandler()
    {
        bool isGamePause = (Time.timeScale <= 0.0f);

        if (isGamePause) {
            inputVector = Vector3.zero;
            return;
        }

        inputVector.x = Input.GetAxisRaw("Horizontal");
        inputVector.z = Input.GetAxisRaw("Vertical");

        inputVector = Vector3.ClampMagnitude(inputVector, 1.0f);
        bool isInputNotZero = inputVector.sqrMagnitude > 0.2f;

        if (isInputNotZero) {
            nonZeroInputVector = inputVector;
        }
    }

    void MovementHandler()
    {
        switch (moveType) {
            case MoveType.Controlable:
                MoveHandler();
                break;

            case MoveType.CollideForce:
                MoveByCollideForceHandler();
                break;

            default:
                break;
        }
    }

    void ChangeMoveType(MoveType moveType)
    {
        if (this.moveType == moveType) {
            return;
        }

        this.moveType = moveType;

        if (MoveType.CollideForce == moveType) {
            isStartCollideForce = true;
        }
    }

    void MoveHandler()
    {
        if (characterController.isGrounded && velocity.y < 0) {
            velocity.y = 0;
        }

        velocity.x = inputVector.x * moveSpeed;
        velocity.z = inputVector.z * moveSpeed;

        if (Input.GetButtonDown("Jump") && characterController.isGrounded) {
            velocity.y = jumpSpeed;
        }

        velocity.y += (gravity * gravityMultipiler) * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }

    void MoveByCollideForceHandler()
    {
        if (isStartCollideForce) {
            isStartCollideForce = false;
            velocity = collideForceDirection * collideSpeed;
        }

        if (characterController.isGrounded && velocity.y < 0) {
            moveType = MoveType.Controlable;
        }

        velocity.y += (collideForceGravity * gravityMultipiler) * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }

    void UpdateCollideForceDirection(Transform other)
    {
        Vector3 collideDirection = (transform.position - other.transform.position);

        collideDirection.y = 0.0f;
        collideDirection += Vector3.up;

        collideDirection = Vector3.ClampMagnitude(collideDirection, 1.0f);
        collideForceDirection = collideDirection;
    }

    void RotateHandler()
    {
        Quaternion facingRotation = Quaternion.LookRotation(nonZeroInputVector);
        transform.rotation = Quaternion.Slerp(transform.rotation, facingRotation, rotateRate);
    }

    void FlickeringHandler()
    {
        bool shouldStopInvincible = (isInvincible && Time.time > currentInvincibleTime);

        if (shouldStopInvincible) {
            isInvincible = false;
            CancelInvoke("Flickering");
            SetMaterialColor(Color.white);
        }
    }

    void Flickering()
    {
        isFlickering = !isFlickering;
        Color color = (isFlickering) ? Color.red : Color.white;
        SetMaterialColor(color);
    }

    void SetMaterialColor(Color color)
    {
        foreach (var material in materials)
        {
            material.color = color;
        }
    }

    void ResetStatus()
    {
        currentHealth = maxHealth;
    }
}

