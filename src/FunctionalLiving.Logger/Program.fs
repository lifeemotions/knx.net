open System
open System.Threading
open KNXLib
open InfluxDB.FSharp
open FunctionalLiving.Parser.Domain
open FunctionalLiving.Parser.Category1_SingleBit
open FunctionalLiving.Parser.Category9_2ByteFloatValue

type DatapointType =
| Switch
| Toggle
| Percentage
| Time
| Current
| Temperature
| Light
| Speed
| TimeOfDay
| Date
| EnergyWh
| EnergyKWh

type Device = { Address: string; Type: DatapointType; Description: string; }

type PointData =
    { Measurement: Measurement
      Tags: Map<string,string>
      Fields: Map<string, FieldValue>
      Timestamp: DateTime }

let devices =
    [
        // 1.001 Switches
        { Address = "0/1/0"; Type = DatapointType.Switch; Description = "Verlichting - Leefruimte - Spots TV - Aan/Uit" }
        { Address = "0/1/22"; Type = DatapointType.Switch; Description = "Verlichting - Bureau - Centraal - Aan/Uit" }

        // 9.001 Temperate (degrees C)
        { Address = "0/4/0"; Type = DatapointType.Temperature; Description = "Inkomhal (Plafond)" }
        { Address = "0/4/1"; Type = DatapointType.Temperature; Description = "Toilet beneden (Plafond)" }
        { Address = "0/4/2"; Type = DatapointType.Temperature; Description = "Berging (Plafond)" }
        { Address = "0/4/3"; Type = DatapointType.Temperature; Description = "Garage (Plafond)" }
        { Address = "0/4/4"; Type = DatapointType.Temperature; Description = "Nachthal trap (Plafond)" }
        { Address = "0/4/5"; Type = DatapointType.Temperature; Description = "Nachthal badkamer (Plafond)" }
        { Address = "0/4/6"; Type = DatapointType.Temperature; Description = "Badkamer (Plafond)" }
        { Address = "0/4/7"; Type = DatapointType.Temperature; Description = "Toilet boven (Plafond)" }
        { Address = "0/4/8"; Type = DatapointType.Temperature; Description = "Slaapkamer 1 (Gezamelijk)" }
        { Address = "0/4/9"; Type = DatapointType.Temperature; Description = "Slaapkamer 2 (Merel)" }
        { Address = "0/4/10"; Type = DatapointType.Temperature; Description = "Slaapkamer 3 (Norah)" }
        { Address = "0/4/11"; Type = DatapointType.Temperature; Description = "Bureau" }
        { Address = "0/4/12"; Type = DatapointType.Temperature; Description = "Badkamer" }
        { Address = "0/4/13"; Type = DatapointType.Temperature; Description = "Weerstation" }
        { Address = "0/4/14"; Type = DatapointType.Temperature; Description = "Keuken" }

        { Address = "1/3/0"; Type = DatapointType.Temperature; Description = "Boiler - Buitentemperatuur" }
        { Address = "1/3/1"; Type = DatapointType.Temperature; Description = "Boiler - Temperatuur" }
        { Address = "1/3/5"; Type = DatapointType.Temperature; Description = "Zonnecollector - Zonnecollector temperatuur" }
        { Address = "1/3/6"; Type = DatapointType.Temperature; Description = "Zonnecollector - Zonnecylinder temperatuur" }
        { Address = "1/3/12"; Type = DatapointType.Temperature; Description = "Vloerverwarming - Temperatuur" }
        { Address = "1/3/17"; Type = DatapointType.Temperature; Description = "Warmwater - Temperatuur" }
    ]

let client = Client("54.93.94.171")
let db = "knx"

let writeData data precision =
    let timestamp = Some data.Timestamp
    let point = Point.create data.Measurement data.Tags data.Fields timestamp

    match point with
    | Choice1Of2 p ->
        async {
            let! result = client.Write(db, p, precision)

            match result with
            | Choice1Of2 x -> ()
            | Choice2Of2 error -> printf "%A" error
        } |> Async.RunSynchronously
    | Choice2Of2 _ -> ()

let logSwitch device (state: byte[]) =
    match parseSingleBit state.[0] with
    | Some toggle ->
        printfn "[ON/OFF] %s (%s)" device.Description toggle.Text

        let data = {
            Measurement = device.Address
            Tags = Map [ "name", device.Description; ]
            Fields = Map [ "toggle", FieldValue.Bool(toggle = SingleBitState.On) ]
            Timestamp = DateTime.UtcNow
        }

        writeData data Precision.Seconds
    | None -> ()

let logTemperature  device (state: byte[]) =
    let temperature = parseTwoByteFloat (state.[0], state.[1])
    printfn "[TEMP] %s (%f °C)" device.Description temperature

    let data = {
        Measurement = device.Address
        Tags = Map [ "name", device.Description; ]
        Fields = Map [ "temperature", FieldValue.Float(temperature) ]
        Timestamp = DateTime.UtcNow
    }

    writeData data Precision.Microseconds

let logDevice device state =
    match device.Type with
    | Switch -> logSwitch device state
    | Temperature -> logTemperature device state
    | _ -> ()

let log address state =
    let device =
        devices
        |> List.tryFind (fun x -> x.Address = address)

    match device with
    | Some x -> logDevice x state
    | None -> ()

let connect = fun reconnect ->
    let connection =
        new KnxConnectionRouting(ActionMessageCode = 0x29uy,
                                 KnxConnectedDelegate = KnxConnection.KnxConnected(fun _ -> printfn "Connected!"),
                                 KnxDisconnectedDelegate = KnxConnection.KnxDisconnected(reconnect),
                                 KnxEventDelegate = KnxConnection.KnxEvent(log),
                                 KnxStatusDelegate = KnxConnection.KnxStatus(log))

    connection.Connect()

let rec reconnect = fun _ ->
    printfn "Disconnected! Reconnecting..."
    Thread.Sleep(1000)
    connect reconnect  |> ignore

[<EntryPoint>]
let main argv =
    printfn "Connecting..."

    connect reconnect |> ignore

    Console.Read() |> ignore

    0
