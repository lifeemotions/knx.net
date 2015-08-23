namespace FunctionalLiving.Parser

[<AutoOpen>]
module EIS9_Float =

    open System
    open Domain

    let parseFloat(floatBytes: FloatValue) =
        let (byte1, byte2) = floatBytes

        let bits = bytesToBits [| byte1; byte2 |]

        let resolution = 0.01

        let sign =
            match bits.[0] with
            | One -> -1
            | Zero -> 1

        let exponent =
            [| bits.[1]; bits.[2]; bits.[3]; bits.[4] |]
            |> bitsToUInt
            |> float

        let power =
            2.0 ** exponent

        let mantissa =
            bits
            |> Array.skip 5
            |> bitsToUInt
            |> float

        Math.Round(mantissa * power * resolution, 2)