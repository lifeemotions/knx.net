[<AutoOpen>]
module Domain

open System
open InfluxDB.FSharp

type DatapointType =
| Switch
| Toggle
//| Percentage
| Duration
| Current
| Temperature
| Light
| Speed
//| TimeOfDay
//| Date
//| EnergyWh
//| EnergyKWh

type Device = { Address: string; Type: DatapointType; Description: string; }

type PointData =
    { Measurement: Measurement
      Tags: Map<string,string>
      Fields: Map<string, FieldValue>
      Timestamp: DateTime }