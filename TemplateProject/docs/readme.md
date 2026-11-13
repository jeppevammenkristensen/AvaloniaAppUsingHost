# Avalonia App Using Host

This will spin up an Avalonia app using Microsoft.Hosting to set up the App and dependency injection

## Requirements

- .NET 11 SDK for `net11.0` (the default), or .NET 10 SDK for `net10.0`.
- The corresponding .NET runtime is required to run a framework-dependent application.

## Install

```bash
dotnet new install AvaloniaAppUsingHost.Template
```

## Run

```bash
dotnet new avalonia-host -n NameOfApp
```

Choose a framework explicitly:

```bash
dotnet new avalonia-host -n MyNet10App --Framework net10.0
dotnet new avalonia-host -n MyNet11App --Framework net11.0
```

Both choices generate a single-target application and a `.slnx` solution.
Hosting uses version 10.0.15 for .NET 10 and 11.0.0 for .NET 11; other
dependency versions are shared.
