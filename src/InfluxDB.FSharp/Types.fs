namespace InfluxDB.FSharp

open System.Net

type Database = string
type Measurement = string
type InfluxDbVersion = string

type FieldValue =
    | Int of int64
    | Float of double
    | String of string
    | Bool of bool
    with override x.ToString() =
            match x with
            | Int x -> string x
            | Float x -> string x
            | String x -> x
            | Bool x -> string x

type Precision =
    | Microseconds = 0
    | Milliseconds = 1
    | Seconds = 2
    | Minutes = 3
    | Hours = 4

type Credentials =
    { Username: string
      Password: string }

type Serie =
    { Name: string // todo rename to Measurement?
      Tags: Map<string,string>
      Columns: string list
      Values: FieldValue[][] }

type ErrorMsg = string

type QueryResult = Choice<Serie list, ErrorMsg>

type ProxyCredentials =
    | No
    | Default
    | Custom of Credentials

type Proxy =
    { Address: string
      Port: uint16
      Credentials: ProxyCredentials }

type HttpError =
    | BadStatusCode of code: HttpStatusCode * msg: string option
    | OtherError of string * exn option
    with static member otherError str = OtherError (str, None)
         static member otherErrorExn (e: exn) = OtherError (e.Message, Some e)

type Error =
    | HttpError of HttpError
    | ResponseParseError of string
    | ServerError of string
