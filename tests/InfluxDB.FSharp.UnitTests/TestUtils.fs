[<AutoOpen>]
module InfluxDB.FSharp.UnitTests.TestUtils

open System
open NUnit.Framework
open InfluxDB.FSharp

/// Assert values equal
let (=?) (actual: 'a) (expected: 'a) =
    match box actual with
    | :? String
    | :? DateTime -> Assert.That(actual, Is.EqualTo(expected))
    | _ -> Assert.That(actual, Is.EqualTo(expected), sprintf "Expected: %A\n  But was:  %+A" expected actual)

/// Assert enumerables equivalent
let (=~?) (actual: #seq<_>) (expected: #seq<_>) =
    Assert.That(actual, Is.EquivalentTo(expected), sprintf "Expected: %A\n    Actual: %A" expected actual)

let notFail =
    function
    | Ok x -> x
    | Fail err -> failwithf "unexpectedly return Fail %+A" err

let shouldNotFail choice = choice |> notFail |> ignore

let shouldFailWith expectedErrors =
    function
    | Fail errs -> errs =? expectedErrors
    | Ok x -> failwithf "unexpectedly return Ok %O" x

[<AutoOpen>]
module StringBuffer =
    open System.Text

    [<NoComparison; NoEquality>]
    type StringBuffer = StringBuffer of (StringBuilder -> unit)

    let build (StringBuffer f) =
        let b = StringBuilder()
        do f b
        b.ToString ()

    let private zero = StringBuffer (fun _ -> ())
    let inline private (!) x = match x with StringBuffer f -> f

    type StringBufferM () =
        static let zero = StringBuffer (fun _ -> ())
        member inline __.Yield (txt: string) = StringBuffer (fun b -> b.Append txt |> ignore)
        member inline __.Yield (c: char) = StringBuffer (fun b -> b.Append c |> ignore)
        member inline __.YieldFrom (f: StringBuffer) = f
        member __.Combine (f, g) = StringBuffer (fun b -> !f b; !g b)
        member __.Delay f = StringBuffer (fun b -> !(f()) b)
        member __.Zero () = zero

        member __.For (xs: 'a seq, f: 'a -> StringBuffer) =
            StringBuffer <| fun b ->
                let e = xs.GetEnumerator ()
                while e.MoveNext() do
                    !(f e.Current) b

        member __.While (p: unit -> bool, f: StringBuffer) =
            StringBuffer (fun b -> while p () do !f b)

    let stringBuffer = new StringBufferM ()


type PointData =
    { Measurement: Measurement
      Tags: Map<string,string>
      Fields: Map<string, FieldValue>
      Timestamp: DateTime option }

let tryCreateFrom (data: PointData) =
    Point.create data.Measurement data.Tags data.Fields data.Timestamp

let createFrom = tryCreateFrom >> notFail
let createFromMany = Array.map createFrom
