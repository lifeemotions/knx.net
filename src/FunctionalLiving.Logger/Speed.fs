[<AutoOpen>]
module Speed

open System
open InfluxDB.FSharp
open FunctionalLiving.Parser.Category9_2ByteFloatValue

let logSpeed device (state: byte[]) =
    let speed = parseTwoByteFloat (state.[0], state.[1])
    sprintf "[SPEED] %s (%f m/s)" device.Description speed |> logToConsole

    let data = {
        Measurement = "speed_meter_sec"
        Tags = Map [ "address",  device.Address; "name", device.Description; ]
        Fields = Map [ "speed", FieldValue.Float(speed) ]
        Timestamp = DateTime.UtcNow
    }

    writeData data Precision.Microseconds