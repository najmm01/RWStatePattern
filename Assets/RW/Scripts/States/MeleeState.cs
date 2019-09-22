/*
 * Copyright (c) 2019 Razeware LLC
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * Notwithstanding the foregoing, you may not use, copy, modify, merge, publish, 
 * distribute, sublicense, create a derivative work, and/or sell copies of the 
 * Software in any work that is designed, intended, or marketed for pedagogical or 
 * instructional purposes related to programming, coding, application development, 
 * or information technology.  Permission for such use, copying, modification,
 * merger, publication, distribution, sublicensing, creation of derivative works, 
 * or sale is expressly withheld.
 *    
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

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
        private int swingParam = Animator.StringToHash("SwingMelee");
        private int drawParam = Animator.StringToHash("DrawMelee");
        private int sheathParam = Animator.StringToHash("SheathMelee");

        public MeleeState(Character character) : base(character) { }

        public override void Enter()
        {
            character.SetAnimationBool(character.isMelee, true);
            restThreshold = character.MeleeRestThreshold;
            restTime = 0f;
            swingWeapon = changeWeapon = false;
            
            DrawWeapon(character.MeleeWeapon);
            subState = MeleeSubState.Rested;
        }

        public override void Exit()
        {
            character.SetAnimationBool(character.isMelee, false);
            character.Unequip();
        }

        public override void HandleInput()
        {
            swingWeapon = Input.GetButtonDown("Fire1");
            changeWeapon = Input.GetButtonDown("Fire2");
        }

        public override void LogicUpdate()
        {
            switch (subState)
            {
                case MeleeSubState.Swinging:
                    SwingWeapon();

                    if (!swingWeapon)
                    {
                        character.DeactivateHitBox();
                        subState = MeleeSubState.Rested;
                    }

                    break;
                case MeleeSubState.Rested:
                    if (swingWeapon)
                    {
                        restTime = 0f;

                        character.ActivateHitBox();
                        subState = MeleeSubState.Swinging;
                    }
                    else
                    {
                        restTime += Time.deltaTime;
                        if (restTime > restThreshold)
                        {
                            restTime = 0f;

                            SheathWeapon();
                            subState = MeleeSubState.Sheathed;
                        }
                    }

                    break;
                case MeleeSubState.Sheathed:
                    if (swingWeapon)
                    {
                        DrawWeapon();

                        character.ActivateHitBox();
                        subState = MeleeSubState.Swinging;
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

        private void SwingWeapon()
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.meleeSwings);
            character.TriggerAnimation(swingParam);
        }

        private void SheathWeapon()
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.meleeSheath);
            character.TriggerAnimation(sheathParam);
            character.SheathWeapon();
        }

        private void DrawWeapon(GameObject weapon = null)
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.meleeEquip);
            character.TriggerAnimation(drawParam);
            character.Equip(weapon);
        }
    }
}
