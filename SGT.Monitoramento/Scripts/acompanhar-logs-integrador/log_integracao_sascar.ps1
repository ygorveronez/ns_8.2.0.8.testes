$date = (Get-Date).tostring("yyyy-MM-dd") 
$Host.UI.RawUI.WindowTitle = "Integrador Tracking: Integração Sascar $date"
$filename = "..\..\LogTracking\IntegracaoSascar-$date.txt"
Write-Host $filename
cat $filename -Tail 10 -Wait