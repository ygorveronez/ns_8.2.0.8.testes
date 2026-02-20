$date = (Get-Date).tostring("yyyy-MM-dd")
$Host.UI.RawUI.WindowTitle = "Integrador Tracking: Controle Monitoramento $date"
$filename = "..\..\LogTracking\ControleMonitoramento-$date.txt"
Write-Host $filename
cat $filename -Tail 20 -Wait