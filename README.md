KNX.net ![build status](https://travis-ci.org/lifeemotions/knx.net.svg?branch=master) ![nuget status](http://img.shields.io/nuget/v/KNX.net.svg?style=flat)
=======

KNX.net provides a [KNX](http://en.wikipedia.org/wiki/KNX_%28standard%29) API for C#

This API allows to connect in both modes:
* Tunneling
* Routing

After connecting you will be able to send actions to the bus and receive messages from it.

The following datapoints are available in the API:

| DPT     | input type                                  | input range     | output type | output range    | Description                         |
| ------- |-------------------------------------------- | --------------- | ----------- | --------------- | ----------------------------------- |
| `3.007` | `int`, `float`, `long`, `double`, `decimal` | `[-7,7]`        | `int`       | `[-7,7]`        | Control blinds (steps) [`0` stops]  |
| `3.008` | `int`, `float`, `long`, `double`, `decimal` | `[-7,7]`        | `int`       | `[-7,7]`        | Control dimming (steps) [`0` stops] |
| `5.001` | `int`, `float`, `long`, `double`, `decimal` | `[0,100]`       | `decimal`   | `[0,100]`       | Percentage (%)                      |
| `5.003` | `int`, `float`, `long`, `double`, `decimal` | `[0,360]`       | `decimal`   | `[0,360]`       | Angle (°)                           |
| `5.004` | `int`, `float`, `long`, `double`, `decimal` | `[0,255]`       | `int`       | `[0,255]`       | Percentage [0,255] (%)              |
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
  var connection = new KNXConnectionRouting();
  connection.Connect();
  connection.KNXEventDelegate += new KNXConnection.KNXEvent(Event);
  connection.Action("5/0/2", false);
  Thread.Sleep(5000);
  connection.Action("5/0/2", true);
  Thread.Sleep(5000);
}
static void Event(string address, string state)
{
  Console.WriteLine("New Event: device " + address + " has status " + state);
}
```

### Working with datapoints

Sending an action

```csharp
connection.Action("1/1/16", connection.toDPT("9.001", 24.0f));
connection.Action("1/1/17", connection.toDPT("5.001", 50));
```

Converting status from event

```csharp
float temp = (float)connection.fromDPT("9.001", state);
```

### Sending actions without using datapoints

```csharp
connection.Action("1/1/19", true);
connection.Action("1/1/20", false);
connection.Action("1/1/21", 60);
```

### Connecting using Tunneling

The only difference is how the connection object is created

```csharp
connection = new KNXConnectionTunneling(remoteIP, remotePort, localIP, localPort);
```

### Notes

If connecting in routing mode:
* make sure the system firewall allows incoming connections to the routing port ()if not specified when connecting, default is `3671`)

If connecting in tunneling mode:
* make sure the system firewall allows incoming connections to the specified `localPort`
