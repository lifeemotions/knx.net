namespace InfluxDB.FSharp

open System

// todo replace signature file with just private DU constructor?
module Point =

    type T

    val create : Measurement -> Map<string,string> -> Map<string,FieldValue> -> DateTime option -> Choice<T, string list>
    val toLine : Precision -> T -> string
