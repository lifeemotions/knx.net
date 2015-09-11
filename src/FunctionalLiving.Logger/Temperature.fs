[<AutoOpen>]
module Temperature

open System
open InfluxDB.FSharp
open FunctionalLiving.Parser.Category9_2ByteFloatValue

let logTemperature device (state: byte[]) =
    let temperature = parseTwoByteFloat (state.[0], state.[1])
    sprintf "[TEMP] %s (%f °C)" device.Description temperature |> logToConsole

    let data = {
        Measurement = "temperature_celsius"
        Tags = Map [ "address",  device.Address; "name", device.Description; ]
        Fields = Map [ "temperature", FieldValue.Float(temperature) ]
        Timestamp = DateTime.UtcNow
    }

    writeData data Precision.Microseconds