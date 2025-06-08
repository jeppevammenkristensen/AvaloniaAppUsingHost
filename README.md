# Avalonia App with Host

This is a template that can help you get quickly started with an Avalonia application that uses the `Host` package to manage the application lifecycle and dependency injection.

## Installing

To install the Application download the latest release from the [Releases page] and in the corresponding folder run

```bash
dotnet new install AvaloniaAppUsingHost.Template.{Version}.nupkg
```

Where {Version} is the version you want to install.

You can now create a new solution with

```bash
dotnet new avalonia-host 
```

Remember you can use -n to give it a different name than the folder from which this command is called

## Quick overview

The template creates an application where `Microsoft.Extensions.Hosting` is used to wire it all up. The solution provides the following

### ViewModel and View setup

This solution provides a mechanism to hook up View and ViewModel so that

* The ViewModel is registered in a Service container
* There is a coupling between the View and the ViewModel

 This coupling is used to generate DataTemplates which allow relative easy binding in XAML. For instance. Given a ViewModel mapped to Screen. The Xaml below would bind it.  

```xml
<ContentControl Content="{Binding Screen} />
```

It also works well with a TabControl where the given Screen can be mapped to SelectedItem.

In the `app.xaml.cs` file this is achieved in the `RegisterViews` method. `AddViewModelAndRegisterView` will both register the given ViewModel and combine the ViewModel and View so they are added to the DataTemplates
