namespace Configs
{
    public interface ICarConfig
    {
        public float MaxSuspentionLength { get; }
        public float SuspentionMultiplier { get; }
        public float DampSensitivity { get; }
        public float WheelRadius { get; }
        public float MaxAntiCapapultForce { get; }
        public float CustomGravity { get; }
        public float BumpStopMultiplier { get; }
        public float CoastDampingMultiplier { get; }
        public float MinWheelLocalY { get; }
        public float MaxWheelLocalY { get; }
        public float WheelSmoothSpeed { get; }
        public float MotorForce { get; }
        public float MotorKickForce { get; }
        public float MaxSpeed { get; }
        public float AirPitchForce { get; }
        public float AirAngularDamping { get; }
        public float AirSpinSpeed { get; }
        public float WheelAirBrakeSpeed { get; }
    }

}