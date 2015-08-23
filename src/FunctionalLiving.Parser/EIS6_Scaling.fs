namespace FunctionalLiving.Parser

[<AutoOpen>]
module EIS6_Scaling =

    open System
    open Domain

    let parseScaling min max (scalingBytes: ScalingValue) =
        match scalingBytes with
        | 0x00uy -> min
        | 0xFFuy -> max
        | b -> Math.Round(max / 255.0 * float(b), 2)