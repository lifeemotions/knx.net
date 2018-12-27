[<AutoOpen>]
module Light

open System
open InfluxDB.FSharp
open FunctionalLiving.Parser.Category9_2ByteFloatValue

let logLight device (state: byte[]) =
    let light = parseTwoByteFloat (state.[0], state.[1])
    sprintf "[LUX] %s (%f Lux)" device.Description light |> logToConsole

    let data = {
        Measurement = "light_lux"
        Tags = Map [ "address",  device.Address; "name", device.Description; ]
        Fields = Map [ "light", FieldValue.Float(light) ]
        Timestamp = DateTime.UtcNow
    }

    writeData data Precision.Microseconds