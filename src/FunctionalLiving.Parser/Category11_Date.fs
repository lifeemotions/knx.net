namespace FunctionalLiving.Parser

[<AutoOpen>]
module Category11_Date =

    open System
    open Domain

    let parseDate (dateValue: DateValue) =
        let (byte1, byte2, byte3) = dateValue

        let day = lastBits byte1 5
        let month = lastBits byte2 4

        let year =
            match int(byte3) with
            | previousCentury when byte3 >= 90uy -> 1900 + previousCentury
            | nextCentury -> 2000 + nextCentury

        DateTime(year, month, day)