#multi-etapas
# --- ETAPA 1: COMPILACIÓN (SDK) --- 
# Esta es la parte "pesada" que tiene los compiladores
# Usar la imagen oficial del SDK de .NET para construir la aplicación
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# copiar los archivos del proyecto y restaurar las dependencias 
COPY *.csproj ./
RUN dotnet restore

# Copiar los archivos restantes y compilar la aplicacion
COPY . ./
RUN dotnet publish -c Release -o out

# --- ETAPA 2: EJECUCIÓN (RUNTIME) --- 
# Esta es la parte "liviana" para que la app corra rápido y segura
# Usar la imagen de runtime para ejecutar la aplicación 
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./

# Exponer el puerto y definir el punto de entrada
EXPOSE 80
ENTRYPOINT ["dotnet", "ProyectoFinal-ISII.dll"]