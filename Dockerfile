FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

COPY Sigma.sln .

COPY src/Common/Common.Monitoring/Common.Monitoring.csproj src/Common/Common.Monitoring/
COPY src/Common/Common.Db/Common.Db.csproj src/Common/Common.Db/
COPY src/Common/Common.Caching/Common.Caching.csproj src/Common/Common.Caching/
COPY src/Common/Common.Nats/Common.Nats.csproj src/Common/Common.Nats/
COPY src/Common/Common.PubSub/Common.PubSub.csproj src/Common/Common.PubSub/
COPY src/Common/Common.OpenApi/Common.OpenApi.csproj src/Common/Common.OpenApi/
COPY src/Common/Common.Api/Common.Api.csproj src/Common/Common.Api/
COPY src/Common/Common.Validation/Common.Validation.csproj src/Common/Common.Validation/

COPY src/Infrastructure/Core.Builder/Core.Builder.csproj src/Infrastructure/Core.Builder/
COPY src/Infrastructure/Core.Cache/Core.Cache.csproj src/Infrastructure/Core.Cache/
COPY src/Infrastructure/Core.Nats/Core.Nats.csproj src/Infrastructure/Core.Nats/
COPY src/Infrastructure/Core.Db/Core.Db.csproj src/Infrastructure/Core.Db/
COPY src/Infrastructure/Core.Migrations/Core.Migrations.csproj src/Infrastructure/Core.Migrations/

COPY src/Domain/Core.Contracts/Core.Contracts.csproj src/Domain/Core.Contracts/
COPY src/Domain/Core.Services/Core.Services.csproj src/Domain/Core.Services/

COPY src/Apps/Core.API/Core.API.csproj src/Apps/Core.API/
COPY src/Apps/Core.WS/Core.WS.csproj src/Apps/Core.WS/
COPY src/Apps/Access.API/Access.API.csproj src/Apps/Access.API/
COPY src/Apps/LetsAuth.API/LetsAuth.API.csproj src/Apps/LetsAuth.API/
COPY src/Apps/Redirect.API/Redirect.API.csproj src/Apps/Redirect.API/
COPY src/Apps/Affiliate.API/Affiliate.API.csproj src/Apps/Affiliate.API/
COPY src/Apps/Domain.API/Domain.API.csproj src/Apps/Domain.API/

RUN dotnet restore
RUN dotnet build Sigma.sln --no-restore

FROM build AS publish
COPY . .

ARG APP_NAME
ENV APP_PROJECT=src/Apps/$APP_NAME/$APP_NAME.csproj
RUN dotnet publish $APP_PROJECT -c Release -o /app --no-build

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
ARG APP_NAME
WORKDIR /app
COPY --from=publish /app ./

RUN mv $APP_NAME.dll app.dll && \
    mv $APP_NAME.deps.json app.deps.json && \
    mv $APP_NAME.runtimeconfig.json app.runtimeconfig.json

ENTRYPOINT ["dotnet", "app.dll"]