using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        
        private readonly static Vector2[] shape = new Vector2[] {
                    new Vector2(0.1f, 0),
                new Vector2(-0.1f,1),
                new Vector2(1.1f, 1),
                new Vector2(0.9f,0)};

    public Lander(World world, Vector2 position)
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

            JointLeft.FrequencyHz = 10f;
            JointLeft.DampingRatio = 6;

            JointRight.FrequencyHz = 10f;
            JointRight.DampingRatio = 6;
        }

        public void ThrustLeft(float value)
        {
            value = MathUtils.Clamp(value, 0, 1);

            Vector2 ThrustVector = Vessel.GetWorldVector(new Vector2(0, -5.5f * value));
            
            Vessel.ApplyForce(ThrustVector, Vessel.GetWorldPoint(new Vector2(0, 0)));
        }

        public void ThrustRight(float value)
        {
            value = MathUtils.Clamp(value, 0, 1);

            Vector2 ThrustVector = Vessel.GetWorldVector(new Vector2(0, -5.5f * value));

            Vessel.ApplyForce(ThrustVector, Vessel.GetWorldPoint(new Vector2(1, 0)));
        }

        public void Destroy()
        {
            world.RemoveJoint(JointLeft);
            world.RemoveJoint(JointRight);
            world.RemoveBody(LegLeft);
            world.RemoveBody(LegRight);
            world.RemoveBody(Vessel);
        }
    }
}
