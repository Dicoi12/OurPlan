# Dockerfile pentru backend .NET 9.0
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiem fișierul proiectului și restauram dependențele
COPY ["OurPlan.csproj", "./"]
RUN dotnet restore "OurPlan.csproj"

# Copiem restul codului sursă
COPY . .
WORKDIR "/src"
RUN dotnet build "OurPlan.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OurPlan.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Setăm variabilele de mediu
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "OurPlan.dll"]


