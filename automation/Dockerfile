FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["automation/automation.csproj", "automation/"]
RUN dotnet restore "automation/automation.csproj"
COPY . .
WORKDIR "/src/automation"
RUN dotnet build "automation.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "automation.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "automation.dll"]