FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
COPY ../ /src
WORKDIR /src/Rabbit.Consumer_1
RUN dotnet restore "Rabbit.Consumer_1.csproj"
RUN dotnet build "Rabbit.Consumer_1.csproj" -c Release -o /app/build
RUN dotnet publish "Rabbit.Consumer_1.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Rabbit.Consumer_1.dll"]
