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

Para una demostración escolar sin costo se utiliza Static Web Apps Free, App Service F1 y la oferta gratuita de Azure SQL con pausa al alcanzar el límite.

```powershell
az group create --name rg-taskflow --location centralus
az appservice plan create --name plan-taskflow --resource-group rg-taskflow --sku F1 --is-linux
az webapp create --name <nombre-unico> --resource-group rg-taskflow --plan plan-taskflow --runtime "DOTNETCORE:8.0"
az staticwebapp create --name <frontend-unico> --resource-group rg-taskflow --location centralus --sku Free
```

Al crear Azure SQL se debe confirmar `useFreeLimit=true` y `freeLimitExhaustionBehavior=AutoPause`. Si la suscripción no ofrece esas opciones, no continuar sin revisar el costo.

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
tar -a -c -f taskflow.zip -C publish .
az webapp deploy --resource-group rg-taskflow --name <nombre-unico> --src-path taskflow.zip --type zip
```

En Windows no se recomienda `Compress-Archive` para un App Service Linux porque puede guardar separadores `\` dentro del ZIP. `tar -a` produce rutas `/` compatibles con Kudu.

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

El QR debe apuntar al frontend de Static Web Apps:

```powershell
python -m pip install "qrcode[pil]"
.\scripts\generate-qr.ps1 -Url "https://<frontend>.azurestaticapps.net"
```
