[<AutoOpen>]
module Toggle

open System
open InfluxDB.FSharp
open FunctionalLiving.Parser.Domain
open FunctionalLiving.Parser.Category1_SingleBit

let logToggle device (state: byte[]) =
    match parseSingleBit state.[0] with
    | Some toggle ->
        sprintf "[TRUE/FALSE] %s (%s)" device.Description toggle.Text |> logToConsole

        let data = {
            Measurement = "toggle"
            Tags = Map [ "address", device.Address; "name", device.Description; ]
            Fields = Map [ "toggle", FieldValue.Bool(toggle = SingleBitState.On) ]
            Timestamp = DateTime.UtcNow
        }

        writeData data Precision.Seconds
    | None -> ()