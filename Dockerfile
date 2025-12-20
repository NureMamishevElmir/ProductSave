# build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY *.sln ./
COPY Web.Api/Web.Api.csproj Web.Api/
COPY Infrastructure/Infrastructure.csproj Infrastructure/
COPY DomainEntity/DomainEntity.csproj DomainEntity/
COPY Service/Service.csproj Service/

RUN dotnet restore Web.Api/Web.Api.csproj

COPY . .
WORKDIR /src/Web.Api
RUN dotnet publish Web.Api.csproj -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish ./
ENTRYPOINT ["dotnet", "Web.Api.dll"]
