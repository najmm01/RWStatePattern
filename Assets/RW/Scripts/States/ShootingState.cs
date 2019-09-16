using UnityEngine;

namespace RayWenderlich.Unity.StatePatternInUnity
{
    public class ShootingState : State
    {
        bool shoot;
        bool changeWeapon;

        public ShootingState(Character character) : base(character) { }

        public override void Enter()
        {
            shoot = changeWeapon = false;
            character.Equip(character.ShootableWeapon);
        }

        public override void Exit()
        {
            character.Unequip();
        }

        public override void HandleInput()
        {
            shoot = Input.GetButtonDown("Fire1");
            changeWeapon = Input.GetButtonDown("Fire2");
        }

        public override void LogicUpdate()
        {
            if (changeWeapon)
            {
                RemoveState(character.equipmentStates);
            }
            else if (shoot)
            {
                character.Shoot();
            }
        }

        public override void PhysicsUpdate()
        {
            return;
        }
    }
}
