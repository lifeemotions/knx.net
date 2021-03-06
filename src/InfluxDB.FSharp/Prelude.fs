namespace InfluxDB.FSharp

open System

[<AutoOpen>]
module internal Prelude =
    let Ok a: Choice<_, _> = Choice1Of2 a
    let Fail a: Choice<_, _> = Choice2Of2 a

    let inline ok _ = Ok ()

    let (|Ok|Fail|) =
        function
        | Choice1Of2 a -> Ok a
        | Choice2Of2 a -> Fail a

    let inline konst value ignored = value
    let inline swap fn x y = fn y x

    let inline isNull arg = Object.ReferenceEquals (null, arg)

    let invCulture = System.Globalization.CultureInfo.InvariantCulture

    [<Measure>] type msec


module internal Array =
    let emptyIfNull array =
        if isNull array then Array.empty else array


module internal String =
    let trim (str: string) =
        if isNull str then
            str
        else
            str.Trim()

    let inline replace (oldsubstr: string) (newsubstr: string) (str: string) =
        if isNull str then
            str
        else
            str.Replace(oldsubstr, newsubstr)

    let excludePrefix prefix str =
        if String.IsNullOrEmpty str then str
        elif str.StartsWith prefix then str.[prefix.Length..]
        else str

    let excludeSuffix suffix str =
        if String.IsNullOrEmpty str then str
        elif str.EndsWith suffix then str.[..str.Length - suffix.Length - 1]
        else str

    let ensureEndsWith suffix (str: string) =
        if str.EndsWith suffix then str
        else str + suffix


module internal Choice =
    let ofOption (value : 'T option) : Choice<'T, unit> =
        match value with
        | Some result -> Ok result
        | None -> Fail ()

    let map (mapping: 'a -> 'b) (value: Choice<'a,'err>) =
        match value with
        | Ok result -> Ok (mapping result)
        | Fail error -> Fail error

    let mapFail (mapping : 'err1 -> 'err2) (value: Choice<'t, 'err1>) =
        match value with
        | Ok result -> Ok result
        | Fail error -> Fail (mapping error)

    let attempt fn =
        try
            Ok (fn())
        with
        | exn -> Fail exn

    let isResult = function Ok _ -> true | Fail _ -> false
    let isFail = function Ok _ -> false | Fail _ -> true

    let get value = match value with Ok x -> x | Fail _ -> invalidArg "value" "Cannot get result because the Choice`2 instance is an error value."
    let getFail value = match value with Fail x -> x | Ok _ -> invalidArg "value" "Cannot get fail because the Choice`2 instance is an result value."

    let inline (<!>) value fn = mapFail fn value
    let inline (<!~>) value error = mapFail (konst error) value


module internal Seq =
    let trySingle (source: _ seq) =
        use e = source.GetEnumerator()
        if e.MoveNext() then
            let first = e.Current
            if e.MoveNext() = false then Some first
            else None
        else None

    let trySingleC (source: _ seq) =
        trySingle source |> Choice.ofOption

    let single (source: _ seq) =
        use e = source.GetEnumerator()
        if e.MoveNext() then
            let first = e.Current
            if e.MoveNext() = false then first
            else failwithf "Expected seq with signle element, but there is more than one"
        else failwithf "Expected seq with signle element, but there is no one"


module internal Map =
    open System.Collections.Generic

    let ofDict (source: Dictionary<_,_>) =
        let seq = source |> Seq.map (fun kvp -> kvp.Key, kvp.Value)
        Map seq

    let keys (map: Map<_,_>) =
        map |> Map.toSeq |> Seq.map fst

    let values (map: Map<_,_>) =
        map |> Map.toSeq |> Seq.map snd

    let toString (map: Map<_,_>) =
        let pairs =
            Map.toSeq map
            |> Seq.map ((<||) (sprintf "%O -> %O"))
            |> String.concat "; "
        sprintf "[%s]" pairs


module internal Option =
    let inline ofNull value =
        if isNull value then None else Some value


module internal Async =
    open System.Threading.Tasks

    let map (mapping : 'T -> 'U) (value : Async<'T>) : Async<'U> = async {
        let! x = value
        return mapping x
    }

    /// Await void Task, rethrow exception if it occurs
    let AwaitTaskVoid (task: Task) =
        task.ContinueWith<unit> (fun t -> if t.IsFaulted then raise t.Exception) |> Async.AwaitTask


module internal AsyncChoice =
    let inline map (fn: 'a -> 'b) (value: Async<Choice<'a, 'c>>) : Async<Choice<'b, 'c>> =
        value |> Async.map (Choice.map fn)

    let inline mapFail (fn: 'b -> 'c) (value: Async<Choice<'a, 'b>>) : Async<Choice<'a, 'c>> =
        value |> Async.map (Choice.mapFail fn)

    let inline (<!!>) value fn = mapFail fn value


module internal DateTime =
    let unixStartEpoch = DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)

    let private ticksPerMicrosecond = 10L

    /// number of microseconds since 1 January 1970 00:00:00 UTC (Unix Epoch)
    let toUnixMicroseconds (value: DateTime) =
        let ts = value.Subtract(unixStartEpoch)
        int64 ts.Ticks / ticksPerMicrosecond

    /// number of milliseconds since 1 January 1970 00:00:00 UTC (Unix Epoch)
    let toUnixMilliseconds (value: DateTime) =
        value.Subtract(unixStartEpoch).TotalMilliseconds |> int64

    /// number of seconds since 1 January 1970 00:00:00 UTC (Unix Epoch)
    let toUnixSeconds (value: DateTime) =
        value.Subtract(unixStartEpoch).TotalSeconds |> int64

    /// number of minutes since 1 January 1970 00:00:00 UTC (Unix Epoch)
    let toUnixMinutes (value: DateTime) =
        value.Subtract(unixStartEpoch).TotalMinutes |> int64

    /// number of hours since 1 January 1970 00:00:00 UTC (Unix Epoch)
    let toUnixHours (value: DateTime) =
        value.Subtract(unixStartEpoch).TotalHours |> int64
