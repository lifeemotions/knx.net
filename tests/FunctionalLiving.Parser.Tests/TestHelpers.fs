namespace FunctionalLiving.Parser.Tests

[<AutoOpen()>]
module TestHelpers =

    open System
    open Grean.Exude
    open Swensen.Unquote

    let verify = test

    let case f input expected =
        let action = Action(fun _ -> f input expected)
        TestCase(action)

    let verifyParser parser telegramBytes expected =
        verify
            <@
                let datapoint =
                    telegramBytes
                    |> parser

                datapoint = expected
            @>
