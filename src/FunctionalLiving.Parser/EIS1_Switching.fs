namespace FunctionalLiving.Parser

[<AutoOpen>]
module EIS1_Switching =

    open Domain

    let parseSwitching (switching: Switching) =
        match switching with
        | 0x00uy -> Some Off
        | 0x01uy -> Some On
        | _ -> None
