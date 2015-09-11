namespace FunctionalLiving.Parser.Tests

open Xunit
open Swensen.Unquote
open Grean.Exude

module Category13_4ByteSignedValue =

    open FunctionalLiving.Parser
    open System

    let ``13.010 active energy (Wh) test`` telegramBytes expected =
        verifyParser parseFourByteSigned telegramBytes expected

    [<FirstClassTests>]
    let ``13.010 active energy (Wh)`` () =
        let f = ``13.010 active energy (Wh) test``

        let data = [
            ((0x00uy, 0x00uy, 0x0Euy, 0x56uy), 3670)
            ((0x00uy, 0x00uy, 0x1Buy, 0x07uy), 6919)
        ]

        data
        |> List.map (fun (actual, expected) -> case f actual expected)
        |> List.toSeq
