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
    public class DivingState : State
    {
        //1
        private enum DiveSubState
        {
            InAir,
            Grounded,
            CoolDown
        }

        private float cooldownTimer;
        //2
        private DiveSubState subState;
        private int hardLanding = Animator.StringToHash("HardLand");
        
        //3
        public DivingState(Character character) : base(character) { }

        public override void Enter()
        {
            DisplayOnUI(UIManager.Alignment.Left);
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
            character.RestrictRotation();
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

