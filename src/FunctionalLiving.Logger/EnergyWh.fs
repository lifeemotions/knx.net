[<AutoOpen>]
module EnergyWh

open System
open InfluxDB.FSharp
open FunctionalLiving.Parser.Category13_4ByteSignedValue

let logEnergyWh device (state: byte[]) =
    let wattHour = parseFourByteSigned (state.[0], state.[1], state.[2], state.[3])
    sprintf "[ENERGY] %s (%d Wh)" device.Description wattHour |> logToConsole

    let data = {
        Measurement = "energy_watt_hour"
        Tags = Map [ "address",  device.Address; "name", device.Description; ]
        Fields = Map [ "energy_watt_hour", FieldValue.Int(wattHour) ]
        Timestamp = DateTime.UtcNow
    }

    writeData data Precision.Microseconds