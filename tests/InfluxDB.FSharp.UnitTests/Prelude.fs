[<NUnit.Framework.TestFixture>]
module InfluxDB.FSharp.UnitTests.Prelude

open NUnit.Framework
open InfluxDB.FSharp

[<Test>]
let ``Seq.trySingle`` () =
    Seq.trySingle [1] =? Some 1
    Seq.trySingle [] =? None
    Seq.trySingle [1; 2] =? None

let testFn fn = TestDelegate (fun () -> fn() |> ignore)

[<Test>]
let ``Seq.single`` () =
    Seq.single [1] =? 1
    Assert.That(testFn (fun () -> Seq.single []), Throws.Exception.With.Message.EqualTo("Expected seq with signle element, but there is no one"))
    Assert.That(testFn (fun () -> Seq.single [1; 2]), Throws.Exception.With.Message.EqualTo("Expected seq with signle element, but there is more than one"))

[<Test>]
let ``DateTime.toUnixMicroseconds`` () =
    DateTime.toUnixMicroseconds (DateTime.unixStartEpoch.AddMilliseconds 1.) =? int64 1e3
    DateTime.toUnixMicroseconds (DateTime.unixStartEpoch.AddMilliseconds -1.) =? int64 -1e3

[<Test>]
let ``DateTime.toUnixMilliseconds`` () =
    DateTime.toUnixMilliseconds (DateTime.unixStartEpoch.AddMilliseconds 1.) =? 1L
    DateTime.toUnixMilliseconds (DateTime.unixStartEpoch.AddMilliseconds -1.) =? -1L

[<Test>]
let ``DateTime.toUnixSeconds`` () =
    DateTime.toUnixSeconds (DateTime.unixStartEpoch.AddSeconds 1.) =? 1L
    DateTime.toUnixSeconds (DateTime.unixStartEpoch.AddSeconds -1.) =? -1L

[<Test>]
let ``DateTime.toUnixMinutes`` () =
    DateTime.toUnixMinutes (DateTime.unixStartEpoch.AddMinutes 1.) =? 1L
    DateTime.toUnixMinutes (DateTime.unixStartEpoch.AddMinutes -1.) =? -1L

[<Test>]
let ``DateTime.toUnixHours`` () =
    DateTime.toUnixHours (DateTime.unixStartEpoch.AddHours 1.) =? 1L
    DateTime.toUnixHours (DateTime.unixStartEpoch.AddHours -1.) =? -1L
