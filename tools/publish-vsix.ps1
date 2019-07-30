$vsixpublish = Get-ChildItem -File .\packages -recurse | 
    Where-Object { $_.Name -eq "VsixPublisher.exe" } | 
    Sort-Object -Descending -Property CreationTime | 
    Select-Object -First 1 -ExpandProperty FullName

. $vsixpublish login -publisherName MarekMagdziak -personalAccessToken $env:marketplacepublishtoken

$overview = (Get-Item .\Telia.GraphQL.Tooling/README.md).FullName
$manifest = (Get-Item .\Telia.GraphQL.Tooling/publish-manifest.json).FullName
$vsix = (Get-Item .\Telia.GraphQL.Tooling\bin\Release\Telia.GraphQL.Tooling.vsix).FullName
Write-Host "vsix: $vsix"
Write-Host "manifest: $manifest"
Write-Host "overview: $overview"

. $vsixpublish publish -payload "$vsix" -publishManifest "$manifest"