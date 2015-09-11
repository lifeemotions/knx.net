[<AutoOpen>]
module EnergyKWh

open System
open InfluxDB.FSharp
open FunctionalLiving.Parser.Category13_4ByteSignedValue

let logEnergyKWh device (state: byte[]) =
    let kiloWattHour = parseFourByteSigned (state.[0], state.[1], state.[2], state.[3])
    sprintf "[ENERGY] %s (%d kWh)" device.Description kiloWattHour |> logToConsole

    let data = {
        Measurement = "energy_kilowatt_hour"
        Tags = Map [ "address",  device.Address; "name", device.Description; ]
        Fields = Map [ "energy_kilowatt_hour", FieldValue.Int(kiloWattHour) ]
        Timestamp = DateTime.UtcNow
    }

    writeData data Precision.Microseconds