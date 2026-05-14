FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
RUN apt-get update && apt-get install -y ffmpeg && rm -rf /var/lib/apt/lists/*

WORKDIR /src

COPY VideoOrders.sln ./
COPY ProcessingSystem/Gateway/Gateway.csproj ProcessingSystem/Gateway/
ENV ArchiveStorage__RootPath=/app/storage/archive-results
COPY ProcessingSystem/Gateway/appsettings*.json /app/
COPY BuildingBlocks/Events.Abstractions/Events.Abstractions.csproj BuildingBlocks/Events.Abstractions/

RUN dotnet restore ProcessingSystem/Gateway/Gateway.csproj

COPY . .

WORKDIR /src/ProcessingSystem/Gateway
RUN dotnet publish Gateway.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
RUN apt-get update && apt-get install -y ffmpeg && rm -rf /var/lib/apt/lists/*
WORKDIR /app
COPY --from=build /app/publish .

ENV RabbitMQ__Host=orders-rabbitmq
ENV RabbitMQ__VirtualHost=localhost
ENV RabbitMQ__User=guest
ENV RabbitMQ__Password=guest

ENTRYPOINT ["dotnet", "Gateway.dll"]