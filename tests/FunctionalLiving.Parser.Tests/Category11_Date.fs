namespace FunctionalLiving.Parser.Tests

open Xunit
open Swensen.Unquote
open Grean.Exude

module Category11_Date =

    open FunctionalLiving.Parser
    open System

    let ``11.001 date test`` telegramBytes expected =
        verifyParser parseDate telegramBytes expected

    [<FirstClassTests>]
    let ``11.001 date`` () =
        let f = ``11.001 date test``

        let data = [
            ((0x17uy, 0x08uy, 0x0Fuy), new DateTime(2015, 8, 23))
        ]

        data
        |> List.map (fun (actual, expected) -> case f actual expected)
        |> List.toSeq
