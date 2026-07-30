# Despliegue de TaskFlow en Microsoft Azure

## 1. Requisitos

- Suscripción de Azure.
- Azure CLI o Visual Studio 2022.
- .NET 8 SDK.
- Repositorio en GitHub si se utilizará el workflow.

## 2. Validación Release

```powershell
dotnet restore TaskFlow.sln
dotnet build TaskFlow.sln --configuration Release --no-restore
dotnet test TaskFlow.Tests --configuration Release --no-build
```

## 3. Recursos

Crear un grupo de recursos, Azure SQL Server, Azure SQL Database y App Service para .NET 8. Un plan básico es suficiente para una demostración escolar.

```powershell
az group create --name rg-taskflow --location centralus
az appservice plan create --name plan-taskflow --resource-group rg-taskflow --sku B1 --is-linux
az webapp create --name <nombre-unico> --resource-group rg-taskflow --plan plan-taskflow --runtime "DOTNETCORE:8.0"
```

Azure SQL puede crearse desde el portal para evitar exponer contraseñas en el historial de terminal.

## 4. Configuración

En App Service → Environment variables:

- `ASPNETCORE_ENVIRONMENT=Production`
- `ConnectionStrings__DefaultConnection=<cadena de Azure SQL>`

No guardar la cadena en Git. Permitir en el firewall de Azure SQL las conexiones provenientes de servicios Azure.

## 5. Migraciones

Aplicar desde un equipo autorizado antes del primer inicio:

```powershell
dotnet ef database update --project TaskFlow.Infrastructure --startup-project TaskFlow.Api --connection "<cadena-azure-sql>"
```

La migración crea Identity, la relación de usuario y subtareas. Las tareas antiguas mantienen `UserId = null` y no se muestran a ninguna cuenta.

## 6. Publicación

### Azure CLI

```powershell
dotnet publish TaskFlow.Api -c Release -o publish
Compress-Archive -Path publish\* -DestinationPath taskflow.zip -Force
az webapp deploy --resource-group rg-taskflow --name <nombre-unico> --src-path taskflow.zip --type zip
```

### Visual Studio

Publicar `TaskFlow.Api` mediante **Publish → Azure → Azure App Service**. Confirmar que no se incluyan secretos en el perfil.

### GitHub Actions

Configurar:

- Variable `AZURE_WEBAPP_NAME`.
- Secreto `AZURE_WEBAPP_PUBLISH_PROFILE`.

Ejecutar manualmente `.github/workflows/azure-deploy.yml`.

## 7. Verificación

1. Abrir `https://<nombre>.azurewebsites.net/health`.
2. Registrar dos cuentas de prueba.
3. Crear una tarea y subtareas con la primera.
4. Confirmar que la segunda cuenta no las ve.
5. Revisar **Log stream** en App Service si ocurre un error.

## 8. URL y QR

La URL será `https://<nombre-real>.azurewebsites.net`. Cuando exista:

```powershell
python -m pip install "qrcode[pil]"
.\scripts\generate-qr.ps1 -Url "https://<nombre-real>.azurewebsites.net"
```

No se genera un QR hasta conocer y verificar la URL pública real.
