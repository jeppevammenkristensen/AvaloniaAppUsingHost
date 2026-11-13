# Avalonia App with Host

[![AvaloniaAppUsingHost.Template](https://img.shields.io/nuget/v/AvaloniaAppUsingHost.Template?style=flat-square&label=NuGet%3A%20AvaloniaAppUsingHost.Template)](https://www.nuget.org/packages/AvaloniaAppUsingHost.Template/)

This is a template that can help you get quickly started with an Avalonia application that uses the `Host` package to manage the application lifecycle and dependency injection.

## Installing

To install the template run

```bash
dotnet new install AvaloniaAppUsingHost.Template.{Version}.nupkg
```

Where `{Version}` is the version you want to install.

You can now create a new solution with:

From version 2.1.0 forward instead install directly with

```bash
dotnet new install AvaloniaAppUsingHost.Template
```

Use is like this to create it in the current folder with the given name

```bash
dotnet new avalonia-host -n OtherName
```

or in a new folder 

```bash
dotnet new avalonia-host -o OutputFolder
```

You can use `-n` to give it a different name than the folder from which this command is called.

### Target framework

The template supports .NET 10 and .NET 11 (the default):

```powershell
dotnet new avalonia-host -n MyNet10App --Framework net10.0
dotnet new avalonia-host -n MyNet11App --Framework net11.0
```

Install the .NET 10 SDK for the .NET 10 variant, or the .NET 11 SDK for the
.NET 11 variant. Running a framework-dependent application also requires the
corresponding runtime. Both choices generate a single-target application with a
`.slnx` solution. Microsoft.Extensions.Hosting is 10.0.15 for .NET 10 and 11.0.0
for .NET 11; the remaining dependencies are shared.

### Package and validate a release

From the repository root, pack without publishing:

```powershell
nuget pack ./TemplateProject/AvaloniaAppUsingHost.Template.nuspec -OutputDirectory ./artifacts
```

With both SDKs installed, validate the packed template:

```powershell
dotnet run ./Scripts/Test-Template.cs -- --package-path ./artifacts/AvaloniaAppUsingHost.Template.4.1.0.nupkg
```

This checks template metadata, default selection, rejection of an unsupported
framework, generated framework/Hosting versions, and Debug/Release builds for both
variants. It uses an isolated template hive and temporary output, without modifying
your installed templates. Each build is pinned to the corresponding SDK major
version. On a machine or CI job with only SDK 10 installed, run the same command
with `--framework net10.0`; use `--framework net11.0` for an SDK 11-only job.
The validator is a FileBasedApp.Toolkit file-based app and requires SDK 10 or later.
Package paths are resolved relative to the current working directory.
Run both jobs before releasing, and manually smoke-test application startup on
supported desktop platforms.

After validation, publish the package separately using your normal NuGet release
process. `Scripts/build.cs` packs **and pushes**, so do not use it for a pack-only
check.

*IMPORTANT* Please avoid naming in -o like Foo-Bar as it will generate uncompilable solution. The fix is

- Rename the path in the solution file from Foo_Bar to Foo-Bar
- In App.axaml. For the two ResourceInlude also rename for Foo_Bar to Foo-Bar

## Shameless self promotion

I have developed a Jetbrains Rider/Resharper plugin that can help with MVVM based apps called. You can find it here <https://plugins.jetbrains.com/plugin/26397-mvvm-helper/> it can be useful for developing this kind of application. And I use it myself.

It's completely free and open source.

## Quick Overview

The template creates an application where `Microsoft.Extensions.Hosting` is used to manage the application lifecycle, dependency injection, and configuration.

### Key Features

#### Included Examples

The generated application starts with an empty workspace where you can launch any of the included examples from the workspace or the **File** menu:

- **Primary screen**: Demonstrates the standard `ScreenPage` lifecycle and `CanClose` behavior
- **Validation**: Demonstrates data-annotation validation with CommunityToolkit.Mvvm's `ObservableValidator`
- **Tree view**: Demonstrates hierarchical data, resource-backed icons, selection, and a resizable details pane

#### ViewModel and View Setup

This solution provides a mechanism to hook up Views and ViewModels so that:

- The ViewModel is registered in a Service container
- There is automatic coupling between the View and the ViewModel

This coupling generates DataTemplates for binding in XAML. For example, given a ViewModel mapped to a Screen, you can bind it like this:

```xml
<ContentControl Content="{Binding Screen}" />
```

This also works with TabControl where a ViewModel can be bound to the SelectedItem property.

The `RegisterViews` method in `App.axaml.cs` handles this. The `AddViewModelAndRegisterView` extension method registers the ViewModel and couples it with the View to be added to the DataTemplates collection. Registered ViewModels can derive from any `ObservableObject` type; regular screens normally derive from `ScreenPage`, while validation screens can derive from `ValidatingScreenPage`.

#### Screen Pages and Lifecycle Management

Navigation uses the `IScreenPage` contract, allowing screens with different ViewModel base classes to share the same tab lifecycle. The template provides:

- **ScreenPage**: The standard `ViewModelBase` implementation with status-message helpers
- **ValidatingScreenPage**: An `ObservableValidator` implementation for screens that use data-annotation validation

Both implementations provide:

- **Title Property**: Displays in the tab view
- **CanClose Property**: Signals if the current screen can be closed
- **OnActivatedAsync()**: Override to perform operations when a screen is activated
- **CloseAsync()**: Override to perform cleanup operations before the screen is closed

`MainWindowViewModel` stores and displays screens as `IScreenPage` instances.

#### Tab Navigation and Empty Workspace

After the startup task completes, the application displays an empty workspace instead of opening a tab automatically. Screens can be launched from the workspace or the **File** menu. Open tabs include a close button and a **Close** context-menu command, both of which respect the screen's `CanClose` value. Closing the final tab returns to the empty workspace.

#### Long-Running Tasks

Infrastructure for handling asynchronous operations:

- **BaseProgressReportingTask**: Base class for long-running operations with progress reporting
- **StartupTask**: Example cancellable startup operation with status and progress reporting
- **StatusValueDataMessage** and **ProgressDataMessage**: Messaging support for status and progress updates
- **StatusMessage** and **StatusType**: Information, error, and success status presentation
- **MessengerExtensions**: `SendInformation`, `SendError`, and `SendSuccess` helpers
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

## Using AI with the Template

The generated solution ships with `AGENTS.md` and `CLAUDE.md` so AI coding assistants follow the project's conventions. A few short prompts that work well:

### Create a view + view model

> Create a new view called `SettingsPage` with a matching view model, and register them in `App.axaml.cs`.

The assistant will scaffold `Views/SettingsPage.axaml` (+ `.axaml.cs`), `ViewModels/SettingsPageViewModel.cs` deriving from `ScreenPage`, and add `AddViewModelAndRegisterView<SettingsPageViewModel, SettingsPage>(...)` to `RegisterViews`. For a data-annotation form, ask it to derive the ViewModel from `ValidatingScreenPage` instead.

### Add a menu item that launches it

> Add a menu item under `File` in `MainWindow` that launches the `SettingsPageViewModel` as a new tab.

The assistant will add a `<MenuItem>` to `MainWindow.axaml` and a `[RelayCommand]`-decorated method on `MainWindowViewModel` that resolves the view model via `_locator` and calls `Launch(...)`.

### Example projects using the template

- https://github.com/jeppevammenkristensen/DacPac_Viewer An UI for opening Dacpac files for analysis. Using Velopack for installation
