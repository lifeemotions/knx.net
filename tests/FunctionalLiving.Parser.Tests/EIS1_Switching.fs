namespace FunctionalLiving.Parser.Tests

open Xunit
open Swensen.Unquote
open Grean.Exude

module EIS1_Switching =

    open FunctionalLiving.Parser
    open System

    let ``1.* switching test`` telegramBytes expected =
        verifyParser parseSwitching telegramBytes expected

    [<FirstClassTests>]
    let ``1.* switching`` () =
        let f = ``1.* switching test``

        let data = [
            (0x00uy, Some Switching.Off)
            (0x01uy, Some Switching.On)
        ]

        data
        |> List.map (fun (actual, expected) -> case f actual expected)
        |> List.toSeq
