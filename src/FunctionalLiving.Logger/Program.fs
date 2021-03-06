open System
open System.Threading
open KNXLib
open KNXLib.Events

let logDevice device state =
    async {
        try
            match device.Type with
            | Switch -> logSwitch device state
            | Toggle -> logToggle device state
            | Duration -> logDuration device state
            | Current -> logCurrent device state
            | Temperature -> logTemperature device state
            | Light -> logLight device state
            | Speed -> logSpeed device state
            | EnergyWh -> logEnergyWh device state
            | EnergyKWh -> logEnergyKWh device state
        with
        | _ -> ()
    }

let log sender (args: KnxEventArgs) =
    let device =
        devices
        |> List.tryFind (fun x -> x.Address = args.DestinationAddress.ToString())

    match device with
    | Some x -> Async.Start(logDevice x args.State)
    | None -> ()

let connect = fun reconnect ->
    let connection =
        new KnxConnectionRouting(ActionMessageCode = 0x29uy,
                                 KnxConnectedDelegate = KnxConnection.KnxConnected(fun _ -> sprintf "Connected!" |> logToConsole),
                                 KnxDisconnectedDelegate = KnxConnection.KnxDisconnected(reconnect),
                                 KnxEventDelegate = KnxConnection.KnxEvent(log),
                                 KnxStatusDelegate = KnxConnection.KnxStatus(log))

    connection.Connect()

let rec reconnect = fun _ ->
    sprintf "Disconnected! Reconnecting..." |> logToConsole
    Thread.Sleep(1000)
    connect reconnect  |> ignore

[<EntryPoint>]
let main argv =
    System.Net.ServicePointManager.DefaultConnectionLimit <- 1
    System.Net.ServicePointManager.Expect100Continue <- false

    sprintf "Connecting..." |> logToConsole

    connect reconnect |> ignore

    Console.Read() |> ignore

    0
