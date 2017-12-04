Write-Host "Cleaning Store Broker folder" -ForegroundColor Cyan
$installFolder = [System.IO.Path]::Combine($Home, "Documents\WindowsPowerShell\Modules\StoreBroker");
if (Test-Path -Path $installFolder)
{
    Remove-Item -Force -Recurse -Path $installFolder
}
New-Item -Type Directory -Force -Path $installFolder

Write-Host "Installing Store Broker" -ForegroundColor Cyan
git clone https://github.com/Microsoft/StoreBroker.git $installFolder
exit $LASTEXITCODE