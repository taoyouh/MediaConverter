$tenantId = "05fd4458-757f-4d64-8c5d-ca5617532083"
$clientId = "07e52762-7779-4184-8d85-b2f8ebd64d5d"
$clientSecret = $env:clientSecret | ConvertTo-SecureString -AsPlainText -Force

$appId = "9NDJV6K3G2TJ"
$outName = "Submission"

$configPath = $env:rootPath + "Publish\StoreBroker\SBConfig.json"
$pdpRootPath = $env:rootPath + "Publish\StoreBroker\Pdp\"
$imageRootPath = $env:rootPath + "PublishStoreBroker\Images\"
$appxPath = $env:rootPath + "Build\AppxPackages\"

$outPath = $env:rootPath + "\Publish\SubmissionPackage"

"Initializing submission package path"
if (Test-Path -Path $outPath)
{
    Remove-Item -Force -Recurse -Path $outPath
}
New-Item -Type Directory -Force -Path $outPath

"Logging in to Dev Center"
$cred = New-Object System.Management.Automation.PSCredential $clientId, $clientSecret
Set-StoreBrokerAuthentication -TenantId $tenantId -Credential $cred

"Looking for appxupload at " + $appxPath
$appxupload = (Get-ChildItem -Path $appxPath | Where-Object Name -like "*.appxupload")[0].FullName

"Creating submission package:"
New-SubmissionPackage -ConfigPath $configPath -PDPRootPath $pdpRootPath -ImagesRootPath $imageRootPath -OutPath $outPath -OutName $outName -AppxPath $appxupload

"Submitting package to Dev Center"
Update-ApplicationSubmission -AppId $appId -SubmissionDataPath ($outPath + $outName + ".json") -PackagePath ($outPath + $outName + ".zip") -Force -AddPackages -UpdateListings

"Clearing Authentication"
Clear-StoreBrokerAuthentication