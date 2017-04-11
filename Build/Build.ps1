$projectPath = $env:APPVEYOR_BUILD_FOLDER + "\Converter\Converter.csproj"
$destination = $env:APPVEYOR_BUILD_FOLDER + "\Build\AppxPackages"

"nuget restore"
nuget restore
"msbuild"
msbuild $projectPath /verbosity:minimal /logger:"C:\Program Files\AppVeyor\BuildAgent\Appveyor.MSBuildLogger.dll" /p:AppxBundlePlatforms="x86|x64|ARM" /p:AppxPackageDir=$destination /p:AppxBundle=Always /p:UapAppxPackageBuildMode=StoreUpload /p:configuration="release" /p:VisualStudioVersion="14.0"