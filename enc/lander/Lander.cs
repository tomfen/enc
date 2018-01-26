using Microsoft.Xna.Framework;
using System;
using VelcroPhysics.Dynamics;
using VelcroPhysics.Dynamics.Joints;
using VelcroPhysics.Factories;
using VelcroPhysics.Utilities;

namespace enc.lander
{
    public class Lander
    {
        private World world;

        public Body Vessel { get; private set; }
        public Body LegLeft { get; private set; }
        public Body LegRight { get; private set; }
        public WeldJoint JointLeft { get; private set; }
        public WeldJoint JointRight { get; private set; }
        public bool LeftLegBroken { get; private set; }
        public bool RightLegBroken { get; private set; }

        public float fuel = 5000;
        public float damage = 0;

        public float ThrustUpValue { get; private set; }
        public float ThrustLeftValue { get; private set; }
        public float ThrustRightValue { get; private set; }

        private bool landedLegLeft = false;
        private bool landedLegRight = false;

        private readonly static Vector2[] shape = new Vector2[] {
            new Vector2(-0.4f, -0.5f),
            new Vector2(-0.6f, 0.5f),
            new Vector2(0.6f, 0.5f),
            new Vector2(0.4f, -0.5f)};

    public Lander(World world, Vector2 position, float tilt)
        {
            this.world = world;

            Vessel = BodyFactory.CreatePolygon(world,
                new VelcroPhysics.Shared.Vertices(shape), 1.0f, position, bodyType: BodyType.Dynamic);

            Vector2 legOffsetL = new Vector2(-0.6f, 0.5f);
            Vector2 legOffsetR = new Vector2(+0.6f, 0.5f);

            LegLeft = BodyFactory.CreateRectangle(world,
                0.1f, 0.8f, 1.0f,
                Vessel.WorldCenter + legOffsetL, +0.2f, BodyType.Dynamic);

            LegRight = BodyFactory.CreateRectangle(world,
                0.1f, 0.8f, 1.0f,
                Vessel.WorldCenter + legOffsetR, -0.2f, BodyType.Dynamic);

            JointLeft = JointFactory.CreateWeldJoint(world,
                LegLeft, Vessel, -legOffsetL, Vessel.LocalCenter);

            JointRight = JointFactory.CreateWeldJoint(world,
                LegRight, Vessel, -legOffsetR, Vessel.LocalCenter); 
            

            JointLeft.FrequencyHz = 60f;
            JointLeft.DampingRatio = 30;
            JointLeft.Breakpoint = 15;

            JointRight.FrequencyHz = 60f;
            JointRight.DampingRatio = 30;
            JointRight.Breakpoint = 15;

            JointLeft.Broke += (J, F) => { LeftLegBroken = true; damage += 100; };
            JointRight.Broke += (J, F) => { RightLegBroken = true; damage += 100; };
            Vessel.OnCollision += (me, hit, contact) =>
            {
                var points = contact.Manifold.Points;
                foreach (var contactPoint in points)
                {
                    Vector2 contactPosition = contactPoint.LocalPoint;

                    Vector2 v0 = me.Body.GetLinearVelocityFromWorldPoint(contactPosition);

                    Vector2 v1 = hit.Body.IsStatic ?
                        new Vector2(0) :
                        hit.Body.GetLinearVelocityFromWorldPoint(contactPosition);


                    float hitVelocity = (v0 - v1).Length();
                    damage += hitVelocity * 10;
                }
            };

            RotateAroundPoint(Vessel.WorldCenter, tilt, new Body[] { LegLeft, LegRight, Vessel });

            Vessel.SleepingAllowed = false;
            LegLeft.SleepingAllowed = false;
            LegRight.SleepingAllowed = false;

            LegLeft.OnCollision += (A, B, C) => { if (B.Body != Vessel) landedLegLeft = true; };
            LegRight.OnCollision += (A, B, C) => { if (B.Body != Vessel) landedLegRight = true; };

            LegLeft.OnSeparation += (A, B, C) => { if (B.Body != Vessel) landedLegLeft = false; };
            LegRight.OnSeparation += (A, B, C) => { if (B.Body != Vessel) landedLegRight = false; };
        }

        private void RotateAroundPoint(Vector2 pivot, float angle, Body[] bodies)
        {
            var cosAngle = (float)Math.Cos(angle);
            var sinAngle = (float)Math.Sin(angle);

            foreach(var body in bodies)
            {
                var distX = body.WorldCenter.X - pivot.X;
                var distY = body.WorldCenter.Y - pivot.Y;
                var newPoint = new Vector2(cosAngle * distX - sinAngle * distY + pivot.X,
                                           cosAngle * distY + sinAngle * distX + pivot.Y);
                body.SetTransform(newPoint, angle);
            }
        }

        public void ThrustLeft(float value)
        {
            value = MathUtils.Clamp(value, 0, 1);
            value *= -1f;

            if (fuel > 0)
            {
                Vector2 ThrustVector = Vessel.GetWorldVector(new Vector2(value, 0));

                Vessel.ApplyForce(ThrustVector, Vessel.GetWorldPoint(new Vector2(0.4f, -0.5f)));

                fuel -= Math.Abs(value);

                ThrustLeftValue = Math.Abs(value);
            }
            else
            {
                ThrustLeftValue = 0;
            }
        }

        public void ThrustRight(float value)
        {
            value = MathUtils.Clamp(value, 0, 1);
            value *= 1f;

            if (fuel > 0)
            {
                Vector2 ThrustVector = Vessel.GetWorldVector(new Vector2(value, 0));

                Vessel.ApplyForce(ThrustVector, Vessel.GetWorldPoint(new Vector2(-0.4f, -0.5f)));

                fuel -= Math.Abs(value);

                ThrustRightValue = Math.Abs(value);
            }
            else
            {
                ThrustRightValue = 0;
            }
        }

        public void ThrustUp(float value)
        {
            value = MathUtils.Clamp(value, 0, 1);
            value *= -15f;

            if (fuel > 0)
            {
                Vector2 ThrustVector = Vessel.GetWorldVector(new Vector2(0, value));

                Vessel.ApplyForce(ThrustVector, Vessel.GetWorldPoint(new Vector2(0f, 1)));

                fuel -= Math.Abs(value);

                ThrustUpValue = Math.Abs(value);
            }
            else
            {
                ThrustUpValue = 0;
            }
        }

        public void Destroy()
        {
            world.RemoveJoint(JointLeft);
            world.RemoveJoint(JointRight);
            world.RemoveBody(LegLeft);
            world.RemoveBody(LegRight);
            world.RemoveBody(Vessel);
        }

        public Vector2 WorldCenter
        {
            get { return Vessel.WorldCenter; }
        }

        public bool IsLanded()
        {
            return landedLegLeft && landedLegRight
                && !LeftLegBroken && !RightLegBroken;
        }
    }
}
