namespace ForgeDeploy.Core.Processes;

public interface IProcessRunner
{
    Task<int> RunAsync(
        string filename,
        IReadOnlyList<string> arguments,
        string? workingDirectory,
        Action<string> onOutput,
        CancellationToken cancellationToken=default
    );

}    