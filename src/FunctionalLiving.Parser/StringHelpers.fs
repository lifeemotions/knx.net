namespace FunctionalLiving.Parser

[<AutoOpen>]
module StringHelpers =

    open Microsoft.FSharp.Reflection

    let toString (x: 'a) =
        match FSharpValue.GetUnionFields(x, typeof<'a>) with
        | case, _ -> case.Name