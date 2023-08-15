FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/OauthShowcase/OauthShowcase.csproj", "src/OauthShowcase/"]
RUN dotnet restore "src/OauthShowcase/OauthShowcase.csproj"
COPY . .
WORKDIR "/src/src/OauthShowcase"
RUN dotnet build "OauthShowcase.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OauthShowcase.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OauthShowcase.dll"]