$date = (Get-Date).tostring("yyyy-MM-dd") 
$Host.UI.RawUI.WindowTitle = "Integrador Tracking: Integração Positron $date"
$filename = "..\..\LogTracking\IntegracaoPositron-$date.txt"
Write-Host $filename
cat $filename -Tail 10 -Wait