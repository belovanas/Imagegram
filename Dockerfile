FROM mcr.microsoft.com/dotnet/sdk:6.0 as base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 as build
WORKDIR Imagegram/src
COPY ["Imagegram Bandlab.sln", "./"]
COPY . .

RUN dotnet restore
COPY . .
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app -f net6.0

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Endpoint.dll"]
