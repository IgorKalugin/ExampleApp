#addin "Cake.AndroidAppManifest"
#addin "Cake.FileHelpers"
#addin "Cake.Git"
#addin "Cake.Npm"
#addin "Cake.Plist"
#addin "Cake.Slack"
#tool "nuget:?package=GitVersion.CommandLine"

// --------------------------------------------------------------------------------
// ARGUMENTS
// --------------------------------------------------------------------------------

var target = Argument("target", "Default");
// var configuration = Argument("configuration", "Release");

GitVersion versionInfo;
int buildNumber;

// --------------------------------------------------------------------------------
// PREPARATION
// --------------------------------------------------------------------------------

var branchInfo = GitBranchCurrent(Directory("."));
var branchName = branchInfo.FriendlyName;
Information($"Branch name: {branchName}");

var isBetaDeploy = branchName.ToLower() == "develop" || branchName.ToLower().StartsWith("hotfix");
var isStoreDeploy = false; // branchName.ToLower() == "master";
Information($"Beta deploy: {isBetaDeploy}");
Information($"Store deploy: {isStoreDeploy}");

var reportMessage = "";
var artifactsFolder = "./artifacts";

TaskSetup(setupContext =>
{
   if(TeamCity.IsRunningOnTeamCity)
   {
      TeamCity.WriteStartBuildBlock(setupContext.Task.Description ?? setupContext.Task.Name);
      TeamCity.WriteStartProgress(setupContext.Task.Description ?? setupContext.Task.Name);
   }
});

TaskTeardown(teardownContext =>
{
   if(TeamCity.IsRunningOnTeamCity)
   {
      TeamCity.WriteEndProgress(teardownContext.Task.Description ?? teardownContext.Task.Name);
      TeamCity.WriteEndBuildBlock(teardownContext.Task.Description ?? teardownContext.Task.Name);
   }
});

Task("RestoreNuGet")
    .Does(() =>
{
    var solutionFile = File("src/Dose.sln");
    NuGetRestore(solutionFile);
});

Task("NpmInstall")
    .Does(() =>
{
    NpmInstall();
});

Task("UpdateVersion")
    .Does(() => 
{
    // update CommonAssemblyInfo.cs using GitVersion.yml config
    versionInfo = GitVersion(new GitVersionSettings 
    {
        UpdateAssemblyInfo = true,
        UpdateAssemblyInfoFilePath = "src/CommonAssemblyInfo.cs",
        // OutputType = GitVersionOutput.BuildServer
    });

    buildNumber = int.Parse(GetEnvironmentVariable("BUILD_NUMBER"));       // use TeamCity build number

    if(TeamCity.IsRunningOnTeamCity)
    {
        TeamCity.SetBuildNumber(versionInfo.MajorMinorPatch + "_{build.number}");
    }

    Information($"version: {versionInfo.MajorMinorPatch}, semVersion: {versionInfo.SemVer}, build number: {buildNumber}");
});

Task("CreateReleaseNotes")
    .IsDependentOn("UpdateVersion")
    .IsDependentOn("NpmInstall")
    .Does(() =>
{
    // update version of the app in package.json, because create-release-notes script depends on it
    NpmRunScript("update-version", new List<string>{ $"{versionInfo.MajorMinorPatch}" });
    NpmRunScript("create-release-notes");

    // for android we need to place changelog file to metadata folder with name equal to version code
    // android release notes limitation is 500 symbols
    var androidReleaseNotesFolder = "fastlane/metadata/android/en-US/changelogs/";
    EnsureDirectoryExists(androidReleaseNotesFolder);
    var releaseNotes = FileReadText("./CHANGELOG.md");
    var truncatedReleaseNotes = releaseNotes.Substring(0, Math.Min(500, releaseNotes.Length));;
    FileWriteText($"{androidReleaseNotesFolder}{buildNumber}.txt", truncatedReleaseNotes);

    // for ios we need to place changelog file to metadata folder using specific file name
    var iosReleaseNotesFile = "fastlane/metadata/ios/en-US/release_notes.txt";
    CopyFile("./CHANGELOG.md", iosReleaseNotesFile);
});

Task("CleanArtifactsFolder")
    .Does(() =>
{
    EnsureDirectoryExists(artifactsFolder);
    CleanDirectory(artifactsFolder);
});

private string GetEnvironmentVariable(string key) 
{
    var value = EnvironmentVariable(key);
    if (string.IsNullOrEmpty(value))
    {
        throw new Exception($"The {key} environment variable is not defined.");
    }

    return value;
}

