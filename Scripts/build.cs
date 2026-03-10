#:package FileBasedApp.Toolkit@0.14.0
#:property PublishAot=false 

using Spectre.Console.Cli;
using TruePath;
using Spectre.Console;
using FileBasedApp.Toolkit;
using System.IO.Abstractions;
using TruePath.TestableIO.System.IO;
using static SimpleExec.Command;

var commandApp = new CommandApp<RunCommand>();
commandApp.Configure(config => config.PropagateExceptions());
await commandApp.RunAsync(args);

public class RunCommand : AsyncCommand<RunCommand.Settings> // For sync only you can use Command (and have Execute instead of ExecuteAsync
{
	public override async Task<int> ExecuteAsync(CommandContext context, Settings settings, CancellationToken cancellationToken)
	{
		AnsiConsole.MarkupLineInterpolated($"[green]{settings.RootPath.Value}[/]");
		
		var nuspec = settings.RootPath.GetFiles("*.nuspec", SearchOption.AllDirectories) is {Length:>= 1} match ? match[0] : throw new InvalidOperationException($"Multiple or none found for nuspec");
		var slnx = settings.RootPath.GetFiles("*.slnx", SearchOption.AllDirectories).Single();
		
		var artifact = settings.RootPath / "artifacts";

		try
		{
			artifact.DirectoryDelete(true);
		}
		catch
		{
			// ignored
		}

		try
		{
			artifact.CreateDirectory();
			var releaseFolder = artifact / "Release";
			var nugetFolder = artifact / "NuGet";
			
			// List<string> arguments = ["build", slnx.Value, "-c", "Release", "-o", releaseFolder.Value];
			// await RunAsync("dotnet", arguments, ct: cancellationToken);

			List<string> nugetParameters = ["pack", nuspec.Value, "-OutputDirectory", nugetFolder.Value];
			if (!string.IsNullOrWhiteSpace(settings.Version))
			{
				nugetParameters.AddRange("-Version", settings.Version);
			}

			if (!string.IsNullOrWhiteSpace(settings.VersionSuffix))
			{
				nugetParameters.AddRange("-Suffix", settings.VersionSuffix);
			}
			await RunAsync("nuget", nugetParameters, ct: cancellationToken);
			
			foreach (var absolutePath in nugetFolder.EnumerateFiles("*.nupkg"))
			{
				AnsiConsole.MarkupLineInterpolated($"[dim]Pushing [bold]{absolutePath.RelativeTo(settings.RootPath)}[/][/]");

				List<string> pushParameters = ["nuget", "push", absolutePath.Value, "--skip-duplicate"];

				if (!string.IsNullOrWhiteSpace(settings.ApiKey))
				{
					pushParameters.AddRange("--api-key", settings.ApiKey);
					pushParameters.AddRange("--source", "nuget.org");
				}
				else
				{
					pushParameters.AddRange("--source", "Local");
				}
				
				await RunAsync("dotnet", pushParameters, ct: cancellationToken);
			}
		}
		finally
		{
			try
			{
				artifact.DirectoryDelete(true);
			}
			catch
			{
				// ignored
			}
		}
		
		//await static.RunAsync("dotnet", ["--version"], parentDirectory.Value, ct: cancellationToken);
		return 0; // 0 for success
	}

	public class Settings : ExtendedCommandSettings
	{
		[CommandOption("--api-key")]
		public string? ApiKey { get; set; }

		public AbsolutePath RootPath { get; private set; }
		[CommandOption("--version")]
		public string? Version { get; set; }
		
		[CommandOption("--version-suffix")]
		public string? VersionSuffix { get; set; }

		protected override ValidationResult DoValidate()
		{
			RootPath = TryGetDirectory("..", false, true, PredefinedRootPath.ExecutionFolder);
			
			return base.DoValidate();
		}
	} 
		
	
}