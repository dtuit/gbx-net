using GBX.NET;
using GBX.NET.Engines.Game;
using System.Text;


string Vec3ToString(Vec3 vec) {
    return $"{vec.X}-{vec.Y}-{vec.Z}";
}
string QuatToString(Quat quat)
{
    return $"{quat.X}-{quat.Y}-{quat.Z}-{quat.W}";
}
string ByteArrayToString(byte[] bytes)
{
    var sb = new StringBuilder("");
    foreach (var b in bytes)
    {
        sb.Append(b + ", ");
    }
    return sb.ToString();
}


//var filepath = "C:/Users/darren/Documents/Trackmania/Replays/AutosavedGhosts/Winter 2023 - 03/2023-01-02 06-01-56-Winter 2023 - 03-bolideblazar-25508ms.Replay.gbx";
//var filepath = "C:\\Users\\darren\\Documents\\Trackmania\\Replays\\My Replays\\WheelStateTest_bolideblazar_9-01-2023_2-24-56-am(00'10''938).Replay.Gbx";
// var filepath = "C:\\Users\\darren\\Documents\\Trackmania\\Replays\\My Replays\\WheelStateTestWater_bolideblazar_9-01-2023_2-55-23-am(00'12''417).Replay.Gbx";
// Skid first
//var filepath = "C:\\Users\\darren\\Documents\\Trackmania\\Replays\\My Replays\\WheelStateTest_bolideblazar_9-01-2023_3-02-49-am(00'14''577).Replay.Gbx";
// Looking for Slip coef
//var filepath = "C:\\Users\\darren\\Documents\\Trackmania\\Replays\\My Replays\\WheelStateTest_bolideblazar_9-01-2023_3-14-37-am(00'13''976).Replay.Gbx";
// Action blocks
//var filepath = "C:\\Users\\darren\\Documents\\Trackmania\\Replays\\My Replays\\WheelStateTestActionBlocks_bolideblazar_9-01-2023_9-48-08-pm(00'07''968).Replay.Gbx";
// Action with Dirt and Ice between
//var filepath = "C:\\Users\\darren\\Documents\\Trackmania\\Replays\\My Replays\\WheelStateTestActionBlocks_bolideblazar_9-01-2023_10-04-38-pm(00'07''998).Replay.Gbx";

// Airbrake and ReactorFlight
//var filepath = "C:\\Users\\darren\\Documents\\Trackmania\\Replays\\My Replays\\WheelStateTestAirBrake.Replay.Gbx";

// Airbrake and ReactorFlight - Dirt
//var filepath = "C:\\Users\\darren\\Documents\\Trackmania\\Replays\\My Replays\\WheelStateTestAirBrakeDirt.Replay.Gbx";

// Ground contact staircase
//var filepath = "C:\\Users\\darren\\Documents\\Trackmania\\Replays\\My Replays\\WheelStateTestGroundContact.Replay.Gbx";

// Ground contact dirtbumps
//var filepath = "C:\\Users\\darren\\Documents\\Trackmania\\Replays\\My Replays\\WheelStateTestGroundContact2.Replay.Gbx";

//var filepath = "C:\\Users\\darren\\Documents\\Trackmania\\Replays\\My Replays\\WheelStateTestGroundContact3.Replay.Gbx";

//var filepath = "C:\\Users\\darren\\Documents\\Trackmania\\Replays\\My Replays\\WheelStateTest_bolideblazar_9-01-2023_3-14-37-am(00'13''976).Replay.Gbx";

//var filepath = "C:\\Users\\darren\\Documents\\Trackmania\\Replays\\My Replays\\WheelStateTestActionBlocks_DirtIceWaterRoadBetweenWithSlip.Replay.Gbx";
//var filepath = "C:\\Users\\darren\\Documents\\Trackmania\\Replays\\My Replays\\WheelStateTest_DirtIceGrassPlasticWaterRoadBetween_WithSlip.Replay.Gbx";

//var filepath = "C:\\Users\\darren\\Documents\\Trackmania\\Replays\\My Replays\\WheelStateTestGroundContact3.Replay.Gbx";

//var filepath = "C:\\Users\\darren\\Documents\\Trackmania\\Replays\\My Replays\\WheelStateTest_DirtIceGrassPlasticWaterRoadBetween_WhichWheelIsWhich.Replay.Gbx";

//var filepath = "C:\\Users\\darren\\Documents\\Trackmania\\Replays\\My Replays\\WheelStateTest_SlipUsingBreakOnlyRoad.Replay.Gbx";

//var filepath = "C:\\Users\\darren\\Documents\\Trackmania\\Replays\\My Replays\\WheelStateTest_SlipGrassAndRow_SingleTireSlip.Gbx";

//var filepath = "C:\\Users\\darren\\Documents\\Trackmania\\Replays\\My Replays\\WheelStateTest_Rotation.Gbx";

//var filepath = "C:\\Users\\darren\\Documents\\Trackmania\\Replays\\My Replays\\WheelStateTest_RotationFrontNoGround.Gbx";

//var filepath = "C:\\Users\\darren\\Documents\\Trackmania\\Replays\\My Replays\\WheelStateTestAirBrakeDirt.Replay.Gbx";

