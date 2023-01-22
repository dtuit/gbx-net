using GBX.NET;
using GBX.NET.Engines.Game;


// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");


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

var filepath = "C:\\Users\\darren\\Documents\\Trackmania\\Replays\\My Replays\\WheelStateTest_bolideblazar_9-01-2023_3-14-37-am(00'13''976).Replay.Gbx";


var replay = GameBox.Parse<CGameCtnReplayRecord>(filepath);

//var samples = replay.Node.Ghosts[0].SampleData.Samples;

var samples = replay.Node.Ghosts[0].RecordData.Samples;

Console.WriteLine("testing");