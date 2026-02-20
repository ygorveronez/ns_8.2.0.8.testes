$date = (Get-Date).tostring("yyyy-MM-dd")
$Host.UI.RawUI.WindowTitle = "Integrador Tracking: Monitorar Threads $date"
$filename = "..\..\LogTracking\MonitorarThreads-$date.txt"
Write-Host $filename
cat $filename -Tail 10 -Wait