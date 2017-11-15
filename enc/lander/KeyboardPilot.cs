using Microsoft.Xna.Framework.Input;

namespace enc.lander
{
    internal class KeyboardPilot : LanderPilot
    {
        public override void Process()
        {
            Lander.ThrustLeft(Keyboard.GetState().IsKeyDown(Keys.A) ? 1 : 0);
            Lander.ThrustRight(Keyboard.GetState().IsKeyDown(Keys.D) ? 1 : 0);
            Lander.ThrustUp(Keyboard.GetState().IsKeyDown(Keys.W) ? 1 : 0);
        }
    }
}