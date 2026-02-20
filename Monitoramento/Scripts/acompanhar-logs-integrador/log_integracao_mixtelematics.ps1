$date = (Get-Date).tostring("yyyy-MM-dd") 
$Host.UI.RawUI.WindowTitle = "Integrador Tracking: Integração MixTelematics $date"
$filename = "..\..\LogTracking\IntegracaoMixTelematics-$date.txt"
Write-Host $filename
cat $filename -Tail 10 -Wait