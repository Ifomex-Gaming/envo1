using UnityEngine;

public class BurstState : State
{
    bool grounded;

    float gravityValue;
    float jumpHeight;
    float playerSpeed;
    bool standing;
    Vector3 currentVelocity;
    GameObject sword;
     bool isAttacking;
    bool hasFinishedAttack;

    Vector3 airVelocity;
     Vector3 cVelocity;

    public BurstState(Character _character, StateMachine _stateMachine) : base(_character, _stateMachine)
    {
        character = _character;
        stateMachine = _stateMachine;
    }

    public override void Enter()
    {
        base.Enter();
        standing = false;
        grounded = false;

        input = Vector2.zero;
        sword = GameObject.Find("sword");
        
        currentVelocity = Vector3.zero;
        gravityVelocity.y = 0;
        gravityValue = character.gravityValue;
        jumpHeight = character.jumpHeight;
        playerSpeed = character.playerSpeed;
        gravityVelocity.y = 0;

        character.animator.SetFloat("Speed", 0);
        character.animator.SetTrigger("Burst");
        Burst();
    }

    public override void HandleInput()
    {
        base.HandleInput();

         input = moveAction.ReadValue<Vector2>();
        velocity = new Vector3(input.x, 0, input.y);
 
        velocity = velocity.x * character.cameraTransform.right.normalized + velocity.z * character.cameraTransform.forward.normalized;
        velocity.y = 0f;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        character.animator.SetFloat("Speed", input.magnitude, character.speedDampTime, Time.deltaTime);

         if (!character.animator.GetCurrentAnimatorStateInfo(0).IsName("Burst"))
        {
            hasFinishedAttack = true;
        }

        // If attack animation has finished and the attack key is pressed again, trigger a new attack
        if (hasFinishedAttack && isAttacking && attackAction.triggered)
        {
            stateMachine.ChangeState(character.burstatk);
        } else{
            stateMachine.ChangeState(character.standing);
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

    void Burst()
    {
       // Debug.Log("Attack performed!");

        // Access the sword collider
     Collider[] hits = Physics.OverlapBox(sword.transform.position, sword.GetComponent<Renderer>().bounds.size / 2, sword.transform.rotation);
    foreach (Collider hit in hits)
    {
        if (hit.CompareTag("Enemy"))
        {
            // Increase hit count for attack
            character.IncreaseHitCount();
            
            break;
        }
    }
        isAttacking = false;
    }
}

