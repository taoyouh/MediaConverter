$projectPath = [System.IO.Path]::Combine($env:APPVEYOR_BUILD_FOLDER, "Converter\Converter.csproj")
$destination = [System.IO.Path]::Combine($env:APPVEYOR_BUILD_FOLDER, "Build\AppxPackages")

"nuget restore"
msbuild $projectPath /t:restore /verbosity:minimal /logger:"C:\Program Files\AppVeyor\BuildAgent\Appveyor.MSBuildLogger.dll"
if ($LASTEXITCODE -ne 0)
{
    exit $LASTEXITCODE
}

"msbuild"
if ($env:APPVEYOR_REPO_BRANCH -eq "master")
{
    msbuild $projectPath /verbosity:minimal /logger:"C:\Program Files\AppVeyor\BuildAgent\Appveyor.MSBuildLogger.dll" /p:AppxBundlePlatforms="x86|x64|ARM" /p:AppxPackageDir=$destination /p:AppxBundle=Always /p:UapAppxPackageBuildMode=StoreUpload /p:configuration="release"
}
else
{
    msbuild $projectPath /verbosity:minimal /logger:"C:\Program Files\AppVeyor\BuildAgent\Appveyor.MSBuildLogger.dll" /p:AppxBundlePlatforms="x86" /p:AppxPackageDir=$destination /p:AppxBundle=Always /p:UapAppxPackageBuildMode=StoreUpload /p:configuration="release" /p:platform="x86"
}
exit $LASTEXITCODE