namespace FunctionalLiving.Parser.Tests

open Xunit
open Swensen.Unquote
open Grean.Exude

module Category7_2ByteUnsignedValue =

    open FunctionalLiving.Parser
    open System

    let ``7.* 2-byte unsigned value test`` telegramBytes expected =
        let parseTwoByteUnsigned = parseTwoByteUnsigned 1.0
        verifyParser parseTwoByteUnsigned telegramBytes expected

    let ``7.012 current (mA) test`` telegramBytes expected =
        let parseTwoByteUnsigned = parseTwoByteUnsigned 1.0
        verifyParser parseTwoByteUnsigned telegramBytes expected

    [<FirstClassTests>]
    let ``7.* 2-byte unsigned value`` () =
        let f1 = ``7.* 2-byte unsigned value test``
        let f2 = ``7.012 current (mA) test``

        let data = [
            (f2, (0x4Fuy, 0x67uy), 20327.0)
            (f2, (0x4Fuy, 0xD9uy), 20441.0)
        ]

        data
        |> List.map (fun (f, actual, expected) -> case f actual expected)
        |> List.toSeq
