[<AutoOpen>]
module Current

open System
open InfluxDB.FSharp
open FunctionalLiving.Parser.Category7_2ByteUnsignedValue

let logCurrent device (state: byte[]) =
    let current = parseTwoByteUnsigned 1.0 (state.[0], state.[1])
    sprintf "[ENERGY] %s (%f mA)" device.Description current |> logToConsole

    let data = {
        Measurement = "energy_ma"
        Tags = Map [ "address",  device.Address; "name", device.Description; ]
        Fields = Map [ "energy", FieldValue.Float(current) ]
        Timestamp = DateTime.UtcNow
    }

    writeData data Precision.Microseconds