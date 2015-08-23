namespace FunctionalLiving.Parser

[<AutoOpen>]
module EIS1_Switching =

    open Domain

    let parseSwitching (switchingBytes: SwitchingValue) =
        match switchingBytes with
        | 0x00uy -> Some Off
        | 0x01uy -> Some On
        | _ -> None
