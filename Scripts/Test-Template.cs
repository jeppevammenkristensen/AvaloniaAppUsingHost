#:package FileBasedApp.Toolkit@1.4.0
#:package FileBasedApp.Toolkit.SimpleExec@1.4.0
#:package FileBasedApp.Toolkit.Spectre@1.4.0
#:property PublishAot=false

using System.ComponentModel;
using System.IO.Abstractions;
using System.IO.Compression;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using FileBasedApp.Toolkit;
using FileBasedApp.Toolkit.CommandCli;
using FileBasedApp.Toolkit.SimpleExec;
using Spectre.Console;
using Spectre.Console.Cli;
using TruePath;
using TruePath.TestableIO.System.IO;

var app = new CommandApp<RunCommand>()
    .WithDescription("Validate a packed Avalonia template and build its selected framework variants.");
app.Configure(config => config.UseAssemblyInformationalVersion().PropagateExceptions());
return await app.RunAsync(args);

/// <summary>Checks packed template metadata and builds generated applications in an isolated workspace.</summary>
public sealed class RunCommand : AsyncCommand<RunCommand.Settings>
{
    private readonly IFileSystem fileSystem;

    /// <summary>Creates the command with the physical file system.</summary>
    public RunCommand() : this(new FileSystem()) { }

    /// <summary>Creates the command with an injectable file system.</summary>
    private RunCommand(IFileSystem fileSystem) => this.fileSystem = fileSystem;

    /// <summary>Validates the package and cleans up temporary output, including when a check fails.</summary>
    public override async Task<int> ExecuteAsync(
        CommandContext context, Settings settings, CancellationToken cancellationToken)
    {
        var work = Environment.SpecialFolder.LocalApplicationData.GetSpecialFolder()
                   / "Temp" / "Avalonia" / $"avalonia-template-{Guid.NewGuid():N}";
        var hive = work / "hive";
        work.CreateDirectory(fileSystem);
        try
        {
            AssertPackage(settings.PackageAbsolute);
            await NewRunner(hive).AddArguments("install").AddArgument(settings.PackageAbsolute)
                .RunAsync(token: cancellationToken);

            var defaultOutput = work / "default";
            await Generate(hive, defaultOutput, "DefaultApp")
                .RunAsync(token: cancellationToken);
            AssertProject(defaultOutput, "DefaultApp", "net10.0");

            var invalid = await Generate(hive, work / "invalid", "InvalidApp", "net9.0")
                .WithExitCodeHandler(_ => true)
                .RunWithExitCodeAsync(token: cancellationToken);
            Require(invalid.ExitCode != 0, "Unsupported framework was accepted.");

            var sdkResult = await SimpleExecRunner.Init("dotnet").AddArgument("--list-sdks")
                .ReadAsync(token: cancellationToken);
            foreach (var target in settings.SelectedFrameworks)
            {
                var major = target == "net10.0" ? 10 : 11;
                var sdk = SelectSdk(sdkResult.StandardOutput, major);
                var name = $"App{major}";
                var output = work / name;
                await Generate(hive, output, name, target).RunAsync(token: cancellationToken);
                AssertProject(output, name, target);

                // Pin the child process, not this validator, to the requested SDK.
                await (output / "global.json").WriteAllTextAsync(JsonSerializer.Serialize(new
                {
                    sdk = new { version = sdk, rollForward = "disable", allowPrerelease = false }
                }), cancellationToken, fileSystem);
                foreach (var configuration in new[] { "Debug", "Release" })
                {
                    await SimpleExecRunner.Init("dotnet")
                        .AddArguments("build", $"{name}.slnx", "--configuration", configuration)
                        .WithWorkingDirectory(output)
                        .RunAsync(token: cancellationToken);
                }
            }
            AnsiConsole.MarkupLine("[green]Template validation passed.[/]");
            return 0;
        }
        finally
        {
            work.DirectoryDelete(true, fileSystem);
        }
    }

    /// <summary>Checks metadata from the actual archive rather than trusting repository source files.</summary>
    private void AssertPackage(AbsolutePath package)
    {
        using var stream = package.OpenRead(fileSystem);
        using var archive = new ZipArchive(stream, ZipArchiveMode.Read);
        var entry = archive.GetEntry("content/.template.config/template.json")
                    ?? throw new InvalidOperationException("Package is missing template.json.");
        using var entryStream = entry.Open();
        using var document = JsonDocument.Parse(entryStream);
        var root = document.RootElement;
        Require(root.GetProperty("primaryOutputs")[0].GetProperty("path").GetString()
                == "AvaloniaAppUsingHost.slnx", "Primary output must be the .slnx solution.");
        var framework = root.GetProperty("symbols").GetProperty("Framework");
        Require(framework.GetProperty("defaultValue").GetString() == "net10.0",
            "The default framework must be net10.0.");
        var choices = framework.GetProperty("choices").EnumerateArray()
            .Select(choice => choice.GetProperty("choice").GetString())
            .Order(StringComparer.Ordinal);
        Require(choices.SequenceEqual(new[] { "net10.0", "net11.0" }),
            "Expected exactly the .NET 10 and .NET 11 choices.");
    }

