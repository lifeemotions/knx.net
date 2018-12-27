[<AutoOpen>]
module Influx

open InfluxDB.FSharp

let client = Client("54.93.94.171")
let db = "knx"

let writeData data precision =
    let timestamp = Some data.Timestamp
    let point = Point.create data.Measurement data.Tags data.Fields timestamp

    match point with
    | Choice1Of2 p -> client.Write(db, p, precision) |> Async.RunSynchronously |> ignore
    | Choice2Of2 _ -> ()

let logToConsole text =
    let time =
        let zone = System.TimeZone.CurrentTimeZone.GetUtcOffset System.DateTime.Now
        let prefix = match (zone<System.TimeSpan.Zero) with | true -> "-" | _ -> "+"
        System.DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.ffff") + prefix + zone.ToString("hhss");

    printfn "%s || %s" time text