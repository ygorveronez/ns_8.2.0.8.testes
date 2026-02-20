$date = (Get-Date).tostring("yyyy-MM-dd") 
$Host.UI.RawUI.WindowTitle = "Integrador Tracking: Integração Ravex $date"
$filename = "..\..\LogTracking\IntegracaoRavex-$date.txt"
Write-Host $filename
cat $filename -Tail 10 -Wait