using UnityEngine;

namespace RayWenderlich.Unity.StatePatternInUnity
{
    public class JumpingState : State
    {
        private bool grounded;
        private float holdTime;
        private int jumpParam = Animator.StringToHash("Jump");
        private int landParam = Animator.StringToHash("Land");

        public JumpingState(Character character) : base(character) { }

        public override void Enter()
        {
            grounded = false;
            holdTime = 0;
            character.transform.Translate(Vector3.up * (character.CollisionOverlapRadius + 0.1f));
            character.ApplyImpulse(Vector3.up * character.JumpForce);
            character.TriggerAnimation(jumpParam);
        }

        public override void Exit()
        {
            return;
        }

        public override void HandleInput()
        {
            if (Input.GetAxis("Vertical") < 0)
            {
                holdTime += Time.deltaTime;
            }

            if (Input.GetAxis("Vertical") == 0)
            {
                holdTime = 0;
            }
        }

        public override void LogicUpdate()
        {
            if (grounded)
            {
                character.TriggerAnimation(landParam);
                RemoveState(character.movementStates);
            }
            else if (holdTime > character.DiveThreshold)
            {
                RemoveOldAndChangeState(character.diving, character.movementStates);
            }
        }

        public override void PhysicsUpdate()
        {
            grounded = character.CheckCollisionOverlap(character.transform.position);
        }
    }
}
