# Ejecutar como Administrador: clic derecho -> "Ejecutar con PowerShell" o terminal admin
#Requires -RunAsAdministrator

$ErrorActionPreference = 'Continue'

Write-Host "=== Deteniendo servicios SQL Server ===" -ForegroundColor Cyan
Get-Service | Where-Object { $_.Name -match 'SQL|MSSQL' } | ForEach-Object {
    if ($_.Status -eq 'Running') {
        Write-Host "Deteniendo $($_.Name)..."
        Stop-Service -Name $_.Name -Force -ErrorAction SilentlyContinue
    }
}
Start-Sleep -Seconds 3

Write-Host "`n=== Eliminando carpetas de SQL Server ===" -ForegroundColor Cyan
$paths = @(
    'C:\Program Files\Microsoft SQL Server',
    'C:\Program Files (x86)\Microsoft SQL Server'
)

foreach ($path in $paths) {
    if (-not (Test-Path -LiteralPath $path)) {
        Write-Host "No existe: $path" -ForegroundColor Yellow
        continue
    }

    Write-Host "Tomando propiedad de: $path"
    takeown /f $path /r /d y | Out-Null
    icacls $path /grant "${env:USERNAME}:(F)" /t /c /q | Out-Null
    icacls $path /grant 'Administrators:(F)' /t /c /q | Out-Null

    Write-Host "Borrando: $path"
    Remove-Item -LiteralPath $path -Recurse -Force -ErrorAction SilentlyContinue

    if (Test-Path -LiteralPath $path) {
        Write-Host "AVISO: Quedaron archivos en $path (puede requerir reinicio)" -ForegroundColor Red
    } else {
        Write-Host "OK: eliminado $path" -ForegroundColor Green
    }
}

Write-Host "`n=== Fin ===" -ForegroundColor Cyan
Write-Host "Reinicia el PC si quedaron archivos bloqueados, luego reinstala SQL Express con LocalDB."
Read-Host "Pulsa Enter para cerrar"
