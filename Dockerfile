FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

COPY Sigma.sln .

COPY src/Common/Common.Monitoring/Common.Monitoring.csproj src/Common/Common.Monitoring/
COPY src/Common/Common.Db/Common.Db.csproj src/Common/Common.Db/
COPY src/Common/Common.Caching/Common.Caching.csproj src/Common/Common.Caching/
COPY src/Common/Common.Nats/Common.Nats.csproj src/Common/Common.Nats/
COPY src/Common/Common.RabbitMq/Common.RabbitMq.csproj src/Common/Common.RabbitMq/
COPY src/Common/Common.PubSub/Common.PubSub.csproj src/Common/Common.PubSub/
COPY src/Common/Common.OpenApi/Common.OpenApi.csproj src/Common/Common.OpenApi/
COPY src/Common/Common.Api/Common.Api.csproj src/Common/Common.Api/
COPY src/Common/Common.Validation/Common.Validation.csproj src/Common/Common.Validation/

COPY src/Infrastructure/Core.Builder/Core.Builder.csproj src/Infrastructure/Core.Builder/
COPY src/Infrastructure/Core.Cache/Core.Cache.csproj src/Infrastructure/Core.Cache/
COPY src/Infrastructure/Core.Nats/Core.Nats.csproj src/Infrastructure/Core.Nats/
COPY src/Infrastructure/Core.RabbitMq/Core.RabbitMq.csproj src/Infrastructure/Core.RabbitMq/
COPY src/Infrastructure/Core.Db/Core.Db.csproj src/Infrastructure/Core.Db/
COPY src/Infrastructure/Core.Tests.Fixtures/Core.Tests.Fixtures.csproj src/Infrastructure/Core.Tests.Fixtures/

COPY src/Domain/Core.Contracts/Core.Contracts.csproj src/Domain/Core.Contracts/
COPY src/Domain/Core.Services/Core.Services.csproj src/Domain/Core.Services/
COPY src/Domain/Core.Services.Tests/Core.Services.Tests.csproj src/Domain/Core.Services.Tests/

COPY src/Apps/Core.Migrations/Core.Migrations.csproj src/Apps/Core.Migrations/
COPY src/Apps/Core.API/Core.API.csproj src/Apps/Core.API/
COPY src/Apps/Core.WS/Core.WS.csproj src/Apps/Core.WS/
COPY src/Apps/Core.Tasks/Core.Tasks.csproj src/Apps/Core.Tasks/
COPY src/Apps/Access.API/Access.API.csproj src/Apps/Access.API/
COPY src/Apps/LetsAuth.API/LetsAuth.API.csproj src/Apps/LetsAuth.API/
COPY src/Apps/Redirect.API/Redirect.API.csproj src/Apps/Redirect.API/
COPY src/Apps/Affiliate.API/Affiliate.API.csproj src/Apps/Affiliate.API/
COPY src/Apps/Domain.API/Domain.API.csproj src/Apps/Domain.API/
COPY src/Apps/Control.API/Control.API.csproj src/Apps/Control.API/
COPY src/Apps/Payment.API/Payment.API.csproj src/Apps/Payment.API/

RUN dotnet restore

COPY Directory.Build.props .
COPY ./src ./src
RUN dotnet build Sigma.sln -c Release --no-restore

FROM build AS publish

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