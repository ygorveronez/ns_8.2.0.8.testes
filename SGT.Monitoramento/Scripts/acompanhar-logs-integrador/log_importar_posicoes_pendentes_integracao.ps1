$date = (Get-Date).tostring("yyyy-MM-dd")
$Host.UI.RawUI.WindowTitle = "Integrador Tracking: Importar Posições Pendentes Integração $date"
$filename = "..\..\LogTracking\ImportarPosicoesPendentesIntegracao-$date.txt"
Write-Host $filename
cat $filename -Tail 10 -Wait