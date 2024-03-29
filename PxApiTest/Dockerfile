#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
ENV ASPNETCORE_URLS http://+:8000;https://+:8443
EXPOSE 8000
EXPOSE 8443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["PxApiTest/PxApiTest.csproj", "PxApiTest/"]
COPY ["DbSource/DbSource.csproj", "DbSource/"]
COPY ["PxDatasource/PxDatasource.csproj", "PxDatasource/"]
RUN dotnet restore "PxApiTest/PxApiTest.csproj"
COPY . .
WORKDIR "/src/PxApiTest"
RUN dotnet build "PxApiTest.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PxApiTest.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PxApiTest.dll"]