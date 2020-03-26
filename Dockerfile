#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster
WORKDIR /src
COPY ["QtMirrorsServer.csproj", "/src"]
RUN dotnet restore "QtMirrorsServer.csproj"
COPY . .
RUN dotnet build "QtMirrorsServer.csproj" -c Release -o /app
WORKDIR /app
EXPOSE 80
ENV ASPNETCORE_URLS=http://0.0.0.0:80
ENTRYPOINT ["dotnet", "QtMirrorsServer.dll"]