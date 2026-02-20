$date = (Get-Date).tostring("yyyy-MM-dd") 
$Host.UI.RawUI.WindowTitle = "Integrador Tracking: Integração Opentech $date"
$filename = "..\..\LogTracking\IntegracaoOpentech-$date.txt"
Write-Host $filename
cat $filename -Tail 10 -Wait