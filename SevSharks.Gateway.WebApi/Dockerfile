FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 52381
EXPOSE 44319

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY IT2.Gateway.WebApi/IT2.Gateway.WebApi.csproj IT2.Gateway.WebApi/
COPY IT2.Gateway.WebApi.Models/IT2.Gateway.WebApi.Models.csproj IT2.Gateway.WebApi.Models/
COPY ./NuGet.Config .
RUN dotnet restore IT2.Gateway.WebApi/IT2.Gateway.WebApi.csproj
COPY . .
WORKDIR /src/IT2.Gateway.WebApi
RUN dotnet build IT2.Gateway.WebApi.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish IT2.Gateway.WebApi.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "IT2.Gateway.WebApi.dll"]
