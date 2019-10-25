namespace GearSim.Objects
{
    public sealed class GearConnection
    {
        public GearConnection(Gear child, float jointAngle)
        {
            this.Child = child;
            this.JointAngle = jointAngle;
        }

        public Gear Child { get; }
        public float JointAngle { get; }
    }
}
