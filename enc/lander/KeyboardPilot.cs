using Microsoft.Xna.Framework.Input;

namespace enc.lander
{
    internal class KeyboardPilot : ILanderPilot
    {
        private Lander _lander;

        public KeyboardPilot(Lander lander)
        {
            _lander = lander;
        }
        
        Lander ILanderPilot.Lander => _lander;

        public void Process()
        {
            _lander.ThrustLeft(Keyboard.GetState().IsKeyDown(Keys.A) ? 1 : 0);
            _lander.ThrustRight(Keyboard.GetState().IsKeyDown(Keys.D) ? 1 : 0);
        }
    }
}