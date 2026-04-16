# Best Practices

## Creating Views and ViewModels

When creating a new View with a ViewModel:

1. **View**: Create the `.axaml` and `.axaml.cs` files in a relevant subfolder under `Views/` (or in the `Views/` root if no subfolder applies).
2. **ViewModel**: Mirror the same folder structure under `ViewModels/` and name the class `<ViewName>ViewModel`. For example, a view `Views/Settings/SettingsPage.axaml` gets a viewmodel at `ViewModels/Settings/SettingsPageViewModel.cs`.
3. **Registration**: Register the View and ViewModel pair in `App.axaml.cs` inside the `RegisterViews` method using `AddViewModelAndRegisterView<TViewModel, TView>()`, choosing `Singleton` or `Transient` scope as appropriate.
