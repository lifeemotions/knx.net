namespace InfluxDB.FSharp

open System

module Point =

    type T =
        { Measurement: Measurement
          Tags: Map<string,string>
          Fields: Map<string, FieldValue>
          Timestamp: DateTime option } // todo replace DateTime with DateTimeOffset
        with override x.ToString () =
                sprintf "{ Measurement: %s; Tags: %s; Fields: %s; Timestamp: %s }"
                    x.Measurement
                    (Map.toString x.Tags)
                    (Map.toString x.Fields)
                    (match x.Timestamp with | Some x -> x.ToString("yyyy-MM-dd HH:mm:ss.fff") | None -> "<none>")

    [<Literal>]
    let private MaxStringLen = 65536 // 64KB

    type ValidateError = string

    let puree x = Ok x

    let apply f x =
        match f, x with
        | Ok f, Ok x       -> Ok (f x)
        | Fail e1, Fail e2 -> Fail (e1 @ e2)
        | Fail e,  _       -> Fail e
        | _, Fail e        -> Fail e

    let (<*>) = apply
    let inline (<!>) f x = puree f <*> x

    let (|IsNullOrEmptyOrWhitespaces|) = String.IsNullOrWhiteSpace

    let escapeString =
        String.trim
        >> String.replace "," @"\,"
        >> String.replace " " @"\ "

    let escapeFieldStringValue =
        String.trim
        >> String.replace "\"" "\\\""

    let validateString name : string -> Choice<string,ValidateError> =
        function
        | null -> Fail (sprintf "%s cant be null" name)
        | "" -> Fail (sprintf "%s cant be empty string or contain only whitespace characters" name)
        | x when x.Length > MaxStringLen -> Fail (sprintf "%s length should be <= 64KB" name)
        | x -> Ok x

    let validateMeasurement =
        escapeString
        >> validateString "measurement"
        >> Choice.mapFail (fun e -> [e])

    let validateTags (tags: Map<string,string>) : Choice<Map<string,string>, ValidateError list> =
        let folder (tags, errors) key value =
            let key = escapeString key
            let value = escapeString value
            match validateString "tag key" key, validateString "tag value" value with
            | Ok key, Ok value -> Map.add key value tags, errors
            | Fail e1, Fail e2 -> tags, e1 :: e2 :: errors
            | Fail e, _ -> tags, e :: errors
            | _, Fail e -> tags, e :: errors

        let tags, errors = Map.fold folder (Map.empty, []) tags
        match errors with
        | [] -> Ok tags
        | errors -> Fail errors

    let validateFieldValue name =
        function
        | String v -> validateString name v |> Choice.map FieldValue.String
        | x -> Ok x

    let escapeStringValueField = String.trim >> String.replace "\"" "\\\""

    let validateFields (fields: Map<string,FieldValue>) : Choice<Map<string,FieldValue>, ValidateError list> =
        if fields.Count = 0 then
            Fail ["at least one field is required"]
        else
            let folder (fields, errors) key value =
                let key = escapeString key
                let value = match value with
                            | String value -> FieldValue.String (escapeStringValueField value)
                            | x -> x
                match validateString "field key" key, validateFieldValue "field value" value with
                | Ok key, Ok value -> Map.add key value fields, errors
                | Fail e1, Fail e2 -> fields, e1 :: e2 :: errors
                | Fail e, _ -> fields, e :: errors
                | _, Fail e -> fields, e :: errors

            let fields, errors = Map.fold folder (Map.empty, []) fields
            match errors with
            | [] -> Ok fields
            | errors -> Fail errors

    let validateTimestamp timestamp : Choice<DateTime option,ValidateError list> =
        // todo remove this restriction after https://github.com/influxdb/influxdb/issues/3367 be fixed
        match timestamp with
        | Some timestamp when timestamp < DateTime.unixStartEpoch ->
            Fail ["timestamp cant be before UNIX start epoch (1970-01-01 00:00:00 UTC)"]
        | _ -> Ok timestamp

    let create (measurement: Measurement) (tags: Map<string,string>) (fields: Map<string,FieldValue>) (timestamp: DateTime option) : Choice<T, string list> =
        (fun measurement tags fields timestamp ->
            { Measurement = measurement
              Tags = tags
              Fields = fields
              Timestamp = timestamp })
        <!> validateMeasurement measurement
        <*> validateTags tags
        <*> validateFields fields
        <*> validateTimestamp timestamp

    let toLine precision point =
        let tags =
            point.Tags
            |> Map.toSeq
            |> Seq.map ((<||) (sprintf "%s=%s"))
            |> String.concat ","
            |> function "" -> "" | s -> sprintf ",%s" s
        let key = sprintf "%s%s" point.Measurement tags

        let fields =
            point.Fields
            |> Map.toSeq
            |> Seq.map (fun (k, v) ->
                let value =
                    match v with
                    | Int v -> sprintf "%di" v
                    | Float v -> v.ToString("0.0###############", invCulture)
                    | String v -> sprintf "\"%s\"" v
                    | Bool true -> "t"
                    | Bool false -> "f"
                sprintf "%s=%s" k value)
            |> String.concat ","

        let timestamp =
            match point.Timestamp with
            | Some timestamp ->
                // todo reorder for perfmance?
                match precision with
                | Precision.Microseconds -> DateTime.toUnixMicroseconds timestamp
                | Precision.Milliseconds -> DateTime.toUnixMilliseconds timestamp
                | Precision.Seconds -> DateTime.toUnixSeconds timestamp
                | Precision.Minutes -> DateTime.toUnixMinutes timestamp
                | Precision.Hours -> DateTime.toUnixHours timestamp
                | x -> raise (NotImplementedException(sprintf "precision %A" x))
                |> sprintf " %d"
            | None -> ""

        let line = sprintf "%s %s%s" key fields timestamp
        line
