# Meadow.Logging Library

## Status

[![CI Build](https://github.com/WildernessLabs/Meadow.Logging/actions/workflows/logging-build.yml/badge.svg)](https://github.com/WildernessLabs/Meadow.Logging/actions/workflows/logging-build.yml)
[![Meadow.Logging Latest Binaries](https://github.com/WildernessLabs/Meadow.Logging/actions/workflows/logging-binaries.yml/badge.svg)](https://github.com/WildernessLabs/Meadow.Logging/actions/workflows/logging-binaries.yml)

## Summary

Many good logging libraries already exist in the .NET development community, so why `Meadow.Logging`?  

The primary driver is that `Meadow` is designed to run on microcontrollers, where storage and memory resources are limited.  Most existing libraries provide a wide array of capabilties and features, but at the cost of size.  `Meadow.Logging` is intended to be very, very lean, only exposing the minimal set of what might be needed by a typical Meadow application, but still completely usable in larger, non-Meadow application scenarios.

## `Logger`

The `Logger` class has two functions:

- Providing the methods an Application would typically call in order to add a log entry.
- Holding a collection of `ILogProvider` instances that will react to those methods.

## `ILogProvider`

The `ILogProvider` interface provides the methods that the `Logger` will call when an Application makes a specific Logging call that meets the currently set Loglevel.

Two common implementations are inluded in the compiled library:

- `ConsoleLogger` outputs log information to `System.Console`
- `DebugLogger` outputs log information to `System.Diagnostics.Debug`

Since a primary goal of `Meadow.Logging` is to be small, some additional implementations (e.g. UDP broadcast logging or SQLIte logging) will likely be provided in source form until linking is fully supported by Meadow.

## Usage

The general usage pattern is:

- Create a `Logger` instance
- Register one or more `ILogProvider` implementations
- Access the logger instance using a static, dependency injection, or whatever your preferred mechanism

### Basic Usage

A simple example of `Console` output would look like this:

```
static void Main()
{
	var logger = new Logger(new ConsoleLogProvider());
	var App = new App(logger);

	app.DoStuff();
}

class App
{
	Logger Logger { get; init; }

	App(Logger logger)
	{
		Logger = logger;
	}

	public void DoStuff()
	{
		while(true)
		{
			Logger.Debug("tick");
			Thread.Sleep(1000);
		}
	}
}

```

### Using Multiple `ILogProvider`s

The `Logger` allows you to add 0:N `ILogProvider` instances, and all of them get called when ap application makes a logging call.  For example, your application may want to output to both the Console and a UDP broadcast.  You can achieve this by simply registering multiple providers:

```
	var logger = new Logger();
	logger.AddProvider(new ConsoleLogProvider());
	logger.AddProvider(new UdpLogProvider());
	...
	logger.Info("This message will get delivered to all Providers");
```

### Conditional Output Using LogLevel

Logger-wide output can be controlled using the `LogLevel` property.  Log output will occur for any level *at or above* the currently set level.

```
	var logger = new Logger(new ConsoleLogProvider());
	...
	logger.Loglevel = LogLevel.Debug;
	logger.Debug("This debug message will get delivered to all Providers");
	logger.Warn("This warning message will get delivered to all Providers");
	...
	logger.Loglevel = LogLevel.Info;
	logger.Debug("This debug message will *not* get delivered to any Providers");
	logger.Warn("This warning message will still get delivered to all Providers");
```

### Conditional Output Using Runtime Checks 

Occasionally you may want to keep a given log level, but output information based on a runtime check.  All of the logging outputs support this as well.

```
	var logger = new Logger(new ConsoleLogProvider());
	bool logCondition = true;
	...
	logger.Debug("This debug message will get delivered to all Providers");
	logger.Debug(logCondition, "This debug message will also get delivered to all Providers");
	bool logCondition = false;
	logger.Debug("This debug message will also get delivered to all Providers");
	logger.Debug(logCondition, "This debug message will *not* get delivered to any Providers");
```
