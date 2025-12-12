# Avalonia App with Host

This is a template that can help you get quickly started with an Avalonia application that uses the `Host` package to manage the application lifecycle and dependency injection.

## Installing

To install the template, download the latest release from the [Releases page] and in the corresponding folder run:

```bash
dotnet new install AvaloniaAppUsingHost.Template.{Version}.nupkg
```

Where `{Version}` is the version you want to install.

You can now create a new solution with:

```bash
dotnet new avalonia-host 
```

You can use `-n` to give it a different name than the folder from which this command is called.

## Quick Overview

The template creates an application where `Microsoft.Extensions.Hosting` is used to manage the application lifecycle, dependency injection, and configuration.

### Key Features

#### ViewModel and View Setup

This solution provides a mechanism to hook up Views and ViewModels so that:

- The ViewModel is registered in a Service container
- There is automatic coupling between the View and the ViewModel

This coupling generates DataTemplates for binding in XAML. For example, given a ViewModel mapped to a Screen, you can bind it like this:

```xml
<ContentControl Content="{Binding Screen}" />
```

This also works with TabControl where a ViewModel can be bound to the SelectedItem property.

The `RegisterViews` method in `App.axaml.cs` handles this. The `AddViewModelAndRegisterView` extension method registers the ViewModel and couples it with the View to be added to the DataTemplates collection.

#### Screen Pages and Lifecycle Management

The template includes a `ScreenPage` base class that extends `ViewModelBase` with:

- **Title Property**: Displays in the tab view
- **CanClose Property**: Signals if the current screen can be closed
- **OnActivatedAsync()**: Override to perform operations when a screen is activated
- **CloseAsync()**: Override to perform cleanup operations before the screen is closed
- **Status Messages**: Support for submitting status messages displayed at the bottom of the screen

#### Long-Running Tasks

Infrastructure for handling asynchronous operations:

- **BaseProgressReportingTask**: Base class for long-running operations with progress reporting
- **StatusDataMessage** and **ProgressDataMessage**: Messaging support for status and progress updates
- Integrated with the MVVM Community Toolkit Messenger pattern

#### Global Error Handling

Exception handling support includes:

- Global exception handlers registered in `Program.cs`
- UIThread error handling in `App.axaml.cs` that logs errors to the MainWindowViewModel
- Error handling at multiple levels

#### Service Locator Pattern

Service locator implementation includes:

- `IServiceLocator` interface
- `ServiceCollectionServiceLocator` implementation
- Integration with Microsoft.Extensions.DependencyInjection
