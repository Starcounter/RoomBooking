///
/// Private namespace
///
{
    ///
    /// Configuration from current script
    ///
    DirectoryPath rootDirectory = Context.GetCallerInfo().SourceFilePath.GetDirectory();
    string rootPath = rootDirectory.FullPath;
    string targetSubName = rootDirectory.GetDirectoryName();

    ///
    /// Argument parsing
    ///
    string configuration = Argument("configuration", "Debug");
    string starcounterBin = Argument("scBinForApps", "");
    string msBuildFullPath = Argument("msBuildFullPath", "");
    string msBuildPropertyAssemblyVersion = Argument("msBuildPropertyAssemblyVersion", "");
    string msBuildPropertyBuildCommonTargets = Argument("msBuildPropertyBuildCommonTargets", "");
    string starpackArtifactsPath = Argument("starpackArtifactsPath", "");
    string starNugetPath = Argument("starNugetPath", "");
    bool skipRunningM = Argument<bool>("skipRunningM", false);

    ///
    /// Dependent targets
    ///
    Task(string.Format("Build{0}", targetSubName))
        .IsDependentOn(string.Format("Restore{0}", targetSubName))
        .IsDependentOn(string.Format("Build{0}I", targetSubName));

    Task(string.Format("Run{0}", targetSubName))
        .IsDependentOn(string.Format("Restore{0}", targetSubName))
        .IsDependentOn(string.Format("Build{0}I", targetSubName))
        .IsDependentOn(string.Format("Run{0}I", targetSubName));

    Task(string.Format("Test{0}", targetSubName))
        .IsDependentOn(string.Format("Restore{0}", targetSubName))
        .IsDependentOn(string.Format("Build{0}I", targetSubName))
        .IsDependentOn(string.Format("Run{0}I", targetSubName))
        .IsDependentOn(string.Format("Test{0}I", targetSubName));

    Task(string.Format("Pack{0}", targetSubName))
        .IsDependentOn(string.Format("Restore{0}", targetSubName))
        .IsDependentOn(string.Format("Build{0}I", targetSubName))
        .IsDependentOn(string.Format("Run{0}I", targetSubName))
        .IsDependentOn(string.Format("Test{0}I", targetSubName))
        .IsDependentOn(string.Format("Pack{0}I", targetSubName));

    ///
    /// Independent restore target
    ///
    Task(string.Format("Restore{0}", targetSubName)).Does(() =>
    {
        var settings = new NuGetRestoreSettings
        {
            NoCache = true,
            EnvironmentVariables = GetEnvironmentVariables(),
            Verbosity = NuGetVerbosity.Normal
        };

        NuGetRestore($"{rootPath}/{targetSubName}.sln", settings);
    });

    ///
    /// Independent build target
    ///
    Task(string.Format("Build{0}I", targetSubName)).Does(() =>
    {
        var settings = new MSBuildSettings
        {
            Configuration = configuration,
            EnvironmentVariables = GetEnvironmentVariables(),
            Verbosity = Verbosity.Minimal,
            Restore = false,
            MSBuildPlatform = MSBuildPlatform.x64,
            PlatformTarget = PlatformTarget.x64
        };

        if (!string.IsNullOrEmpty(msBuildPropertyAssemblyVersion))
        {
            settings.WithProperty("VersionAssembly", msBuildPropertyAssemblyVersion);
        }

        if (!string.IsNullOrEmpty(msBuildPropertyBuildCommonTargets))
        {
            settings.WithProperty("BuildCommonTargets", msBuildPropertyBuildCommonTargets);
        }

        if (!string.IsNullOrEmpty(msBuildFullPath))
        {
            settings.ToolPath = msBuildFullPath;
        }

        MSBuild($"{rootPath}/{targetSubName}.sln", settings);
    });

    ///
    /// Independent run target
    ///
    Task(string.Format("Run{0}I", targetSubName)).Does(() =>
    {
        string cliShell = "cmd";
        string cliArgs;
        ProcessSettings processSettings = new ProcessSettings
        {
            EnvironmentVariables = GetEnvironmentVariables(),
            WorkingDirectory = rootPath
        };

        if (!skipRunningM)
        {
            string mExecutableFullPath = $"{rootPath}/src/{targetSubName}.Mapper/bin/{configuration}/M.exe";

            if (!FileExists(mExecutableFullPath))
            {
                throw new Exception($"M.exe is missing at {mExecutableFullPath}. Build the solution first");
            }

            processSettings.Arguments = new ProcessArgumentBuilder()
                .Append("/c")
                .Append($"staradmin.exe list apps | findstr /b /c:\"M (in default)\" || star.exe \"{mExecutableFullPath}\"");

            if(StartProcess(cliShell, processSettings) != 0)
            {
                throw new Exception(string.Format("Error while running: {0} {1}", cliShell, processSettings.Arguments.Render()));
            }
        }

        cliArgs = string.Format("/c star.exe --resourcedir=\"{0}/src/{1}/wwwroot\" \"{0}/src/{1}/bin/{2}/{1}.exe\"", 
            rootPath, targetSubName, configuration);
        processSettings.Arguments = new ProcessArgumentBuilder().Append(cliArgs);

        if(StartProcess(cliShell, processSettings) != 0)
        {
            throw new Exception(string.Format("Error while running: {0} {1}", cliShell, cliArgs));
        }
    });

    ///
    /// Independent test target
    ///
    Task(string.Format("Test{0}I", targetSubName)).Does(() =>
    {
        Warning("There is no test");
    });

    ///
    /// Independent pack target
    ///
    Task(string.Format("Pack{0}I", targetSubName)).Does(() =>
    {
        string cliShell = "cmd";
        DirectoryPath starpackOutputDir = string.IsNullOrEmpty(starpackArtifactsPath) ? 
            Directory($"{rootPath}/src/{targetSubName}") : MakeAbsolute(Directory(starpackArtifactsPath));

        if (!DirectoryExists(starpackOutputDir))
        {
            CreateDirectory(starpackOutputDir);
        }

        var cliArgs = new ProcessArgumentBuilder()
            .Append("/c starpack.exe")
            .Append($"--pack \"{rootPath}/src/{targetSubName}/{targetSubName}.csproj\"")
            .Append($"--config={configuration}")
            .Append($"--output=\"{starpackOutputDir.FullPath}\"");

        var processSettings = new ProcessSettings
        {
            Arguments = cliArgs,
            EnvironmentVariables = GetEnvironmentVariables(),
            WorkingDirectory = rootPath
        };

        int exitCode = StartProcess(cliShell, processSettings);

        if(exitCode != 0)
        {
            throw new Exception(string.Format("Error while packaging: {0} {1}", cliShell, cliArgs.Render()));
        }
    });

    ///
    /// Utilities
    ///
    Dictionary<string, string> GetEnvironmentVariables()
    {
        Dictionary<string, string> envVars = new Dictionary<string, string>();

        envVars.Add("config", configuration);
        envVars.Add("configuration", configuration);

        if (IsRunningOnWindows())
        {
            if (!string.IsNullOrEmpty(starcounterBin))
            {
                string absoluteScBin = MakeAbsolute(Directory(starcounterBin)).FullPath;
                envVars.Add("StarcounterBin", absoluteScBin);
                envVars.Add("Path", $"{absoluteScBin};{absoluteScBin}/StarDump;{absoluteScBin}/StarPack;{Environment.GetEnvironmentVariable("Path")}");
                envVars.Add("ReferencePath", $"{absoluteScBin};{Environment.GetEnvironmentVariable("ReferencePath")}");
            }

            if (!string.IsNullOrEmpty(starNugetPath))
            {
                envVars.Add("STAR_NUGET", MakeAbsolute(Directory(starNugetPath)).FullPath);
            }
        }
        else if (IsRunningOnUnix())
        {
            throw new Exception("Unix support has not yet been implemented.");
        }
        else
        {
            throw new Exception("Only Unix and Windows platforms are supported.");
        }

        return envVars;
    }

    ///
    /// Run targets if invoked as self-containment script
    ///
    if (!Tasks.Any(t => t.Name.Equals("Bifrost")))
    {
        // Read targets arguments
        IEnumerable<string> targetsArg = Argument("targets", string.Format("Pack{0}", targetSubName)).Split(new Char[]{',', ' '}).Where(s => !string.IsNullOrEmpty(s));

        // Self-containment dependent targets
        Task("Restore").IsDependentOn(string.Format("Restore{0}", targetSubName));
        Task("Build").IsDependentOn(string.Format("Build{0}", targetSubName));
        Task("Run").IsDependentOn(string.Format("Run{0}", targetSubName));
        Task("Test").IsDependentOn(string.Format("Test{0}", targetSubName));
        Task("Pack").IsDependentOn(string.Format("Pack{0}", targetSubName));

        // Run target
        foreach (string t in targetsArg)
        {
            RunTarget(t);
        }
    }
}