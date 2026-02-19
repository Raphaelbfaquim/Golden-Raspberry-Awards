# Adiciona o diretório do dotnet ao PATH da sessão atual (PowerShell).
# Ajuste o caminho abaixo se a sua instalação for em outro local.
$dotnetPath = "C:\Program Files\dotnet"
if (Test-Path "$dotnetPath\dotnet.exe") {
    $env:PATH = "$dotnetPath;$env:PATH"
    Write-Host "PATH atualizado. Teste: dotnet --version"
    & "$dotnetPath\dotnet.exe" --version
} else {
    Write-Host "dotnet nao encontrado em: $dotnetPath"
    Write-Host "Instale o .NET 8 SDK em: https://dotnet.microsoft.com/download/dotnet/8.0"
}
