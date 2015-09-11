open System
open System.Threading
open KNXLib

let logDevice device state =
    async {
        match device.Type with
        | Switch -> logSwitch device state
        | Toggle -> logToggle device state
        | Duration -> logDuration device state
        | Current -> logCurrent device state
        | Temperature -> logTemperature device state
        | Light -> logLight device state
        | Speed -> logSpeed device state
    }

let log address state =
    let device =
        devices
        |> List.tryFind (fun x -> x.Address = address)

    match device with
    | Some x -> Async.Start(logDevice x state)
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
