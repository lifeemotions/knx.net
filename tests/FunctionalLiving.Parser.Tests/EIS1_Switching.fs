namespace FunctionalLiving.Parser.Tests

open Xunit
open Swensen.Unquote
open Grean.Exude

module EIS1_Switching =

    open FunctionalLiving.Parser
    open System

    let ``1.* switching test`` telegramBytes expected =
        let datapoint =
            telegramBytes
            |> parseSwitching

        test <@ datapoint = expected @>

    [<FirstClassTests>]
    let ``1.* switching`` () =
        let f = ``1.* switching test``

        let data = [
            (0x00uy, Some SwitchingValue.Off)
            (0x01uy, Some SwitchingValue.On)
        ]

        data
        |> List.map (fun (actual, expected) -> case f actual expected)
        |> List.toSeq
