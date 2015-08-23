namespace FunctionalLiving.Parser

[<AutoOpen>]
module Category5_Scaling =

    open System
    open Domain

    let parseScaling min max (scalingValue: ScalingValue) =
        match scalingValue with
        | 0x00uy -> min
        | 0xFFuy -> max
        | b -> Math.Round(max / 255.0 * float(b), 2)