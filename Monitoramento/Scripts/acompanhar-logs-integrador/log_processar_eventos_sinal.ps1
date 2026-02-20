$date = (Get-Date).tostring("yyyy-MM-dd")
$Host.UI.RawUI.WindowTitle = "Integrador Tracking: Processar Eventos Sinal $date"
$filename = "..\..\LogTracking\ProcessarEventosSinal-$date.txt"
Write-Host $filename
cat $filename -Tail 10 -Wait