FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

COPY *.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Install CA certificates to trust SSL certs (fix pre-login handshake error)
RUN apt-get update && apt-get install -y ca-certificates && update-ca-certificates

COPY --from=build-env /app/out .

ENTRYPOINT [ "dotnet", "PlatformService.dll" ]