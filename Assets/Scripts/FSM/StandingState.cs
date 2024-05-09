using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Threading.Tasks;

public class StandingState: State
{  
    float gravityValue;
    bool jump;   
    bool crouch;
    bool attack;

    GameObject sword;

    bool burst;
    Vector3 currentVelocity;
    bool grounded;
    bool sprint;
    float playerSpeed;

     public int nhitCount = 0;
    public int ndeathThreshold = 1000;
 
    Vector3 cVelocity;
 
    public StandingState(Character _character, StateMachine _stateMachine) : base(_character, _stateMachine)
    {
        character = _character;
        stateMachine = _stateMachine;
    }
 
    public override void Enter()
    {
        base.Enter();
 
        jump = false;
        crouch = false;
        sprint = false;
        attack = false;
        burst = false;
        input = Vector2.zero;

        sword = GameObject.Find("sword");
        
        currentVelocity = Vector3.zero;
        gravityVelocity.y = 0;
 
        velocity = character.playerVelocity;
        playerSpeed = character.playerSpeed;
        grounded = character.controller.isGrounded;
        gravityValue = character.gravityValue;    
    }
 
    public override void HandleInput()
    {
        base.HandleInput();
 
        if (jumpAction.triggered)
        {
            jump = true;
        }

        if (attackAction.triggered)
        {
            attack = true;
        }

        if (burstAction.triggered)
        {
            burst = true;
        }
        
 
        input = moveAction.ReadValue<Vector2>();
        velocity = new Vector3(input.x, 0, input.y);
 
        velocity = velocity.x * character.cameraTransform.right.normalized + velocity.z * character.cameraTransform.forward.normalized;
        velocity.y = 0f;

         
     
    }
 
    public override void LogicUpdate()
    {
        base.LogicUpdate();
 
        character.animator.SetFloat("Speed", input.magnitude, character.speedDampTime, Time.deltaTime);

         // Show or hide the sword based on player's proximity to the enemy
    if (!IsPlayerCloseToEnemy())
    {
        // If the player is not close to the enemy, hide the sword
        
        if (sword != null)
        {
            sword.SetActive(false);
        }
        // Set a trigger bool value for the enemy's animation 
        GameObject enemyObject = GameObject.FindGameObjectWithTag("Enemy"); 
        if (enemyObject != null)
        {
            Animator enemyAnimator = enemyObject.GetComponent<Animator>();
            if (enemyAnimator != null)
            {
                 enemyAnimator.SetFloat("IsIdle", 0);
                
              
            }
        }
        
    }
    else
    {
        // If the player is close to the enemy, show the sword
        if (sword != null)
        {
            sword.SetActive(true);
        }

        // Set a trigger for the enemy's animation 
        GameObject enemyObject = GameObject.FindGameObjectWithTag("Enemy"); 
        if (enemyObject != null)
        {
            Animator enemyAnimator = enemyObject.GetComponent<Animator>();
            if (enemyAnimator != null)
            {
                enemyAnimator.SetFloat("IsIdle", 1);
                enemyAnimator.SetTrigger("Attack"); 
                
            }

            if (!character.animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            Collider[] hits = Physics.OverlapBox(sword.transform.position, sword.GetComponent<Renderer>().bounds.size / 2, sword.transform.rotation);
    foreach (Collider hit in hits)
    {
             if (hit.CompareTag("Enemy"))
        {
            // Increase hit count for attack
            NCounter();
            
            break;
        
        }
       
    }
           
        
        }
        }

        
    }

    if (IsPlayerCloseToHeal()){
         GameObject healthBar = GameObject.FindGameObjectWithTag("hbar");
        if (healthBar != null)
        {
            // Access the Slider component of the health bar and increase its value by 10
            Slider healthBarSlider = healthBar.GetComponent<Slider>();
               if (healthBarSlider != null)
         {
               healthBarSlider.value = 0;
             }

        }
    }

         
 
        if (sprint)
        {
           // stateMachine.ChangeState(character.sprinting);
        }    
        if (jump)
        {
            stateMachine.ChangeState(character.jumping);
        }
        if (crouch)
        {
           // stateMachine.ChangeState(character.crouching);
        }

        if (attack)
        {
           stateMachine.ChangeState(character.attacking);
        }

         if (burst)
        {
           stateMachine.ChangeState(character.burstatk);
        }


        
    }
 
