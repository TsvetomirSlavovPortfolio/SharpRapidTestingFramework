
$from = 'C:\Users\bezrukov\Desktop\Testing'
$to = 'C:\Users\bezrukov\Desktop\min'


$excludeMatch = @("bin", "obj", ".vs", "TestResults", ".gitignore", "packages")
[regex] $excludeMatchRegEx = ‘(?i)‘ + (($excludeMatch |foreach {[regex]::escape($_)}) –join “|”) + ‘’
Get-ChildItem -Path $from -Recurse -Exclude $exclude | 
 where { $excludeMatch -eq $null -or $_.FullName.Replace($from, "") -notmatch $excludeMatchRegEx} |
 Copy-Item -Destination {
  if ($_.PSIsContainer) {
   Join-Path $to $_.Parent.FullName.Substring($from.length)
  } else {
   Join-Path $to $_.FullName.Substring($from.length)
  }
 } -Force -Exclude $exclude