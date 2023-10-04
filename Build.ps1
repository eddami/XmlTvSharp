function Exec
{
    [CmdletBinding()]
    param(
        [Parameter(Position=0,Mandatory=1)][scriptblock]$cmd,
        [Parameter(Position=1,Mandatory=0)][string]$errorMessage = ($msgs.error_bad_command -f $cmd)
    )
    & $cmd
    if ($lastexitcode -ne 0) {
        throw ("Exec: " + $errorMessage)
    }
}

$artifacts = ".\artifacts"

if(Test-Path $artifacts) { Remove-Item $artifacts -Force -Recurse }

exec { & dotnet clean -c Release }

exec { & dotnet build -c Release }

exec { & dotnet test -c Release --no-build -l trx --verbosity=normal }

exec { & dotnet pack .\src\XmlTvSharp\XmlTvSharp.csproj -c Release -o $artifacts --no-build }