using UnityEngine;

namespace RayWenderlich.Unity.StatePatternInUnity
{
    public class MeleeState : State
    {
        private enum MeleeSubState
        {
            Swinging,
            Rested,
            Sheathed
        }

        private MeleeSubState subState;
        private bool swingWeapon;
        private bool changeWeapon;
        private float restTime;
        private float restThreshold;

        public MeleeState(Character character) : base(character) { }

        public override void Enter()
        {
            restThreshold = character.MeleeRestThreshold;
            restTime = 0f;
            swingWeapon = changeWeapon = false;
            subState = MeleeSubState.Rested;
            character.Equip(character.MeleeWeapon);
        }

        public override void Exit()
        {
            character.Unequip();
        }

        public override void HandleInput()
        {
            swingWeapon = Input.GetButton("Fire1");
            changeWeapon = Input.GetButtonDown("Fire2");
        }

        public override void LogicUpdate()
        {
            switch (subState)
            {
                case MeleeSubState.Swinging:
                    if (!swingWeapon)
                    {
                        subState = MeleeSubState.Rested;
                        character.DeactivateHitBox();
                        restTime = 0;
                    }
                    else
                    {
                        //swing
                    }

                    break;
                case MeleeSubState.Rested:
                    if (swingWeapon)
                    {
                        subState = MeleeSubState.Swinging;
                        character.Equip();
                        character.ActivateHitBox();
                    }
                    else
                    {
                        restTime += Time.deltaTime;
                        if (restTime > restThreshold)
                        {                            
                            subState = MeleeSubState.Sheathed;
                            character.SheathWeapon();
                            restTime = 0;
                        }
                    }

                    break;
                case MeleeSubState.Sheathed:
                    if (swingWeapon)
                    {
                        subState = MeleeSubState.Swinging;
                        character.Equip();
                        character.ActivateHitBox();
                    }

                    break;
            }

            if (changeWeapon)
            {
                ChangeState(character.shooting, character.equipmentStates);
            }
        }

        public override void PhysicsUpdate()
        {
            return;
        }
    }
}
