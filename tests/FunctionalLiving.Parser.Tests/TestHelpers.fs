namespace FunctionalLiving.Parser.Tests

[<AutoOpen()>]
module TestHelpers =

    open System
    open Grean.Exude

    let case f input expected =
        let action = Action(fun _ -> f input expected)
        TestCase(action)

