﻿/*
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

using System;
using System.Collections.Generic;
using UnityEngine;

namespace RayWenderlich.Unity.StatePatternInUnity
{
    [RequireComponent(typeof(CapsuleCollider))]
    public class Character : MonoBehaviour
    {
        #region Variables

        public StandingState standing;
        public DuckingState ducking;
        public JumpingState jumping;
        public DivingState diving;
        public ShootingState shooting;
        public MeleeState melee;
        public Stack<State> movementStates = new Stack<State>();
        public Stack<State> equipmentStates = new Stack<State>();

#pragma warning disable 0649
        [SerializeField]
        private Transform handTransform;
        [SerializeField]
        private Transform sheathTransform;
        [SerializeField]
        private CharacterData data;
        [SerializeField]
        private LayerMask whatIsGround;
        [SerializeField]
        private Collider hitBox;
        [SerializeField]
        private Animator anim;
#pragma warning restore 0649
        [SerializeField]
        private float meleeRestThreshold = 10f;
        [SerializeField]
        private float diveThreshold = 1f;
        [SerializeField]
        private float collisionOverlapRadius = 0.1f;

        private GameObject currentWeapon;
        private int horizonalMoveParam = Animator.StringToHash("H_Speed");
        private int verticalMoveParam = Animator.StringToHash("V_Speed");
        private int crouchParam = Animator.StringToHash("Crouch");
        #endregion

        #region Properties

        public float NormalColliderHeight => data.normalColliderHeight;
        public float CrouchColliderHeight => data.crouchColliderHeight;
        public float DiveForce => data.diveForce;
        public float JumpForce => data.jumpForce;
        public float MovementSpeed => data.movementSpeed;
        public float CrouchSpeed => data.crouchSpeed;
        public float RotationSpeed => data.rotationSpeed;
        public float CrouchRotationSpeed => data.crouchRotationSpeed;
        public GameObject MeleeWeapon => data.meleeWeapon;
        public GameObject ShootableWeapon => data.staticShootable;
        public float CollisionOverlapRadius => collisionOverlapRadius;
        public float DiveThreshold => diveThreshold;
        public float MeleeRestThreshold => meleeRestThreshold;

        public float ColliderSize
        {
            get
            {
                return GetComponent<CapsuleCollider>().height;
            }

            set
            {
                GetComponent<CapsuleCollider>().height = value;
                var center = GetComponent<CapsuleCollider>().center;
                center.y = value / 2f;
                GetComponent<CapsuleCollider>().center = center;
            }
        }

        #endregion

        #region Methods

        public void RestrictRotation()
        {
            var currentRotation = transform.rotation;
            currentRotation.x = currentRotation.z = 0f;
            transform.rotation = currentRotation;
        }

        public void Move(float speed, float rotationSpeed)
        {
            var targetVelocity = speed * transform.forward * Time.deltaTime;
            targetVelocity.y = GetComponent<Rigidbody>().velocity.y;
            GetComponent<Rigidbody>().velocity = targetVelocity;

            GetComponent<Rigidbody>().angularVelocity = rotationSpeed * Vector3.up * Time.deltaTime;

            anim.SetFloat(horizonalMoveParam, GetComponent<Rigidbody>().angularVelocity.y);
            anim.SetFloat(verticalMoveParam, speed * Time.deltaTime);
        }

        public void ApplyImpulse(Vector3 force)
        {
            GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
        }

        public void SetCrouchAnimation(bool value)
        {
            anim.SetBool(crouchParam, value);
        }

        public void TriggerAnimation(int param)
        {
            anim.SetTrigger(param);
        }

        public void Shoot()
        {
            Instantiate(data.shootableObject, handTransform.transform.position, handTransform.transform.rotation);
        }

        public bool CheckCollisionOverlap(Vector3 point)
        {
            return Physics.OverlapSphere(point, CollisionOverlapRadius, whatIsGround).Length > 0;
        }

        public void Equip(GameObject weapon = null)
        {
            if (weapon != null)
            {
                currentWeapon = Instantiate(weapon, handTransform, false);
            }
            else
            {
                ParentCurrentWeapon(handTransform);
            }
        }

        public void SheathWeapon()
        {
            ParentCurrentWeapon(sheathTransform);
        }

        public void Unequip()
        {
            Destroy(currentWeapon);
        }

        public void ActivateHitBox()
        {
            hitBox.enabled = true;
        }

        public void DeactivateHitBox()
        {
            hitBox.enabled = false;
        }

        private void ParentCurrentWeapon(Transform parent)
        {
            currentWeapon.transform.SetParent(sheathTransform);
            currentWeapon.transform.localPosition = Vector3.zero;
            currentWeapon.transform.localRotation = Quaternion.identity;
        }
        #endregion

        #region MonoBehaviour Callbacks

        private void Start()
        {
            standing = new StandingState(this);
            ducking = new DuckingState(this);
            jumping = new JumpingState(this);
            diving = new DivingState(this);
            shooting = new ShootingState(this);
            melee = new MeleeState(this);

            State.ChangeState(standing, movementStates);
            State.ChangeState(melee, equipmentStates);
        }

        private void Update()
        {
            movementStates.Peek().HandleInput();
            equipmentStates.Peek().HandleInput();

            movementStates.Peek().LogicUpdate();
            equipmentStates.Peek().LogicUpdate();
        }

        private void FixedUpdate()
        {
            movementStates.Peek().PhysicsUpdate();
            equipmentStates.Peek().PhysicsUpdate();
        }
        #endregion
    }
}