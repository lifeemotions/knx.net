namespace FunctionalLiving.Parser

open System.Runtime.CompilerServices

[<Extension>]
module Methods =

    [<Extension>]
    let Exists (opt: 'a option) =
        match opt with
        | Some _ -> true
        | None -> false
