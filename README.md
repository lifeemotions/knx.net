# KNX.net

KNX.net provides a [KNX](http://en.wikipedia.org/wiki/KNX_%28standard%29) API for C#

This API allows to connect in both modes:
* Tunneling
* Routing

After connecting you will be able to send actions to the bus and receive messages from it.

The following datapoints are available in the API:
* bit (lights, buttons)
* byte (dimmers, temperature difference, RGB)
* 9.001 (temperatures)

There may be some bugs on the implementation as I don't have access to KNX documentation, many information about the protocol is from [OpenRemote](http://www.openremote.org) Knowlodge Base.

# Examples

## Connecting using Routing (turn off and on a light)

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

## Working with 9.001 datapoints

Sending an action

```csharp
connection.Action("1/1/16", connection.toDPT("9.001", 24.0f));
```

Converting status from event

```csharp
float temp = (float)connection.fromDPT("9.001", state);
```

## Connecting using Tunneling

The only difference is how the connection object is created

```csharp
connection = new KNXConnectionTunneling(remoteIP, remotePort, localIP, localPort);
```
