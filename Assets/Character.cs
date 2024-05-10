using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using UnityEngine.UI;
using System.Collections;


public class Character : MonoBehaviour
{
     [Header("Controls")]
    public float playerSpeed = 5.0f;
    //public float crouchSpeed = 2.0f;
   // public float sprintSpeed = 7.0f;
    public float jumpHeight = 0.8f; 
    public float gravityMultiplier = 2;
    public float rotationSpeed = 5f;
    public float crouchColliderHeight = 1.35f;
 
    [Header("Animation Smoothing")]
    [Range(0, 1)]
    public float speedDampTime = 0.1f;
    [Range(0, 1)]
    public float velocityDampTime = 0.9f;
    [Range(0, 1)]
    public float rotationDampTime = 0.2f;
    [Range(0, 1)]
    public float airControl = 0.5f;

    public StateMachine movementSM;
    public StandingState standing;
    public JumpingState jumping;
    public BurstState burstatk;

    public AttackState attacking;
    //public CrouchingState crouching;
   // public LandingState landing;
   // public SprintState sprinting;

    [HideInInspector]
    public float gravityValue = -9.81f;
    [HideInInspector]
    public float normalColliderHeight;
    [HideInInspector]
    public CharacterController controller;
    [HideInInspector]
    public PlayerInput playerInput;
    [HideInInspector]
    public Transform cameraTransform;
    [HideInInspector]
    public Animator animator;
    [HideInInspector]
    public Vector3 playerVelocity;

     public int hitCount = 0;
    public int deathThreshold = 10;

    public void IncreaseHitCount()
    {
       

        if (hitCount >= deathThreshold)
        {
            // Trigger enemy's death animation
            GameObject enemy = GameObject.FindGameObjectWithTag("Enemy");
            if (enemy != null)
            {
                Animator enemyAnimator = enemy.GetComponent<Animator>();
                if (enemyAnimator != null)
                {
                    enemyAnimator.SetFloat("IsIdle", 1);
                    enemyAnimator.SetTrigger("Die");

                    float delay = 5f; // Adjust the delay time as needed
                    StartCoroutine(DestroyOrHideEnemy(enemy, delay));
                    
                }
            }
        }
        else
        {
             hitCount++;
            // Debug.Log(hitCount);
            // Trigger enemy's hit animation
            GameObject enemy = GameObject.FindGameObjectWithTag("Enemy");
            if (enemy != null)
            {
                Animator enemyAnimator = enemy.GetComponent<Animator>();
                if (enemyAnimator != null)
                {
                  
                     enemyAnimator.SetFloat("IsIdle", 1);
                    enemyAnimator.SetTrigger("Hit");
                }
            }

        GameObject healthBar = GameObject.FindGameObjectWithTag("healthbar");
        if (healthBar != null)
        {
            // Access the Slider component of the health bar and increase its value by 10
            Slider healthBarSlider = healthBar.GetComponent<Slider>();
               if (healthBarSlider != null)
         {
               healthBarSlider.value += 10;
             }

        }
        }

        

    }

        IEnumerator DestroyOrHideEnemy(GameObject enemy, float delay)
    {
        yield return new WaitForSeconds(delay);

        // Check if the enemy still exists before destroying or hiding it
        if (enemy != null)
        {
            // You can either destroy the enemy or deactivate it
            // Destroy(enemy);
            enemy.SetActive(false);
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
        cameraTransform = Camera.main.transform;

 
        movementSM = new StateMachine();
        standing = new StandingState(this, movementSM);
        jumping = new JumpingState(this, movementSM);
       // crouching = new CrouchingState(this, movementSM);
       // landing = new LandingState(this, movementSM);
        burstatk = new BurstState(this, movementSM);

        attacking = new AttackState(this, movementSM);
       
        movementSM.Initialize(standing);
 
        normalColliderHeight = controller.height;
        gravityValue *= gravityMultiplier;
    }

    // Update is called once per frame
    void Update()
    {
        movementSM.currentState.HandleInput();
 
        movementSM.currentState.LogicUpdate();
    }

    private void FixedUpdate()
    {
        movementSM.currentState.PhysicsUpdate();
    }
}

