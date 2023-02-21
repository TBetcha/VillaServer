FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["VillaAPI/VillaAPI.csproj", "VillaAPI/"]
RUN dotnet restore "VillaAPI/VillaAPI.csproj"
COPY . .
WORKDIR "/src/VillaAPI"
RUN dotnet build "VillaAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "VillaAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VillaAPI.dll"]
