namespace FunctionalLiving.Parser.Tests

open Xunit
open Swensen.Unquote
open Grean.Exude

module EIS9_Float =

    open FunctionalLiving.Parser
    open System

    let ``9.* 2-byte float value test`` telegramBytes expected =
        verifyParser parseFloat telegramBytes expected

    [<FirstClassTests>]
    let ``9.* 2-byte float value`` () =
        let f = ``9.* 2-byte float value test``

        let data = [
            ((0x12uy, 0x71uy), 25.0)
            ((0x0Cuy, 0x74uy), 22.8)
            ((0x15uy, 0xA0uy), 57.6)
            ((0x15uy, 0xE8uy), 60.48)
            ((0x4Euy, 0x6Buy), 8412.16)
            ((0x54uy, 0x4Buy), 11253.76)
            ((0x4Euy, 0xE6uy), 9041.92)
            ((0x34uy, 0x1Duy), 673.92)
            ((0x0Cuy, 0x74uy), 22.8)
            ((0x00uy, 0x14uy), 0.2)
            ((0x0Cuy, 0x74uy), 22.8)
            ((0x15uy, 0xEDuy), 60.68)
            ((0x16uy, 0x15uy), 62.28)
            ((0x16uy, 0x13uy), 62.2)
        ]

        data
        |> List.map (fun (actual, expected) -> case f actual expected)
        |> List.toSeq
