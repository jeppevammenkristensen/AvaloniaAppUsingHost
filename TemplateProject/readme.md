# AvaloniaAppUsingHost

An Avalonia desktop app wired up with `Microsoft.Extensions.Hosting` so you get dependency injection, configuration, and logging from the start.

## What happens on startup

1. `Program.Main` builds the Avalonia `AppBuilder` and registers global exception handlers.
2. `App.OnFrameworkInitializationCompleted` builds an `IHost` via `Host.CreateDefaultBuilder`, registering view/view-model pairs in `RegisterViews` and other services in `RegisterOtherDependencies`.
3. `MainWindowViewModel` is resolved from the container and set as `DataContext` on a new `MainWindow`.
4. UI-thread exceptions surface on `MainWindowViewModel.Status` and are logged through `ILogger<App>`.

## Tooling

`AGENTS.md` and `CLAUDE.md` ship with the project so AI coding assistants follow the conventions above. A few short prompts that work well:

### Create a view + view model

> Create a new view called `SettingsPage` with a matching view model, and register them in `App.axaml.cs`.

Scaffolds `Views/SettingsPage.axaml` (+ `.axaml.cs`), `ViewModels/SettingsPageViewModel.cs` deriving from `ScreenPage`, and adds `AddViewModelAndRegisterView<SettingsPageViewModel, SettingsPage>(...)` to `RegisterViews`.

### Add a menu item that launches it

> Add a menu item under `File` in `MainWindow` that launches the `SettingsPageViewModel` as a new tab.

Adds a `<MenuItem>` to `MainWindow.axaml` and a `[RelayCommand]`-decorated method on `MainWindowViewModel` that resolves the view model via `_locator` and calls `Launch(...)`.
