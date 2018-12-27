namespace InfluxDB.FSharp

type internal ChoiceBuilder () =
    static let zero : Choice<unit,'Error> = Choice1Of2 ()

    member inline __.Return value : Choice<'T, 'Error> = Ok value
    member inline __.ReturnFrom (m : Choice<'T, 'Error>) = m
    member inline __.Zero () : Choice<unit, 'Error> = zero
    member inline __.Delay (generator : unit -> Choice<'T, 'Error>) : Choice<'T, 'Error> =
        generator ()

    member inline __.Combine (r1, r2) : Choice<'T, 'Error> =
        match r1 with
        | Fail error -> Fail error
        | Ok () -> r2

    member inline __.Bind (value, binder : 'T -> Choice<'U, 'Error>) : Choice<'U, 'Error> =
        match value with
        | Fail error -> Fail error
        | Ok x -> binder x

    member inline __.TryWith (body : 'T -> Choice<'U, 'Error>, handler) =
        fun value ->
            try body value
            with ex -> handler ex

    member inline __.TryFinally (body : 'T -> Choice<'U, 'Error>, handler) =
        fun value ->
            try body value
            finally handler ()

    member this.Using (resource : ('T :> System.IDisposable), body : _ -> Choice<_,_>) : Choice<'U, 'Error> =
        try body resource
        finally
            if not <| isNull (box resource) then
                resource.Dispose ()

    member this.While (guard, body : Choice<unit, 'Error>) : Choice<_, 'Error> =
        if guard () then
            this.Bind (body, (fun () -> this.While (guard, body)))
        else
            this.Zero ()

    member inline this.For (sequence : seq<_>, body : 'T -> Choice<unit, 'Error>) =
        this.Using (sequence.GetEnumerator (), fun enum ->
            this.While (
                enum.MoveNext,
                this.Delay (fun () -> body enum.Current)))

[<AutoOpen>]
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module internal ChoiceBuilder =
    let choice = ChoiceBuilder()
