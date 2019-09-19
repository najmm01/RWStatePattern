using UnityEngine;

namespace RayWenderlich.Unity.StatePatternInUnity
{
    enum DiveSubState
    {
        InAir,
        Grounded,
        CoolDown
    }

    public class DivingState : State
    {
        private float cooldownTimer;
        private DiveSubState subState;
        private int hardLanding = Animator.StringToHash("HardLand");

        public DivingState(Character character) : base(character) { }

        public override void Enter()
        {
            cooldownTimer = 0f;
            subState = DiveSubState.InAir;
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
            switch (subState)
            {
                case DiveSubState.Grounded:
                    character.TriggerAnimation(hardLanding);
                    character.PlayShockwaveFX();
                    subState = DiveSubState.CoolDown;
                    break;
                case DiveSubState.CoolDown:
                    if (cooldownTimer >= character.DiveCooldownTimer)
                    {
                        RemoveState(character.movementStates);
                    }
                    else
                    {
                        character.RestrictRotation();
                        cooldownTimer += Time.deltaTime;
                    }
                    break;
            }
        }

        public override void PhysicsUpdate()
        {
            if (subState == DiveSubState.InAir && character.CheckCollisionOverlap(character.transform.position))
            {
                subState = DiveSubState.Grounded;
            }
        }
    }
}

