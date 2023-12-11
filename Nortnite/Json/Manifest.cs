namespace Nortnite.Json;

public class Manifest {
    public required string LaunchCommand { get; init; }
    public required string LaunchExecutable { get; init; }
    public required string InstallLocation { get; init; }
    public required string AppName { get; init; }
}
