Param(
  [string]$newVersion
)

$file = "$PSScriptRoot\..\Telia.GraphQL.Tooling\source.extension.vsixmanifest"

[xml]$xml = get-content -Path $file

if ($newVersion -eq "") {
    $version = [System.Version]::Parse($xml.PackageManifest.Metadata.Identity.Version)

    $newVersion = "$($version.Major).$($version.Minor).$($version.Build + 1)"
} else {
    $newVersion = $newVersion -replace "(\d*\.\d*\.\d*).*", "`$1"
}

$xml.PackageManifest.Metadata.Identity.Version = $newVersion

$xml.Save($file)

"Updated $file to $newVersion "