"Cleaning..."
$installFolder = $Home + "\Documents\WindowsPowerShell\Modules\StoreBroker"
Remove-Item -Path $installFolder
New-Item -Type Directory -Force -Path $installFolder

"Cloning Store Broker"
git clone https://github.com/Microsoft/StoreBroker.git $installFolder