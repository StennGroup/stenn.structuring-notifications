FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM stenn.azurecr.io/stenn-dotnet-sdk:6.0 AS build
WORKDIR /src
COPY src .
WORKDIR "/src/StructuringNotifications.WebApp"
RUN dotnet restore "StructuringNotifications.WebApp.csproj"
RUN dotnet build "StructuringNotifications.WebApp.csproj" -c Release --no-restore

FROM build AS publish
WORKDIR "/src/StructuringNotifications.WebApp"
RUN dotnet publish "StructuringNotifications.WebApp.csproj" -c Release --no-restore --no-build -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StructuringNotifications.WebApp.dll"]
