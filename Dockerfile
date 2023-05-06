FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /build
COPY . .

RUN dotnet restore "BlazorSignalRBackplaneTest.sln"
RUN dotnet publish "BlazorSignalRBackplaneTest/BlazorSignalRBackplaneTest.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:7.0-alpine AS release
EXPOSE 80
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "BlazorSignalRBackplaneTest.dll"]