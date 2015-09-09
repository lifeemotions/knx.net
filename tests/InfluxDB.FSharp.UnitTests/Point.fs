[<NUnit.Framework.TestFixture>]
module InfluxDB.FSharp.UnitTests.Point

open System
open NUnit.Framework
open InfluxDB.FSharp

let private overlongString =
    let maxLen = 64 * 1024
    let overlongLen = maxLen + 1
    System.String('a', overlongLen)

let private whitespaces = "   \t \r \n "

// line: cpu host=server01,region=uwest value=3.0 1420070400
let correct = { Measurement = "cpu"
                Tags = Map [ "host", "server01"; "region", "uwest" ]
                Fields = Map [ "value", Float 3.0 ]
                Timestamp = Some (DateTime(2015, 1, 1, 0, 0, 0, DateTimeKind.Utc)) }

// todo validation on field = Float NaN (or Infinity)?

[<Test>]
let ``can create correct point`` () =
    shouldNotFail (tryCreateFrom correct)

[<Test>]
let ``measurement validation`` () =
    shouldFailWith [ "measurement cant be null" ] (tryCreateFrom { correct with Measurement = null })
    shouldFailWith [ "measurement cant be empty string or contain only whitespace characters" ] (tryCreateFrom { correct with Measurement = "" })
    shouldFailWith [ "measurement cant be empty string or contain only whitespace characters" ] (tryCreateFrom { correct with Measurement = whitespaces })
    shouldFailWith [ "measurement length should be <= 64KB" ] (tryCreateFrom { correct with Measurement = overlongString })

[<Test>]
let ``tags validation`` () =
    shouldNotFail (tryCreateFrom { correct with Tags = Map.empty })

    shouldFailWith [ "tag key cant be null"; "tag value cant be null" ] (tryCreateFrom { correct with Tags = Map [ null, null ] })
    shouldFailWith [ "tag key cant be empty string or contain only whitespace characters"
                     "tag value cant be empty string or contain only whitespace characters" ] (tryCreateFrom { correct with Tags = Map [ "", whitespaces ] })
    shouldFailWith [ "tag key length should be <= 64KB"; "tag value length should be <= 64KB" ] (tryCreateFrom { correct with Tags = Map [ overlongString, overlongString ] })

[<Test>]
let ``fields validation`` () =
    shouldFailWith [ "at least one field is required" ] (tryCreateFrom { correct with Fields = Map.empty })

    shouldFailWith [ "field key cant be null" ] (tryCreateFrom { correct with Fields = Map [ null, Int 42L ] })
    shouldFailWith [ "field key cant be empty string or contain only whitespace characters" ] (tryCreateFrom { correct with Fields = Map [ "", Int 42L ] })
    shouldFailWith [ "field key length should be <= 64KB" ] (tryCreateFrom { correct with Fields = Map [ overlongString, Int 42L ] })

    shouldFailWith [ "field value cant be null" ] (tryCreateFrom { correct with Fields = Map [ "value", String null ] })
    shouldFailWith [ "field value cant be empty string or contain only whitespace characters" ] (tryCreateFrom { correct with Fields = Map [ "value", String "" ] })
    shouldFailWith [ "field value cant be empty string or contain only whitespace characters" ] (tryCreateFrom { correct with Fields = Map [ "value", String whitespaces ] })
    shouldFailWith [ "field value length should be <= 64KB" ] (tryCreateFrom { correct with Fields = Map [ "value", String overlongString ] })

[<Test>]
let ``timestamp validation`` () =
    shouldFailWith
        [ "timestamp cant be before UNIX start epoch (1970-01-01 00:00:00 UTC)" ]
        (tryCreateFrom { correct with Timestamp = Some (DateTime.unixStartEpoch.AddMilliseconds -1.) })

[<Test>]
let ``escaping`` () =
    let toLine data =
        let point = createFrom data
        Point.toLine Precision.Seconds point

    // measurement
    toLine { correct with Measurement = "cpu,01" } =? @"cpu\,01,host=server01,region=uwest value=3.0 1420070400"
    toLine { correct with Measurement = "cpu load" } =? @"cpu\ load,host=server01,region=uwest value=3.0 1420070400"

    // tags
    toLine { correct with Tags = Map [ "host name", "server,01" ] } =? @"cpu,host\ name=server\,01 value=3.0 1420070400"

    // fields
    toLine { correct with Fields = Map [ "key to,escape", Int 1L ] } =? @"cpu,host=server01,region=uwest key\ to\,escape=1i 1420070400"
    toLine { correct with Fields = Map [ "key", String "double\"quotes" ] } =? "cpu,host=server01,region=uwest key=\"double\\\"quotes\" 1420070400"

    // complex examples from influxdb docs
    toLine { Measurement = "total disk free"
             Tags = Map [ "volumes", "/net,/home,/" ]
             Fields = Map [ "value", Int 442221834240L ]
             Timestamp = correct.Timestamp } =? @"total\ disk\ free,volumes=/net\,/home\,/ value=442221834240i 1420070400"

    toLine { Measurement = "\"measurement with quotes\""
             Tags = Map [ "tag key with spaces", "tag,value,with\"commas\"" ]
             Fields = Map [ @"field_key\\\\", String "string field value, only \" need be quoted" ]
             Timestamp = None } =? @"""measurement\ with\ quotes"",tag\ key\ with\ spaces=tag\,value\,with""commas"" field_key\\\\=""string field value, only \"" need be quoted"""

