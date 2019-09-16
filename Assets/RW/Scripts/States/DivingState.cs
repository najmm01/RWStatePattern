using UnityEngine;

namespace RayWenderlich.Unity.StatePatternInUnity
{
    public class DivingState : State
    {
        private bool grounded;
        private int hardLanding = Animator.StringToHash("HardLand");

        public DivingState(Character character) : base(character) { }

        public override void Enter()
        {
            grounded = false;
            character.ApplyImpulse(Vector3.down * character.DiveForce);
        }

        public override void Exit()
        {
            return;
        }

        public override void HandleInput()
        {
            return;
        }

        public override void LogicUpdate()
        {
            if (grounded)
            {
                character.TriggerAnimation(hardLanding);
                RemoveState(character.movementStates);
            }
        }

        public override void PhysicsUpdate()
        {
            grounded = character.CheckCollisionOverlap(character.transform.position);
        }
    }
}

