$date = (Get-Date).tostring("yyyy-MM-dd")
$Host.UI.RawUI.WindowTitle = "Integrador Tracking: Processar Trocas de Alvo $date"
$filename = "..\..\LogTracking\ProcessarTrocaAlvo-$date.txt"
Write-Host $filename
cat $filename -Tail 10 -Wait