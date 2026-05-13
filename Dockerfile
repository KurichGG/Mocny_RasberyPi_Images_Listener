FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["Mocny_RasberyPi_Images_Listener.csproj", ""]
RUN dotnet restore "Mocny_RasberyPi_Images_Listener.csproj"

COPY . .
RUN dotnet build "Mocny_RasberyPi_Images_Listener.csproj" -c Release -o /app/build

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/build .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "Mocny_RasberyPi_Images_Listener.dll"]