using System.Diagnostics;
using System.Net.NetworkInformation;
using ForgeDeploy.Core.Processes;

namespace ForgeDeploy.Infrastructure.Processes;

public sealed class ProcessRunner : IProcessRunner
{
    public async Task<int> RunAsync(
        string filename,
        IReadOnlyList<string> arguments,
        string? workingDirectory,
        Action<String> onOutput,
        CancellationToken cancellationToken = default
    ){
        var startInfo = new ProcessStartInfo
        {
            FileName = filename,
            UseShellExecute = false,
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            CreateNoWindow = true
        };

        if (!string.IsNullOrWhiteSpace(workingDirectory))
        {
            startInfo.WorkingDirectory = workingDirectory;
        }
        
        foreach(var argument in arguments)
        {
            startInfo.ArgumentList.Add(argument);
        }

        using var process = new Process
        {
            StartInfo = startInfo
        };

        process.OutputDataReceived += (_, eventArgs) =>
        {
          if(eventArgs.Data is not null)
            {
                onOutput(eventArgs.Data);
            }  
        };

        process.ErrorDataReceived += (_,eventArgs) =>
        {
          if(eventArgs.Data is not null)
            {
                onOutput(eventArgs.Data);
            }  
        };


        process.Start();

        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        await process.WaitForExitAsync(cancellationToken);

        return process.ExitCode;
    }
}