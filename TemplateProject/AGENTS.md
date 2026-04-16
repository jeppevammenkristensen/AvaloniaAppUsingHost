# Best Practices

## Important: Project Guidelines
When working on this project, ALWAYS follow the instructions in this file. It contains the standard patterns for creating Views, ViewModels, and UI interactions.

## Creating Views and ViewModels

The project heavily uses CommunityToolkit.MVVM especially for the ViewModels

When a View and a ViewModel is defined (look at When creating a new view with a ViewModel) you can register it by following the instrutions in step 3


When creating a new View with a ViewModel:

1. **View**: Create the `.axaml` and `.axaml.cs` files in a relevant subfolder under `Views/` (or in the `Views/` root if no subfolder applies). If possible create it through the ide (default Avalonia UserControl)
2. **ViewModel**: Mirror the same folder structure under `ViewModels/` and name the class `<ViewName>ViewModel`. For example, a view `Views/Settings/SettingsPage.axaml` gets a viewmodel at `ViewModels/Settings/SettingsPageViewModel.cs`. 
3. **Registration**: Register the View and ViewModel pair in `App.axaml.cs` inside the `RegisterViews` method using `AddViewModelAndRegisterView<TViewModel, TView>()`, choosing `Singleton` or `Transient` scope as appropriate.
4. **ViewModel**: Are always in some way derived from ObservableObject. But use a type relevant to the ViewModel. Typically ScreenPage for a UserControl. Prefer UserControls over Window.
5. **Prompt**: After creating the View and ViewModel, ask the user if they also want a menu item added to launch it.

## Adding a Menu Item to Launch a ViewModel

To add a menu item in `MainWindow.axaml` that launches a ViewModel as a new tab:

1. If not defined ask the user where it should be placed
2. **View**: Add a `<MenuItem>` inside the `<Menu>` in `MainWindow.axaml`. Example: `<MenuItem Header="Open Foo" Command="{Binding LaunchFooCommand}" />`
2. **ViewModel**: In `MainWindowViewModel.cs`, add a `CanExecute` method that returns `true` (or applies custom logic): `private bool CanExecuteLaunchFoo() => true;`
3. **ViewModel**: Add a method decorated with `[RelayCommand(CanExecute = nameof(CanExecuteLaunchFoo))]` that resolves the ViewModel via `_locator` and calls `Launch()`:
   ```csharp
   [RelayCommand(CanExecute = nameof(CanExecuteLaunchFoo))]
   private async Task LaunchFoo()
   {
       var screen = _locator.GetRequiredService<FooViewModel>();
       await Launch(screen);
   }
   ```
4. Consider whether the launch method needs to be async (it typically is, since `Launch` calls `OnActivatedAsync`).