    /// <summary>Checks renamed solution output, single targeting, and the matching Hosting dependency.</summary>
    private void AssertProject(AbsolutePath output, string name, string target)
    {
        var solution = output / $"{name}.slnx";
        Require(solution.FileExists(fileSystem), $"Missing generated solution: {solution}");
        var project = XDocument.Parse((output / name / $"{name}.csproj").ReadAllText(fileSystem));
        Require(project.Descendants("TargetFramework").Select(element => element.Value)
            .SequenceEqual(new[] { target }), $"Expected TargetFramework {target}.");
        Require(!project.Descendants("TargetFrameworks").Any(),
            "Generated application must be single-targeted.");
        var expectedHosting = target == "net10.0" ? "10.0.15" : "11.0.0";
        var hosting = project.Descendants("PackageReference")
            .SingleOrDefault(element => (string?)element.Attribute("Include") == "Microsoft.Extensions.Hosting");
        Require((string?)hosting?.Attribute("Version") == expectedHosting,
            $"Expected Hosting {expectedHosting} for {target}.");
    }

    /// <summary>Selects the newest stable SDK of the requested major, excluding previews.</summary>
    private static string SelectSdk(string sdkOutput, int major)
    {
        var versions = Regex.Matches(sdkOutput, $@"^{major}\.\d+\.\d+(?= \[)", RegexOptions.Multiline)
            .Select(match => Version.Parse(match.Value))
            .OrderDescending();
        return versions.FirstOrDefault()?.ToString()
               ?? throw new InvalidOperationException($"Install a stable .NET {major} SDK.");
    }

    /// <summary>Creates a template command that cannot change the user's installed templates.</summary>
    private static SimpleExecRunner NewRunner(AbsolutePath hive) =>
        SimpleExecRunner.Init("dotnet").AddArguments("new", "--debug:custom-hive").AddArgument(hive);

    /// <summary>Constructs a generation command, omitting Framework when checking the default.</summary>
    private static SimpleExecRunner Generate(
        AbsolutePath hive, AbsolutePath output, string name, string? framework = null)
    {
        var runner = NewRunner(hive).AddArguments("avalonia-host", "-n", name)
            .AddArgumentPair("-o", output);
        if (framework is not null)
            runner.AddArgumentPair("--Framework", framework);
        return runner;
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
            throw new InvalidOperationException(message);
    }

    /// <summary>Accepts a package path relative to the caller and optional repeated framework choices.</summary>
    public sealed class Settings : ExtendedCommandSettings
    {
        /// <summary>Gets or sets the packed template to validate.</summary>
        [CommandArgument(0, "<Path>")]
        [Description("Path to the template .nupkg, relative to the current working directory.")]
        public string PackagePath { get; set; } = "";

        /// <summary>Gets or sets optional framework filters; omission validates both variants.</summary>
        [CommandOption("--framework <TFM>")]
        [Description("net10.0 or net11.0. Repeat to select multiple frameworks; default: both.")]
        public string[] Framework { get; set; } = [];

        /// <summary>Gets the validated absolute package path.</summary>
        public AbsolutePath PackageAbsolute { get; private set; }

        /// <summary>Gets the deduplicated variants to build.</summary>
        public string[] SelectedFrameworks { get; private set; } = [];

        /// <summary>Rejects missing packages and unsupported frameworks before creating temporary output.</summary>
        protected override ValidationResult DoValidate()
        {
            if (string.IsNullOrWhiteSpace(PackagePath))
                return ValidationResult.Error("--package-path is required.");
            PackageAbsolute = TryGetFile(PackagePath, true, new AbsolutePath(Environment.CurrentDirectory));
            SelectedFrameworks = Framework.Length == 0
                ? ["net10.0", "net11.0"]
                : Framework.Distinct(StringComparer.Ordinal).ToArray();
            if (SelectedFrameworks.Any(target => target is not ("net10.0" or "net11.0")))
                return ValidationResult.Error("--framework must be net10.0 or net11.0.");
            return base.DoValidate();
        }
    }
}
