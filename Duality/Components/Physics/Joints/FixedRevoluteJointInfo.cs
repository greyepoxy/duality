﻿using System;

using OpenTK;

using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Dynamics.Joints;

using Duality.Editor;
using Duality.Resources;

namespace Duality.Components.Physics
{
	/// <summary>
	/// Pins a local anchor on the the RigidBody to a specific world position without constraining rotation.
	/// </summary>
	[Serializable]
	public sealed class FixedRevoluteJointInfo : JointInfo
	{
		private	Vector2		localAnchor		= Vector2.Zero;
		private	Vector2		worldAnchor		= Vector2.Zero;
		private	float		lowerLimit		= 0.0f;
		private	float		upperLimit		= 0.0f;
		private	bool		limitEnabled	= false;
		private	float		refAngle		= 0.0f;
		private	bool		motorEnabled	= false;
		private float		maxMotorTorque	= 0.0f;
		private float		motorSpeed		= 0.0f;


		public override bool DualJoint
		{
			get { return false; }
		}
		/// <summary>
		/// [GET / SET] The Colliders local anchor point.
		/// </summary>
		[EditorHintIncrement(1)]
		public Vector2 LocalAnchor
		{
			get { return this.localAnchor; }
			set { this.localAnchor = value; this.UpdateJoint(); }
		}
		/// <summary>
		/// [GET / SET] The world anchor point to which the Collider will be attached.
		/// </summary>
		[EditorHintIncrement(1)]
		public Vector2 WorldAnchor
		{
			get { return this.worldAnchor; }
			set { this.worldAnchor = value; this.UpdateJoint(); }
		}
		/// <summary>
		/// [GET / SET] Is the joint limited in its angle?
		/// </summary>
		public bool LimitEnabled
		{
			get { return this.limitEnabled; }
			set { this.limitEnabled = value; this.UpdateJoint(); }
		}
		/// <summary>
		/// [GET / SET] The lower joint limit in radians.
		/// </summary>
		[EditorHintIncrement(MathF.RadAngle1)]
		public float LowerLimit
		{
			get { return this.lowerLimit; }
			set { this.lowerLimit = MathF.Min(value, this.upperLimit); this.UpdateJoint(); }
		}
		/// <summary>
		/// [GET / SET] The upper joint limit in radians.
		/// </summary>
		[EditorHintIncrement(MathF.RadAngle1)]
		public float UpperLimit
		{
			get { return this.upperLimit; }
			set { this.upperLimit = MathF.Max(value, this.lowerLimit); this.UpdateJoint(); }
		}
		/// <summary>
		/// [GET / SET] The joint's reference angle.
		/// </summary>
		[EditorHintIncrement(MathF.RadAngle1)]
		public float ReferenceAngle
		{
			get { return this.refAngle; }
			set { this.refAngle = value; this.UpdateJoint(); }
		}
		/// <summary>
		/// [GET / SET] Is the joint motor enabled?
		/// </summary>
		public bool MotorEnabled
		{
			get { return this.motorEnabled; }
			set { this.motorEnabled = value; this.UpdateJoint(); }
		}
		/// <summary>
		/// [GET / SET] The maximum motor torque.
		/// </summary>
		public float MaxMotorTorque
		{
			get { return this.maxMotorTorque; }
			set { this.maxMotorTorque = value; this.UpdateJoint(); }
		}
		/// <summary>
		/// [GET / SET] The desired motor speed in degree per frame.
		/// </summary>
		[EditorHintIncrement(MathF.RadAngle1)]
		public float MotorSpeed
		{
			get { return MathF.RadToDeg(this.motorSpeed); }
			set { this.motorSpeed = MathF.DegToRad(value); this.UpdateJoint(); }
		}
		/// <summary>
		/// [GET] The current joint angle speed in radians per frame.
		/// </summary>
		[EditorHintFlags(MemberFlags.Invisible)]
		public float JointSpeed
		{
			get { return this.joint == null ? 0.0f : (PhysicsUnit.AngularVelocityToDuality * (this.joint as FixedRevoluteJoint).JointSpeed); }
		}
		/// <summary>
		/// [GET] The current joint angle in radians.
		/// </summary>
		[EditorHintFlags(MemberFlags.Invisible)]
		public float JointAngle
		{
			get { return this.joint == null ? 0.0f : (PhysicsUnit.AngleToDuality * (this.joint as FixedRevoluteJoint).JointAngle); }
		}
		/// <summary>
		/// [GET] The current joint motor torque.
		/// </summary>
		[EditorHintFlags(MemberFlags.Invisible)]
		public float MotorTorque
		{
			get { return this.joint == null ? 0.0f : (PhysicsUnit.TorqueToDuality * (this.joint as FixedRevoluteJoint).MotorTorque); }
		}


		protected override Joint CreateJoint(Body bodyA, Body bodyB)
		{
			return bodyA != null ? JointFactory.CreateFixedRevoluteJoint(Scene.PhysicsWorld, bodyA, Vector2.Zero, Vector2.Zero) : null;
		}
		internal override void UpdateJoint()
		{
			base.UpdateJoint();
			if (this.joint == null) return;

			FixedRevoluteJoint j = this.joint as FixedRevoluteJoint;
			j.WorldAnchorB = PhysicsUnit.LengthToPhysical * this.worldAnchor;
			j.LocalAnchorA = GetFarseerPoint(this.BodyA, this.localAnchor);
			j.MotorEnabled = this.motorEnabled;
			j.MotorSpeed = -this.motorSpeed / Time.SPFMult;
			j.MaxMotorTorque = PhysicsUnit.TorqueToPhysical * this.maxMotorTorque;
			j.LimitEnabled = this.limitEnabled;
			j.LowerLimit = -this.upperLimit;
			j.UpperLimit = -this.lowerLimit;
			j.ReferenceAngle = -this.refAngle;
		}
	}
}
