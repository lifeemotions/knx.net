namespace FunctionalLiving.Parser

[<AutoOpen>]
module EIS3_Time =

    open System
    open Domain

    let parseTime (timeBytes: TimeValue) =
        let (byte1, byte2, byte3) = timeBytes

        let day =
            match firstBits byte1 3 with
            | 1 -> Some Monday
            | 2 -> Some Tuesday
            | 3 -> Some Wednesday
            | 4 -> Some Thursday
            | 5 -> Some Friday
            | 6 -> Some Saturday
            | 7 -> Some Sunday
            | _ -> None

        let hours = lastBits byte1 5
        let minutes = lastBits byte2 6
        let seconds = lastBits byte3 6
        let time = TimeSpan(hours, minutes, seconds)

        (day, time)