namespace FunctionalLiving.Parser.Tests

open Xunit
open Swensen.Unquote
open Grean.Exude

module Category1_SingleBit =

    open FunctionalLiving.Parser
    open System

    let ``1.* switching test`` telegramBytes expected =
        verifyParser parseSingleBit telegramBytes expected

    [<FirstClassTests>]
    let ``1.* switching`` () =
        let f = ``1.* switching test``

        let data = [
            (0x00uy, Some SingleBitState.Off)
            (0x01uy, Some SingleBitState.On)
        ]

        data
        |> List.map (fun (actual, expected) -> case f actual expected)
        |> List.toSeq
