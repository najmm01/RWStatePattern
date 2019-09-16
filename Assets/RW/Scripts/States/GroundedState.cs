using UnityEngine;

namespace RayWenderlich.Unity.StatePatternInUnity
{
    public class GroundedState : State
    {
        protected float speed;
        protected float rotationSpeed;

        private float horizontalInput;
        private float verticalInput;

        public GroundedState(Character character) : base(character) { }

        public override void Enter()
        {
            horizontalInput = verticalInput = 0.0f;
        }

        public override void Exit()
        {
            return;
        }

        public override void HandleInput()
        {
            verticalInput = Input.GetAxis("Vertical");
            horizontalInput = Input.GetAxis("Horizontal");
        }

        public override void LogicUpdate()
        {
            character.RestrictRotation();
        }

        public override void PhysicsUpdate()
        {
            character.Move(verticalInput * speed, horizontalInput * rotationSpeed);
        }
    }
}