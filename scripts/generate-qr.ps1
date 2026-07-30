param(
    [Parameter(Mandatory = $true)]
    [ValidatePattern('^https://.+')]
    [string]$Url
)

$outputPath = Join-Path $PSScriptRoot '..\docs\taskflow-azure-qr.png'
$python = Get-Command python -ErrorAction SilentlyContinue
if (-not $python) {
    throw 'No se encontró Python. Instálalo y ejecuta: python -m pip install "qrcode[pil]"'
}

$pythonCode = @'
import sys
try:
    import qrcode
except ImportError:
    print("Falta la dependencia. Ejecuta: python -m pip install 'qrcode[pil]'", file=sys.stderr)
    sys.exit(2)
image = qrcode.make(sys.argv[1])
image.save(sys.argv[2])
print(sys.argv[2])
'@

# PowerShell 5 puede alterar las comillas del código enviado con `python -c`.
$temporaryScript = Join-Path ([System.IO.Path]::GetTempPath()) "taskflow-generate-qr-$([Guid]::NewGuid().ToString('N')).py"
try {
    Set-Content -LiteralPath $temporaryScript -Value $pythonCode -Encoding UTF8
    & $python.Source $temporaryScript $Url $outputPath
    if ($LASTEXITCODE -ne 0) {
        throw 'No se pudo generar el código QR.'
    }
}
finally {
    Remove-Item -LiteralPath $temporaryScript -Force -ErrorAction SilentlyContinue
}

Write-Host "QR generado para: $Url"
Write-Host "Archivo: $outputPath"
