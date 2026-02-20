$date = (Get-Date).tostring("yyyy-MM-dd") 
$Host.UI.RawUI.WindowTitle = "Integrador Tracking: Integração Raster $date"
$filename = "..\..\LogTracking\IntegracaoRaster-$date.txt"
Write-Host $filename
cat $filename -Tail 10 -Wait