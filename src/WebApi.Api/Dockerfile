FROM microsoft/dotnet:2.1-aspnetcore-runtime-nanoserver-1709 AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk-nanoserver-1709 AS build
WORKDIR /src
COPY WebApi.Api/WebApi.Api.csproj WebApi.Api/
RUN dotnet restore WebApi.Api/WebApi.Api.csproj
COPY . .
WORKDIR /src/WebApi.Api
RUN dotnet build WebApi.Api.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish WebApi.Api.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "WebApi.Api.dll"]
