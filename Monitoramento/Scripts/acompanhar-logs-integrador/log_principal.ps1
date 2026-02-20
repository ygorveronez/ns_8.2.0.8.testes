$date = (Get-Date).tostring("d-M-yyyy")
$Host.UI.RawUI.WindowTitle = "Integrador Tracking: Log principal $date"
$filename = "..\..\Log\$date.txt"
Write-Host $filename
cat $filename -Tail 10 -Wait