//var filepath = "C:\\Users\\darren\\Documents\\Trackmania\\Replays\\My Replays\\WheelStateTest_ReactorUpDownLv1Lv2.Replay.Gbx";

//var filepath = "C:\\Users\\darren\\Documents\\Trackmania\\Replays\\My Replays\\WheelStateTest_ReactorUpDownLv1Lv2.Replay.Gbx";

//var filepath = "C:\\Users\\darren\\Documents\\Trackmania\\Replays\\My Replays\\WheelStateTest_TurboTurbo2Turbo3ReactorUpReactorDownReactor2UpReactor2Down.Replay.Gbx";

//var filepath = "C:\\Users\\darren\\Documents\\Trackmania\\Replays\\My Replays\\WheelStateTest_CruiseControlNoBrakesNoEngineNoSteeringSlowMoFragile.Replay.Gbx";

//var filepath = "C:\\Users\\darren\\Documents\\Trackmania\\Replays\\My Replays\\WheelStateTest_ReactorUpAndDownTest.Replay.Gbx";

//var filepath = "C:\\Users\\darren\\Documents\\Trackmania\\Replays\\My Replays\\WheelStateTest_ReactorUpAndDownTest_2.Gbx";

//var filepath = "C:\\Users\\darren\\Documents\\Trackmania\\Replays\\My Replays\\WheelStateTest_ReactorUpLvl2AirControls.Replay.Gbx";

//var filepath = "C:\\Users\\darren\\Documents\\Trackmania\\Replays\\My Replays\\WheelStateTest_ReactorUpLvl2AirControls_AllCombos.Replay.Gbx";

//var filepath = "C:\\Users\\darren\\Documents\\Trackmania\\Replays\\My Replays\\WheelStateTest_WheelRotation_StartStop.Gbx";

//var filepath = "C:\\Users\\darren\\Documents\\Trackmania\\Replays\\My Replays\\WheelStateTest_WheelRotation_BackForward.Gbx";

//var filepath = "C:\\Users\\darren\\Documents\\Trackmania\\Replays\\My Replays\\WheelStateTest_SideSpeed_AND_TopContact.Replay.Gbx";

var filepath = "C:\\Users\\darren\\Documents\\Trackmania\\Replays\\My Replays\\WheelStateTest_BoostAndSpin.Replay.Gbx";

//var filepath = "C:\\Users\\darren\\Documents\\Trackmania\\Replays\\My Replays\\WheelStateTest_CameraChangeQ.Gbx";

//var filepath = "C:\\Users\\darren\\Documents\\TmForever\\Tracks\\Replays\\Autosaves\\darren_A05-Race.Replay.gbx";

var replay = GameBox.Parse<CGameCtnReplayRecord>(filepath);

//var ghosts = (List<CGameCtnGhost>)replay.Node.GetGhosts();
//var samples = ghosts?[0].RecordData?.Samples;

var samples = replay.Node?.Ghosts?.First()?.RecordData?.Samples;
    
//var samples = replay.Node.Ghosts[0].SampleData.Samples;
//var samples = replay.Node?.Ghosts?[0].RecordData.Samples;

string fileName = Path.GetFileNameWithoutExtension(filepath);
string outfilePath = $"C:\\Users\\darren\\code\\gbx-net-ghostdebug-data\\{fileName}_3.csv";

using StreamWriter file = new(outfilePath);

foreach (Sample sample in samples) {
    //Console.WriteLine($"5:1 RPM {rpmByte} 14:1 Steer {steerByte}-{steer} 91:1 Gear {gearByte}-{gear} 15:1 u15 {u15} 18:1 brake/gas {brakeByte}-{brake}/{gas} 47:22 prsv {position}-{rotation}-{speed}-{velocity}");
    var parsedVals = $"{sample.BufferType}, {Vec3ToString(sample.Position)}, {QuatToString(sample.Rotation)}, {Vec3ToString(sample.PitchYawRoll)}, {sample.Speed}, {Vec3ToString(sample.Velocity)}, {sample.Gear}, {sample.RPM}, {sample.Steer}, {sample.Brake}, {sample.Gas}, {sample.FLIcing}, {sample.RLIcing}, {sample.RRIcing}, {sample.FLDirt}, {sample.FRDirt}, {sample.RLDirt}, {sample.RRDirt}, {sample.FLGroundContactMaterial}, {sample.FRGroundContactMaterial}, {sample.RLGroundContactMaterial}, {sample.RRGroundContactMaterial}, {sample.FLDampenLen}, {sample.FRDampenLen}, {sample.RLDampenLen}, {sample.RRDampenLen}, {sample.FLSlipCoef}, {sample.FRSlipCoef}, {sample.RLSlipCoef}, {sample.RRSlipCoef}, {sample.WetnessValue}, {sample.IsGroundContact}, {sample.IsReactorGroundMode}, {sample.SideSpeed}";
    file.WriteLine($"{sample.Timestamp},{sample.Data.Length}, {ByteArrayToString(sample.Data)},{parsedVals}");
}

Console.WriteLine("testing");