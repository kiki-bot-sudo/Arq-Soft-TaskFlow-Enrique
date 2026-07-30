param(
    [Parameter(Mandatory = $true)]
    [ValidatePattern('^https://.+')]
    [string]$Url
)

$outputPath = Join-Path $PSScriptRoot '..\docs\taskflow-azure-qr.png'
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

& python -c $pythonCode $Url $outputPath
if ($LASTEXITCODE -ne 0) {
    throw 'No se pudo generar el código QR.'
}

Write-Host "QR generado para: $Url"
Write-Host "Archivo: $outputPath"
