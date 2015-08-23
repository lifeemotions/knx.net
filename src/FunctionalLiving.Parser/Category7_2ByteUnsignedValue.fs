namespace FunctionalLiving.Parser

[<AutoOpen>]
module Category7_2ByteUnsignedValue =

    open System
    open Domain

    let parseTwoByteUnsigned (resolution: float) (twoByteUnsignedValue: TwoByteUnsignedValue) =
        let (byte1, byte2) = twoByteUnsignedValue

        let value =
            [| byte1; byte2 |]
            |> bytesToBits
            |> bitsToUInt
            |> float

        Math.Round(resolution * value, 2)