private string StartProcessWithResult(string process, string arguments, bool resultFromOutput = false)
{
    IEnumerable<string> redirectedStandardOutput;
    var result = StartProcess(
                    process, 
                    new ProcessSettings{ Arguments = arguments, RedirectStandardOutput = resultFromOutput }, 
                    out redirectedStandardOutput);
    if (result != 0) 
    {
        throw new Exception($"Error in process: {process} {arguments}");
    }

    return resultFromOutput ? string.Concat(redirectedStandardOutput) : string.Empty;
}

// --------------------------------------------------------------------------------
// ANDROID
// --------------------------------------------------------------------------------

Task("CleanAndroid")
    .Does(() =>
{
    var androidBin = Directory("src/Dose.Android/bin/Release");
    CleanDirectory(androidBin);
});

Task("UpdateAndroidManifest")
    .IsDependentOn("UpdateVersion")
    .Does (() =>
{
    var manifestFile = File("src/Dose.Android/Properties/AndroidManifest.xml");
    var manifest = DeserializeAppManifest(manifestFile);
    manifest.VersionName = versionInfo.MajorMinorPatch;
    manifest.VersionCode = buildNumber;
    SerializeAppManifest(manifestFile, manifest);
});

Task("BuildAndroid")
    .IsDependentOn("RestoreNuGet")
    .IsDependentOn("CleanAndroid")
    .IsDependentOn("UpdateAndroidManifest")
    .Does(() =>
{
    var keyStore = MakeAbsolute(File("./dose.keystore")).FullPath;

    var keyStoreAlias = GetEnvironmentVariable("ANDROID_KEYSTORE_ALIAS");
    var keyStorePassword = GetEnvironmentVariable("ANDROID_KEYSTORE_PASSWORD");

    var androidProject = File("src/Dose.Android/Dose.Android.csproj");
 	MSBuild(androidProject, settings =>
        settings.SetConfiguration("Release")
        .WithTarget("SignAndroidPackage")
            .WithProperty("AndroidKeyStore", "true")
            .WithProperty("AndroidSigningStorePass", keyStorePassword)
            .WithProperty("AndroidSigningKeyStore", keyStore)
            .WithProperty("AndroidSigningKeyAlias", keyStoreAlias)
            .WithProperty("AndroidSigningKeyPass", keyStorePassword)
            .WithProperty("DebugSymbols", "false")
            .WithProperty("OutputPath", "bin/Release/"));

    CopyFileToDirectory("src/Dose.Android/bin/Release/com.daralabs.dose.application-Signed.apk", artifactsFolder);
});

Task("DeployAndroidBeta")
    .WithCriteria(isBetaDeploy)
    .IsDependentOn("NpmInstall")
    .Does(() => 
{
    // var appCenterToken = GetEnvironmentVariable("APP_CENTER_TOKEN");
    // NpmRunScript("appcenter-distribute-android", new List<string> { $"--token {appCenterToken}" });
    if(TeamCity.IsRunningOnTeamCity)
    {
        TeamCity.WriteProgressMessage("Uploading to GooglePlay");
    }

    StartProcessWithResult("fastlane", "googleplay_alpha_deploy");
    reportMessage += $"Beta *Dose android* application version *{versionInfo.MajorMinorPatch}* is available at *Google Play* and *AppCenter*\n";
});

Task("DeployAndroidStore")
    .WithCriteria(isStoreDeploy)
    .Does(() => 
{
    if(TeamCity.IsRunningOnTeamCity)
    {
        TeamCity.WriteProgressMessage("Uploading to GooglePlay");
    }

    StartProcessWithResult("fastlane", "googleplay_production_deploy");
    reportMessage += $"New *Dose android* application version *{versionInfo.MajorMinorPatch}* is available at *Google Play*\n";
});

// --------------------------------------------------------------------------------
// IOS
// --------------------------------------------------------------------------------

Task("CleanIOS")
    .Does(() =>
{
    var iOSBin = Directory("src/Dose.iOS/bin/iPhone/AppStore");
    CleanDirectory(iOSBin);
});

Task("UpdateIOSPlist")
    .IsDependentOn("UpdateVersion")
    .Does (() =>
{
    var plistFile = File("./src/Dose.iOS/Info.plist");
    dynamic plist = DeserializePlist(plistFile);
    plist["CFBundleShortVersionString"] = versionInfo.MajorMinorPatch;
    plist["CFBundleVersion"] = buildNumber.ToString();
    SerializePlist(plistFile, plist);
});

Task("UpdateIOSCertificates")
    .Does(() =>
{
    StartProcessWithResult("fastlane", "ios_certs");
});

