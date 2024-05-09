using UnityEngine;
using UnityEngine.InputSystem;
 
public class State
{
    public Character character;
    public StateMachine stateMachine;
 
    protected Vector3 gravityVelocity;
    protected Vector3 velocity;
    protected Vector2 input;
 
    public InputAction moveAction;
   // public InputAction lookAction;
    public InputAction jumpAction;
   // public InputAction crouchAction;
    public InputAction attackAction;
    public InputAction burstAction;
 
    public State(Character _character, StateMachine _stateMachine)
    {
        character = _character;
        stateMachine = _stateMachine;
 
        moveAction = character.playerInput.actions["Move"];
       // lookAction = character.playerInput.actions["MoveBackward"];
        jumpAction = character.playerInput.actions["Jump"];
        attackAction = character.playerInput.actions["Attack"];
        burstAction = character.playerInput.actions["Burst"];
       // crouchAction = character.playerInput.actions["Crouch"];
       // sprintAction = character.playerInput.actions["Sprint"];
 
    }
 
    public virtual void Enter()
    {
        Debug.Log("enter state: "+this.ToString());
    }
 
    public virtual void HandleInput()
    {
    }
 
    public virtual void LogicUpdate()
    {
    }
 
    public virtual void PhysicsUpdate()
    {
    }
 
    public virtual void Exit()
    {
    }
}
