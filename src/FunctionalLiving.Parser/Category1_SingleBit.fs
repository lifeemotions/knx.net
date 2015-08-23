namespace FunctionalLiving.Parser

[<AutoOpen>]
module Category1_SingleBit =

    open Domain

    let parseSingleBit (singleBitValue: SingleBitValue) =
        match singleBitValue with
        | 0x00uy -> Some Off
        | 0x01uy -> Some On
        | _ -> None
