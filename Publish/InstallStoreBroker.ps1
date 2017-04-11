"Cleaning..."
$installFolder = [System.IO.Path]::Combine($Home, "Documents\WindowsPowerShell\Modules\StoreBroker");
if (Test-Path -Path $installFolder)
{
    Remove-Item -Force -Recurse -Path $installFolder
}
New-Item -Type Directory -Force -Path $installFolder

"Cloning Store Broker"
git clone https://github.com/Microsoft/StoreBroker.git $installFolder