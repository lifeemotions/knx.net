namespace FunctionalLiving.Parser

[<AutoOpen>]
module Category13_4ByteSignedValue =

    open System
    open Domain

    let parseFourByteSigned (fourByteSignedValue: FourByteSignedValue) =
        let (byte1, byte2, byte3, byte4) = fourByteSignedValue
        let data = [| byte4; byte3; byte2; byte1 |]

        BitConverter.ToInt32(data, 0)