    public override void PhysicsUpdate()
    {

       base.PhysicsUpdate();

    // Perform downward raycast to detect terrain
    RaycastHit hit;
    if (Physics.Raycast(character.transform.position, Vector3.down, out hit))
    {
        // Calculate distance from character to terrain surface
        float distanceToGround = hit.distance;

        // Adjust character's position to maintain distance above terrain
        Vector3 newPosition = character.transform.position - Vector3.up * (distanceToGround - character.controller.height / 2f);
        character.controller.Move(newPosition - character.transform.position);
    }

    // Update gravity and grounded state
    gravityVelocity.y += gravityValue * Time.deltaTime;
    grounded = character.controller.isGrounded;

    // If grounded and moving downward, reset gravity velocity
    if (grounded && gravityVelocity.y < 0)
    {
        gravityVelocity.y = 0f;
    }

    // Smoothly move the character based on input velocity and gravity
    currentVelocity = Vector3.SmoothDamp(currentVelocity, velocity, ref cVelocity, character.velocityDampTime);
    character.controller.Move((currentVelocity * playerSpeed + gravityVelocity) * Time.deltaTime);

    // Rotate the character to face the movement direction
    if (velocity.sqrMagnitude > 0)
    {
        character.transform.rotation = Quaternion.Slerp(character.transform.rotation, Quaternion.LookRotation(velocity), character.rotationDampTime);
    } 
        
    }

    bool IsPlayerCloseToEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemies)
        {
            // Calculate the distance between player and each enemy
            float distance = Vector3.Distance(character.transform.position, enemy.transform.position);

            // Set a threshold distance for when the player is considered close to the enemy
            float closeDistanceThreshold = 15f; 

            // Check if the distance is less than the threshold
            if (distance < closeDistanceThreshold)
            {
                // Player is close to at least one enemy
                return true;
            }
        }

        // Player is not close to any enemy
        return false;
    }

    bool IsPlayerCloseToHeal()
    {
        GameObject[] hpoints = GameObject.FindGameObjectsWithTag("heal");

        foreach (GameObject hpoint in hpoints )
        {
            // Calculate the distance between player and each healing spot
            float distance = Vector3.Distance(character.transform.position, hpoint.transform.position);

            // Set a threshold distance for when the player is considered close to the healing spot
            float closeDistanceThreshold = 0.5f; 

            // Check if the distance is less than the threshold
            if (distance < closeDistanceThreshold)
            {
                // Player is close to at least one heal point
                return true;
            }
        }

        // Player is not close to any enemy
        return false;
    }

    public async void NCounter(){
        nhitCount++;

         if (nhitCount >= ndeathThreshold)
        {
            character.animator.SetTrigger("Die");
            character.animator.SetFloat("D", 0);
           
             await Task.Delay(15000);
            character.controller.enabled = false;
             GameObject respoint = GameObject.FindGameObjectWithTag("respoint");
            character.transform.position = respoint.transform.position;

            if (character.transform.position == respoint.transform.position){
                 character.animator.SetTrigger("resp");
                 character.animator.SetFloat("D", 1);
                character.controller.enabled = true;
                 
                  GameObject healthBar = GameObject.FindGameObjectWithTag("hbar");
        if (healthBar != null)
        {
            // Access the Slider component of the health bar and increase its value by 10
            Slider healthBarSlider = healthBar.GetComponent<Slider>();
               if (healthBarSlider != null)
         {
               healthBarSlider.value = 0;
             }

        }
            }
            
        } else{
            GameObject healthBar = GameObject.FindGameObjectWithTag("hbar");
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
      
    public override void Exit()
    {
        base.Exit();
 
        gravityVelocity.y = 0f;
        character.playerVelocity = new Vector3(input.x, 0, input.y);
 
        if (velocity.sqrMagnitude > 0)
        {
            character.transform.rotation = Quaternion.LookRotation(velocity);
        }
    }
 
}