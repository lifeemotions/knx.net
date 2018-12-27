KNX.net
=======

KNX.net provides a [KNX](http://en.wikipedia.org/wiki/KNX_%28standard%29) API for C#

This API allows to connect in both modes:
* Tunneling
* Routing

After connecting you will be able to send actions to the bus and receive messages from it.

The following datapoints are available in the API:

| DPT     | input type                                  | input range     | output type | output range    | Description                         |
| ------- |-------------------------------------------- | --------------- | ----------- | --------------- | ----------------------------------- |
| `3.007` | `int`, `float`, `long`, `double`, `decimal` | `[-7,7]`        | `int`       | `[-7,7]`        | Control dimming (steps) [`0` stops] |
| `3.008` | `int`, `float`, `long`, `double`, `decimal` | `[-7,7]`        | `int`       | `[-7,7]`        | Control blinds (steps) [`0` stops]  |
| `5.001` | `int`, `float`, `long`, `double`, `decimal` | `[0,100]`       | `decimal`   | `[0,100]`       | Percentage (%)                      |
| `5.003` | `int`, `float`, `long`, `double`, `decimal` | `[0,360]`       | `decimal`   | `[0,360]`       | Angle (°)                           |
| `5.004` | `int`, `float`, `long`, `double`, `decimal` | `[0,255]`       | `int`       | `[0,255]`       | Percentage `[0,255]` (%)            |
| `5.010` | `int`, `float`, `long`, `double`, `decimal` | `[0,255]`       | `int`       | `[0,255]`       | Counter Pulses                      |
| `6.001` | `int`, `float`, `long`, `double`, `decimal` | `[-128,127]`    | `int`       | `[-128,127]`    | Percentage (%)                      |
| `6.010` | `int`, `float`, `long`, `double`, `decimal` | `[-128,127]`    | `int`       | `[-128,127]`    | Counter Pulses                      |
| `9.001` | `int`, `float`, `long`, `double`, `decimal` | `[-273,670760]` | `decimal`   | `[-273,670760]` | Temperature in Celsius (°C)         |

Also working but no implemented as datapoints (see below for better explanation):
* bit (lights, buttons)
* byte (dimmers, temperature difference, RGB)

Examples
--------

### Connecting using Routing (turn off and on a light)

```csharp
static void Main(string[] args)
{
  var connection = new KnxConnectionRouting();
  connection.Connect();
  connection.GroupAddressStyle = KnxGroupAddressStyle.ThreeLevel;
  connection.KnxEventDelegate += Event;
  
  // Parse GroupAddress from a string
  connection.Action(KnxGroupAddress.Parse("5/0/2"), false);
  Thread.Sleep(5000);
  
  // Instantiate a three level GroupAddress
  connection.Action(new KnxThreeLevelGroupAddress(5, 0, 2), true);
  Thread.Sleep(5000);
}

static void Event(object sender, KnxEventArgs args)
{
  Console.WriteLine($"New Event: device {args.DestinationAddress} has status {args.State}");
}
```

### Working with datapoints

Sending an action

```csharp
connection.Action(new KnxThreeLevelGroupAddress(1, 1, 16), connection.ToDataPoint("9.001", 24.0f));
connection.Action(KnxGroupAddress.Parse("1/1/17"), connection.ToDataPoint("5.001", 50));
```

Converting state from event

```csharp
static void Event(object sender, KnxEventArgs args)
{
  if (args.DestinationAddress.Equals(new KnxThreeLevelGroupAddress(1, 1, 16)))
  {
    decimal temp = (decimal)connection.FromDataPoint("9.001", args.State);
    Console.WriteLine($"New Event: device {args.DestinationAddress} has status {temp}");
    return;
  }

  if (args.DestinationAddress.ToString() == "1/1/17")
  {
    int perc = (int)connection.FromDataPoint("5.001", args.State);
    Console.WriteLine($"New Event: device {args.DestinationAddress} has status {perc}");
    return;
  }
}
```

### Requesting status

Sending an action

```csharp
connection.KnxStatusDelegate += Status;
connection.RequestStatus(KnxGroupAddress.Parse("1/1/16"));
connection.RequestStatus(KnxGroupAddress.Parse("1/1/17"));
```

Converting state from status event

```csharp
private static void Status(object sender, KnxStatusArgs args)
{
  // Process only GroupAddresses (using pattern matching in C# 7.0)
  if (!(args.DestinationAddress is KnxGroupAddress groupAddress))
    return;
    
  if (groupAddress.Equals(1, 1, 6))
  {
    decimal temp = (decimal)connection.FromDataPoint("9.001", args.State);
    Console.WriteLine($"New Status: device {addr} has status {temp}");
    return;
  }

  if (groupAddress.Equals("1/1/17"))
  {
    int perc = (int)connection.FromDataPoint("5.001", args.State);
    Console.WriteLine($"New Status: device {addr} has status {perc}");
    return;
  }
}
```

### Sending actions without using datapoints

```csharp
connection.Action(KnxGroupAddress.Parse("1/1/19"), true);
connection.Action(KnxGroupAddress.Parse("1/1/20"), false);
connection.Action(KnxGroupAddress.Parse("1/1/21"), 60);
connection.Action(KnxGroupAddress.Parse("1/1/22"), 0x4E);
```

### Connecting using Tunneling

The only difference is how the connection object is created

```csharp
connection = new KNXConnectionTunneling(remoteIP, remotePort, localIP, localPort);
```

### Setting GroupAddressStyle

KNX supports 3 ways to represent GroupAddresses:
* 3-Level: MainGroup/MiddleGroup/SubGroup
* 2-Level: MainGroup/SubGroup
* Free: SubGroup

The used style is not transmitted with the packets and is only an artifical representation of a 2-byte address. To allow KNX.net to correctly translate received addresses you need to specify which style to use:

```csharp
connection.GroupAddressStyle = KnxGroupAddressStyle.ThreeLevel;
connection.GroupAddressStyle = KnxGroupAddressStyle.TwoLevel;
connection.GroupAddressStyle = KnxGroupAddressStyle.Free;
```

KNX.net supports extended group addresses, i.e. full 16 bit addresses.

### Notes

If connecting in routing mode:
* make sure the system firewall allows incoming connections to the routing port (if not specified when connecting, default is `3671`)

If connecting in tunneling mode:
* make sure the system firewall allows incoming connections to the specified `localPort`

If sending actions is not working for you, you might need to define the parameter `ActionMessageCode` on the connection. Some KNX Router/Interfaces need this to be set to 0x29 for example.

```csharp
connection = new KnxConnectionRouting();
connection.ActionMessageCode = 0x29;
```

### Run Tests
`./packages/NUnit.ConsoleRunner/tools/nunit3-console.exe ./tests/KNXLibTests/bin/Release/KNXLibTests.dll  --labels=On --nocolor --verbose --workers=1 --full --result:"./nunit-result.xml;format=nunit2"`

`./packages/NUnit.ConsoleRunner/tools/nunit3-console.exe ./tests/InfluxDB.FSharp.UnitTests/bin/Release/InfluxDB.FSharp.UnitTests.dll  --labels=On --nocolor --verbose --workers=1 --full --result:"./nunit-result.xml;format=nunit2"`

`./packages/xunit.runners/tools/xunit.console.clr4.exe ./tests/FunctionalLiving.Parser.Tests/bin/Release/FunctionalLiving.Parser.Tests.dll`
