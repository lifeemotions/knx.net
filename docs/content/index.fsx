(*** hide ***)
// This block of code is omitted in the generated HTML documentation. Use
// it to define helpers that you do not want to show in the documentation.
#I "../../bin"

(**
knx.net [![Travis build status](https://travis-ci.org/lifeemotions/knx.net.png?branch=master)](https://travis-ci.org/lifeemotions/knx.net) [![NuGet Status](http://img.shields.io/nuget/v/KNX.net.svg?style=flat)](https://www.nuget.org/packages/KNX.net/)
======================

<div class="row">
  <div class="span1"></div>
  <div class="span6">
    <div class="well well-small" id="nuget">
      The knx.net library can be <a href="https://nuget.org/packages/KNX.net">installed from NuGet</a>:
      <pre>PM> Install-Package KNX.net</pre>
    </div>
  </div>
  <div class="span1"></div>
</div>

Introduction
------------

KNX.net provides a [KNX](http://en.wikipedia.org/wiki/KNX_%28standard%29) API for .NET

This API allows to connect in both modes:

  - Tunneling
  - Routing

After connecting you will be able to send actions to the bus and receive messages from it.

The following datapoints are available in the API:

  - bit (lights, buttons)
  - byte (dimmers, temperature difference, RGB)
  - 9.001 (temperatures)

There may be some bugs on the implementation as I don't have access to KNX documentation, many information about the protocol is from [OpenRemote](http://www.openremote.org) Knowledge Base.

Example
-------

This example demonstrates turning off and on a light using Routing

*)
static void Main(string[] args)
{
  var connection = new KnxConnectionRouting();
  connection.Connect();
  connection.KnxEventDelegate += new KnxConnection.KnxEvent(Event);
  connection.Action("5/0/2", false);
  Thread.Sleep(5000);
  connection.Action("5/0/2", true);
  Thread.Sleep(5000);
}

static void Event(string address, string state)
{
  Console.WriteLine("New Event: device " + address + " has status " + state);
}
(**

Working with 9.001 datapoints
-------

Sending an action
*)
connection.Action("1/1/16", connection.ToDataPoint("9.001", 24.0f));
(**

Converting status from event
*)
float temp = (float)connection.FromDataPoint("9.001", state);
(**

Connecting using Tunneling
-------

The only difference is how the connection object is created
*)
connection = new KnxConnectionTunneling(remoteIP, remotePort, localIP, localPort);
(**

Documentation
-------------

 * [API Reference](reference/index.html) contains automatically generated documentation for the library.

Contributing and copyright
--------------------------

The project is hosted on [GitHub][gh] where you can [report issues][issues], fork
the project and submit pull requests. If you're adding a new public API, please also
consider adding [samples][content] that can be turned into a documentation. You might
also want to read the [library design notes][readme] to understand how it works.

The library is available under MIT license, which allows modification and
redistribution for both commercial and non-commercial purposes. For more information see the
[License file][license] in the GitHub repository.

  [content]: https://github.com/lifeemotions/knx.net/tree/master/docs/content
  [gh]: https://github.com/lifeemotions/knx.net
  [issues]: https://github.com/lifeemotions/knx.net/issues
  [readme]: https://github.com/lifeemotions/knx.net/blob/master/README.md
  [license]: https://github.com/lifeemotions/knx.net/blob/master/LICENSE.txt
*)
