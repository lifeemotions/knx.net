namespace FunctionalLiving.Parser.Tests

open Xunit
open Swensen.Unquote
open Grean.Exude

module Category5_Scaling =

    open FunctionalLiving.Parser
    open System

    let ``5.* 8-bit unsigned value test`` telegramBytes expected =
        let parseScaling = parseScaling 0.0 255.0
        verifyParser parseScaling telegramBytes expected

    let ``5.001 percentage (0..100%) test`` telegramBytes expected =
        let parseScaling = parseScaling 0.0 100.0
        verifyParser parseScaling telegramBytes expected

    [<FirstClassTests>]
    let ``5.* 8-bit unsigned value`` () =
        let f1 = ``5.* 8-bit unsigned value test``
        let f2 = ``5.001 percentage (0..100%) test``

        let data = [
            (f1, (0xFFuy), 255.0)
            (f2, (0xFFuy), 100.0)
            (f1, (0x00uy), 0.0)
            (f2, (0x00uy), 0.0)
            (f2, (0x31uy), 19.22)
        ]

        data
        |> List.map (fun (f, actual, expected) -> case f actual expected)
        |> List.toSeq
