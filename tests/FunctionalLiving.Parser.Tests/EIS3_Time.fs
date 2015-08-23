namespace FunctionalLiving.Parser.Tests

open Xunit
open Swensen.Unquote
open Grean.Exude

module EIS3_Time =

    open FunctionalLiving.Parser
    open System

    let ``10.001 time of day test`` telegramBytes expected =
        verifyParser parseTime telegramBytes expected

    [<FirstClassTests>]
    let ``10.001 time of day`` () =
        let f = ``10.001 time of day test``

        let data = [
            ((0xEFuy, 0x28uy, 0x02uy), (Some Sunday, TimeSpan(15, 40, 2)))
        ]

        data
        |> List.map (fun (actual, expected) -> case f actual expected)
        |> List.toSeq
