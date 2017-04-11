# 需要的环境变量：
# $env:rootPath：项目根目录地址
# $env:clientSecret：Azure AD的Client Secret

# 从Build\AppxPackages中获取appxupload格式安装包
# 从Publish\StoreBroker\Pdb中获取商店描述信息
# 自动替换已有的包和商店描述信息，并自动提交

$tenantId = "05fd4458-757f-4d64-8c5d-ca5617532083"
$clientId = "07e52762-7779-4184-8d85-b2f8ebd64d5d"
$clientSecret = $env:clientSecret | ConvertTo-SecureString -AsPlainText -Force

$appId = "9NDJV6K3G2TJ"
$outName = "Submission"

$configPath = [System.IO.Path]::Combine($env:rootPath, "Publish\StoreBroker\SBConfig.json")
$pdpRootPath = [System.IO.Path]::Combine($env:rootPath, "Publish\StoreBroker\Pdp\")
$imageRootPath = [System.IO.Path]::Combine($env:rootPath, "Publish\StoreBroker\Images\")
$appxPath = [System.IO.Path]::Combine($env:rootPath, "Build\AppxPackages\")

$outPath = [System.IO.Path]::Combine($env:rootPath, "Publish\SubmissionPackage\")

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
$appxuploads = (Get-ChildItem -Path $appxPath | Where-Object Name -like "*.appxupload")

"Creating submission package:"
New-SubmissionPackage -ConfigPath $configPath -PDPRootPath $pdpRootPath -ImagesRootPath $imageRootPath -OutPath $outPath -OutName $outName -AppxPath $appxuploads.FullName

"Submitting package to Dev Center"
Update-ApplicationSubmission -AppId $appId -SubmissionDataPath ($outPath + $outName + ".json") -PackagePath ($outPath + $outName + ".zip") -Force -ReplacePackages -UpdateListings -TargetPublishMode Manual -AutoCommit

"Clearing Authentication"
Clear-StoreBrokerAuthentication