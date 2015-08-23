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
            | One -> -1.0
            | Zero -> 1.0

        let exponent =
            [| bits.[1]; bits.[2]; bits.[3]; bits.[4] |]
            |> bitsToUInt
            |> float

        let power =
            2.0 ** exponent

        let invertBit bit =
            match bit with
            | One -> Zero
            | Zero -> One

        let mantissa =
            bits
            |> Array.skip 5
            |> Array.map (fun bit -> if sign = -1.0 then invertBit bit else bit)
            |> bitsToUInt
            |> (fun mantissa -> if sign = -1.0 then mantissa + 1 else mantissa)
            |> float

        Math.Round(sign * mantissa * power * resolution, 2)