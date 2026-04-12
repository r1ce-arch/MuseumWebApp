FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY web_application_museum/web_application_museum.csproj ./web_application_museum/
RUN dotnet restore ./web_application_museum/web_application_museum.csproj

COPY web_application_museum/ ./web_application_museum/
RUN dotnet publish ./web_application_museum/web_application_museum.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

RUN mkdir -p wwwroot/uploads/exhibits wwwroot/images

EXPOSE 8080
ENTRYPOINT ["dotnet", "web_application_museum.dll"]
