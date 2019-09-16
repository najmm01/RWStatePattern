using System.Collections.Generic;
using UnityEngine;

namespace RayWenderlich.Unity.StatePatternInUnity
{
    public class StandingState : GroundedState
    {
        private bool jump;
        private bool crouch;

        public StandingState(Character character) : base(character) { }

        public override void Enter()
        {
            base.Enter();
            speed = character.MovementSpeed;
            rotationSpeed = character.RotationSpeed;
            crouch = jump = false;
        }

        public override void Exit()
        {
            base.Exit();
        }

        public override void HandleInput()
        {
            base.HandleInput();
            crouch = Input.GetButtonDown("Fire3");
            jump = Input.GetButtonDown("Jump");
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            if (crouch)
            {
                ChangeState(character.ducking, character.movementStates);
            }
            else if (jump)
            {
                ChangeState(character.jumping, character.movementStates);
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }
    }
}
