$date = (Get-Date).tostring("yyyy-MM-dd")
$Host.UI.RawUI.WindowTitle = "Integrador Tracking: Monitorar Posições $date"
$filename = "..\..\LogTracking\MonitorarPosicoes-$date.txt"
Write-Host $filename
cat $filename -Tail 10 -Wait