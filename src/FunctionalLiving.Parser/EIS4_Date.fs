namespace FunctionalLiving.Parser

[<AutoOpen>]
module EIS4_Date =

    open System
    open Domain

    let parseDate(date: DateValue) =
        let (byte1, byte2, byte3) = date

        let day = lastBits byte1 5
        let month = lastBits byte2 4

        let year =
            match int(byte3) with
            | previousCentury when byte3 >= 90uy -> 1900 + previousCentury
            | nextCentury -> 2000 + nextCentury

        DateTime(year, month, day)