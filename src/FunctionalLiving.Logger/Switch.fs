[<AutoOpen>]
module Switch

open System
open InfluxDB.FSharp
open FunctionalLiving.Parser.Domain
open FunctionalLiving.Parser.Category1_SingleBit

let logSwitch device (state: byte[]) =
    match parseSingleBit state.[0] with
    | Some switch ->
        sprintf "[ON/OFF] %s (%s)" device.Description switch.Text |> logToConsole

        let data = {
            Measurement = "switch"
            Tags = Map [ "address", device.Address; "name", device.Description; ]
            Fields = Map [ "switch", FieldValue.Bool(switch = SingleBitState.On) ]
            Timestamp = DateTime.UtcNow
        }

        writeData data Precision.Seconds
    | None -> ()