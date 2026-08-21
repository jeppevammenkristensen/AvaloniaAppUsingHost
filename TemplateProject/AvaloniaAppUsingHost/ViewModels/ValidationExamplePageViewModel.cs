using System.ComponentModel.DataAnnotations;
using AvaloniaAppUsingHost.Infrastructure;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AvaloniaAppUsingHost.ViewModels;

/// <summary>
/// Demonstrates data-annotation validation in a tab screen.
/// </summary>
public partial class ValidationExamplePageViewModel : ValidatingScreenPage
{
    /// <summary>
    /// Gets the title displayed in the tab view.
    /// </summary>
    public override string Title => "Validation example";

    /// <summary>
    /// Gets or sets the required name entered by the user.
    /// </summary>
    [ObservableProperty]
    [Required(ErrorMessage = "Enter your name before submitting.")]
    public partial string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets the result of the most recent validation attempt.
    /// </summary>
    [ObservableProperty]
    public partial string ValidationResult { get; set; } = string.Empty;

    /// <summary>
    /// Validates the name whenever its value changes.
    /// </summary>
    partial void OnNameChanged(string value)
    {
        ValidateProperty(value, nameof(Name));
    }

    /// <summary>
    /// Validates the screen's properties and reports whether the input is valid.
    /// </summary>
    [RelayCommand]
    private void Validate()
    {
        ValidateAllProperties();
        ValidationResult = HasErrors
            ? "Correct the validation error before submitting."
            : $"Thanks, {Name}! Your input is valid.";
    }
}