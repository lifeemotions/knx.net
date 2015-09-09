namespace InfluxDB.FSharp

open System
open System.Net
open InfluxDB.FSharp.Choice
open InfluxDB.FSharp.AsyncChoice
open InfluxDB.FSharp.Http

module internal Contracts =
    open System.Collections.Generic
    open System.Runtime.Serialization
    open System.Runtime.Serialization.Json

    [<DataContract>]
    type Series =
        {
            [<field: DataMember(Name="name")>]
            Name: string

            [<field: DataMember(Name="tags")>]
            Tags: Dictionary<string,string>

            [<field: DataMember(Name="columns")>]
            Columns: string[]

            [<field: DataMember(Name="values")>]
            Values: obj[][]
        }

    [<DataContract>]
    type Results =
        {
            [<field: DataMember(Name="series")>]
            Series: Series[]

            [<field: DataMember(Name="error")>]
            Error: string
        }

    [<DataContract>]
    type Response =
        {
            [<field: DataMember(Name="results")>]
            Results: Results[]

            [<field: DataMember(Name="error")>]
            Error: string
        }

    let private settings = DataContractJsonSerializerSettings()
    settings.UseSimpleDictionaryFormat <- true

    let deserialize<'a> (json: string) =
        Choice.attempt <| fun () ->
            use ms = new IO.MemoryStream(Text.Encoding.Unicode.GetBytes json)
            let serializer = DataContractJsonSerializer(typedefof<'a>, settings)
            serializer.ReadObject(ms) :?> 'a


// todo xml docs on public members
// todo validate host
// todo validate proxy
type Client (host: string, ?port: uint16, ?credentials: Credentials, ?proxy: InfluxDB.FSharp.Proxy) =
    let port = defaultArg port 8086us
    let baseUri = Uri(sprintf "http://%s:%d" host port)
    let uri (path: string) = Uri(baseUri, path)

    let createRequest =
        match proxy with
        | Some proxy -> fun ``method`` url -> Http.createRequest ``method`` url |> Http.withProxy proxy
        | None -> Http.createRequest

    let buildError (response: Response) =
        let msg = match response.Body with
                  | Some body ->
                      match Contracts.deserialize<Contracts.Response> body with
                      | Ok resp -> Some resp.Error
                      | Fail _ -> Some body
                  | None -> None
        Fail (BadStatusCode (response.StatusCode, msg))

    let query db qstr mapOk = asyncChoice {
        let withDb =
            match db with
            | Some db -> withQueryStringItem "db" db
            | None -> id

        let! response =
            createRequest Get (uri "query")
            |> withDb
            |> withQueryStringItem "q" qstr
            |> getResponse
            <!!> HttpError

        return!
            match response.StatusCode with
            | HttpStatusCode.OK -> mapOk response
            | _ -> buildError response <!> HttpError
    }

    let write query body = asyncChoice {
        let! response =
            createRequest (Post body) (uri "write")
            |> withQueryStringItems query
            |> getResponse
            <!!> HttpError

        return!
            match response.StatusCode with
            | HttpStatusCode.NoContent -> Ok ()
            | _ -> buildError response <!> HttpError
    }

    let ping () = asyncChoice {
        let sw = Diagnostics.Stopwatch()
        do sw.Start()
        let! response =
            createRequest Get (uri "ping")
            |> getResponse
            <!!> HttpError
        do sw.Stop()

        let! version =
            response.Headers
            |> Map.tryFind "X-Influxdb-Version"
            |> function
            | Some version -> Ok version
            | None -> Fail (ResponseParseError "No version header in response")

        return sw.Elapsed, version
    }

    let showDbs () =
        query None "SHOW DATABASES" <| fun resp ->
            choice {
                let! json = resp.Body |> Choice.ofOption <!~> ResponseParseError "Response doesnt contain body"
                let! response = Contracts.deserialize<Contracts.Response> json <!~> ResponseParseError "Cant parse response contract"
                return!
                    response.Results
                    |> Seq.trySingle
                    |> Option.bind (fun r -> Seq.trySingle r.Series)
                    |> Choice.ofOption
                    <!~> ResponseParseError "TODO"
                    |> function
                       | Ok series ->
                            match Option.ofNull series.Values with
                            | Some values ->
                                Ok (values |> Seq.collect id |> Seq.toList)
                            | None -> Ok []
                        | Fail e -> Fail e
            }

    // todo: refact "<!~> ResponseParseError" somehow
    let checkForError resp =
        choice {
            let! json = resp.Body |> Choice.ofOption <!~> ResponseParseError "TODO"
            let! resp = Contracts.deserialize<Contracts.Response> json <!~> ResponseParseError "TODO"
            let! result = resp.Results |> Seq.trySingleC <!~> ResponseParseError "TODO"
            return!
                match result.Error with
                | null -> Ok ()
                | errmsg -> Fail (Error.ServerError errmsg)
        }

    let createDb name =
        query None (sprintf "CREATE DATABASE %s" name) checkForError

    let dropDb name =
        query None (sprintf "DROP DATABASE %s" name) checkForError

    let toStr =
        // todo reorder for perfmance?
        function
        | Precision.Microseconds -> "u"
        | Precision.Milliseconds -> "ms"
        | Precision.Seconds -> "s"
        | Precision.Minutes -> "m"
        | Precision.Hours -> "h"
        | x -> raise (NotImplementedException(sprintf "precision %A" x))

    // todo validate db name
    // todo sort tags by keys for perfomance (see docs)
    // todo rewrite with stringBuffer{} and run under profiler
    let doWrite db (point: Point.T) precision =
        let line = Point.toLine precision point
        let precision = toStr precision
        let query = [ "db", db; "precision", precision ]
        write query line

    let doWriteMany db (points: Point.T[]) precision =
        let lines =
            points
            |> Array.map (Point.toLine precision)
            |> String.concat "\n"
        let precision = toStr precision
        let query = [ "db", db; "precision", precision ]
        write query lines

    let doQuery db querystr =
        query (Some db) querystr <| fun (resp: Response) ->
            choice {
                let! body = Choice.ofOption resp.Body <!~> ResponseParseError "TODO"
                let! qresp = Contracts.deserialize<Contracts.Response> body <!~> ResponseParseError "TODO"
                let response =
                    match Option.ofNull qresp.Error with
                    | Some errormsg -> Fail (ServerError errormsg)
                    | None ->
                        qresp.Results
                        |> Array.map (fun res ->
                            match Option.ofNull res.Error with
                            | Some errormsg -> Fail errormsg
                            | None ->
                                res.Series
                                |> Array.emptyIfNull
                                |> Array.map (fun ser ->
                                    { Name = ser.Name
                                      Tags = match Option.ofNull ser.Tags with
                                             | Some tags -> Map.ofDict tags
                                             | None -> Map.empty
                                      Columns = ser.Columns |> Array.toList
                                      Values = ser.Values |> Array.map (Array.map (function
                                                                                   | :? int32 as v -> FieldValue.Int (int64 v)
                                                                                   | :? int64 as v -> FieldValue.Int v
                                                                                   | :? float as v -> FieldValue.Float v
                                                                                   | :? decimal as v -> FieldValue.Float (float v)
                                                                                   | :? string as v -> FieldValue.String v
                                                                                   | :? bool as v -> FieldValue.Bool v
                                                                                   | x -> failwithf "mapping for %O (%s) not implemented" x (x.GetType().FullName)))
                                     })
                                |> Array.toList
                                |> Ok)
                        |> Array.toList
                        |> Ok
                return! response
            }

    member __.Ping() = ping()

    member __.ShowDatabases() = showDbs()
    member __.CreateDatabase(name: string) = createDb name
    member __.DropDatabase(name: string) = dropDb name

    // todo write warning in xml doc about better usage of WriteMany
    member __.Write(db: Database, point: Point.T, precision: Precision) = doWrite db point precision
    member __.WriteMany(db: Database, points: Point.T[], precision: Precision) = doWriteMany db points precision

    member __.Query(db: Database, query: string) : Async<Choice<QueryResult list,Error>> = doQuery db query