Task("BuildIOS")
    .IsDependentOn("RestoreNuGet")
    .IsDependentOn("CleanIOS")
    .IsDependentOn("UpdateIOSPlist")
    .IsDependentOn("UpdateIOSCertificates")
    .WithCriteria(IsRunningOnUnix())
    .Does(() =>
{
    var iOSProject = File("src/Dose.iOS/Dose.iOS.csproj");
    MSBuild(iOSProject, settings => 
	    settings.SetConfiguration("AppStore")
    		.WithProperty("Platform", "iPhone")
    		.WithProperty("OutputPath", $"bin/iPhone/AppStore/"));
    
    CopyFileToDirectory("src/Dose.iOS/bin/iPhone/AppStore/DoseiOS.ipa", artifactsFolder);

    var artifactsDsymFolder = $"{artifactsFolder}/DoseiOS.app.dSYM";
    EnsureDirectoryExists($"{artifactsFolder}/DoseiOS.app.dSYM");
    CopyDirectory("src/Dose.iOS/bin/iPhone/AppStore/DoseiOS.app.dSYM", artifactsDsymFolder);
});

Task("DeployIOSBeta")
    .WithCriteria(isBetaDeploy)
    .IsDependentOn("NpmInstall")
    .Does(() => 
{
    // if(TeamCity.IsRunningOnTeamCity)
    // {
    //     TeamCity.WriteProgressMessage("Uploading to AppCenter");
    // }
    // var appCenterToken = GetEnvironmentVariable("APP_CENTER_TOKEN");
    // NpmRunScript("appcenter-distribute-ios", new List<string> { $"--token {appCenterToken}" });

    if(TeamCity.IsRunningOnTeamCity)
    {
        TeamCity.WriteProgressMessage("Uploading to TestFlight");
    }

    StartProcessWithResult("fastlane", "testflight_deploy");
    reportMessage += $"Beta *Dose iOS* application version *{versionInfo.MajorMinorPatch}* is available at *TestFlight* and *AppCenter*\n";
});

Task("DeployIOSStore")
    .WithCriteria(isStoreDeploy)
    .Does(() => 
{
    if(TeamCity.IsRunningOnTeamCity)
    {
        TeamCity.WriteProgressMessage("Uploading to AppStore");
    }
    
    StartProcessWithResult("fastlane", "appstore_deploy");
    reportMessage += $"New *Dose iOS* application version *{versionInfo.MajorMinorPatch}* is available at *AppStore*\n";
});

// --------------------------------------------------------------------------------
// AFTER BUILD
// --------------------------------------------------------------------------------

Task("GitTagVersion")
    .WithCriteria(isStoreDeploy)
    .IsDependentOn("UpdateVersion")
    .Does(() => 
{
    var username = GetEnvironmentVariable("GIT_USERNAME");
    var password = GetEnvironmentVariable("GIT_PASSWORD");

    var solutionFolder = "./";
    GitTag(solutionFolder, versionInfo.MajorMinorPatch);
    GitPushRef(solutionFolder, username, password, "origin", versionInfo.MajorMinorPatch);
});

Task("SendReportMessageToSlack")
    .WithCriteria(!string.IsNullOrWhiteSpace(reportMessage))
    .Does(() =>
{
    var slackToken = GetEnvironmentVariable("SLACK_TOKEN");
    var slackChannel = GetEnvironmentVariable("SLACK_CHANNEL");

    var postMessageResult = Slack.Chat.PostMessage(
                token: slackToken,
                channel: slackChannel,
                text: reportMessage
        );

    if (!postMessageResult.Ok)
    {
        Error("Failed to send message: {0}", postMessageResult.Error);
    }
});

Task("PublishArtifacts")
    .WithCriteria(TeamCity.IsRunningOnTeamCity)
    .Does(() => 
{
    TeamCity.PublishArtifacts(artifactsFolder);
});

// --------------------------------------------------------------------------------
// Default
// --------------------------------------------------------------------------------

Task("Default")
    .IsDependentOn("CleanArtifactsFolder")
    .IsDependentOn("RestoreNuGet")
    .IsDependentOn("UpdateVersion")
    .IsDependentOn("CreateReleaseNotes")
    .IsDependentOn("BuildAndroid")
    .IsDependentOn("BuildIOS")
    .IsDependentOn("DeployAndroidBeta")
    .IsDependentOn("DeployAndroidStore")
    .IsDependentOn("DeployIOSBeta")
    .IsDependentOn("DeployIOSStore")
    .IsDependentOn("GitTagVersion")
    .IsDependentOn("SendReportMessageToSlack")
    .IsDependentOn("PublishArtifacts")
    .Does(() =>
{
});

RunTarget(target);