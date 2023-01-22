namespace GBX.NET;

// Ref: https://next.openplanet.dev/Scene/CSceneVehicleVisState
public enum EPlugSurfaceMaterialId : byte
{
    Concrete,
    Pavement,
    Grass,
    Ice,
    Metal,
    Sand,
    Dirt,
    Turbo_Deprecated,
    DirtRoad,
    Rubber,
    SlidingRubber,
    Test,
    Rock,
    Water,
    Wood,
    Danger,
    Asphalt,
    WetDirtRoad,
    WetAsphalt,
    WetPavement,
    WetGrass,
    Snow,
    ResonantMetal,
    GolfBall,
    GolfWall,
    GolfGround,
    Turbo2_Deprecated,
    Bumper_Deprecated,
    NotCollidable,
    FreeWheeling_Deprecated,
    TurboRoulette_Deprecated,
    WallJump,
    MetalTrans,
    Stone,
    Player,
    Trunk,
    TechLaser,
    SlidingWood,
    PlayerOnly,
    Tech,
    TechArmor,
    TechSafe,
    OffZone,
    Bullet,
    TechHook,
    TechGround,
    TechWall,
    TechArrow,
    TechHook2,
    Forest,
    Wheat,
    TechTarget,
    PavementStair,
    TechTeleport,
    Energy,
    TechMagnetic,
    TurboTechMagnetic_Deprecated,
    Turbo2TechMagnetic_Deprecated,
    TurboWood_Deprecated,
    Turbo2Wood_Deprecated,
    FreeWheelingTechMagnetic_Deprecated,
    FreeWheelingWood_Deprecated,
    TechSuperMagnetic,
    TechNucleus,
    TechMagneticAccel,
    MetalFence,
    TechGravityChange,
    TechGravityReset,
    RubberBand,
    Gravel,
    Hack_NoGrip_Deprecated,
    Bumper2_Deprecated,
    NoSteering_Deprecated,
    NoBrakes_Deprecated,
    RoadIce,
    RoadSynthetic,
    Green,
    Plastic,
    DevDebug,
    Free3,
    XXX_Null,
}

public class Sample
{
    public TimeInt32? Timestamp { get; internal set; }

    public byte? BufferType { get; set; }
    public Vec3 Position { get; set; }
    public Quat Rotation { get; set; }
    public Vec3 PitchYawRoll => Rotation.ToPitchYawRoll();
    public float Speed { get; set; }
    public Vec3 Velocity { get; set; }
    public float? Gear { get; set; }
    public byte? RPM { get; set; }
    public float? Steer { get; set; }
    public float? Brake { get; set; }
    public float? Gas { get; set; }

    public byte[] Data { get; set; }

    public float FLIcing { get; set; }
    public float FRIcing { get; set; }
    public float BLIcing { get; set; }
    public float BRIcing { get; set; }

    public float FLDirt { get; set; }
    public float FRDirt { get; set; }
    public float BLDirt { get; set; }
    public float BRDirt { get; set; }

    public EPlugSurfaceMaterialId FLGroundContactMaterial { get; set; }
    public EPlugSurfaceMaterialId FRGroundContactMaterial { get; set; }
    public EPlugSurfaceMaterialId BLGroundContactMaterial { get; set; }
    public EPlugSurfaceMaterialId BRGroundContactMaterial { get; set; }

    public float WetnessValue { get; set; }
    public bool IsGroundContact { get; set; }
    public bool IsReactorGroundMode { get; set; }

    public Sample(byte[] data)
    {
        Data = data;
    }

    public override string ToString()
    {
        if (!BufferType.HasValue || BufferType == 0 || BufferType == 2 || BufferType == 4)
        {
            if (Timestamp.HasValue)
                return $"Sample: {Timestamp.ToTmString()} {Position}";
            return $"Sample: {Position}";
        }

        return $"Sample: {BufferType.ToString() ?? "unknown"}";
    }
}
