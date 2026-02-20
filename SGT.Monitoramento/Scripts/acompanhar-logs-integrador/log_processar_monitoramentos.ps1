$date = (Get-Date).tostring("yyyy-MM-dd")
$Host.UI.RawUI.WindowTitle = "Integrador Tracking: Processar Monitoramentos $date"
$filename = "..\..\LogTracking\ProcessarMonitoramentos-$date.txt"
Write-Host $filename
cat $filename -Tail 10 -Wait