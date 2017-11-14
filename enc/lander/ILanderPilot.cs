namespace enc.lander
{
    public interface ILanderPilot
    {
        Lander Lander { get; }

        void Process();
    }
}