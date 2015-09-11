[<AutoOpen>]
module Duration

open System
open InfluxDB.FSharp
open FunctionalLiving.Parser.Category7_2ByteUnsignedValue

let logDuration device (state: byte[]) =
    let duration = parseTwoByteUnsigned 1.0 (state.[0], state.[1])
    sprintf "[DURATION] %s (%f h)" device.Description duration |> logToConsole

    let data = {
        Measurement = "duration_hours"
        Tags = Map [ "address",  device.Address; "name", device.Description; ]
        Fields = Map [ "duration", FieldValue.Float(duration) ]
        Timestamp = DateTime.UtcNow
    }

    writeData data Precision.Microseconds