namespace enc.lander
{
    public abstract class LanderPilot
    {
        public Lander Lander { get; set; }

        abstract public void Process();
        
    }
}