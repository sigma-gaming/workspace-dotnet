FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

COPY Sigma.sln .

COPY src/Common/Common.Logging/Common.Logging.csproj src/Common/Common.Logging/
COPY src/Common/Common.Db/Common.Db.csproj src/Common/Common.Db/
COPY src/Common/Common.Caching/Common.Caching.csproj src/Common/Common.Caching/
COPY src/Common/Common.PubSub/Common.PubSub.csproj src/Common/Common.PubSub/
COPY src/Common/Common.OpenApi/Common.OpenApi.csproj src/Common/Common.OpenApi/
COPY src/Common/Common.Validation/Common.Validation.csproj src/Common/Common.Validation/

COPY src/Infrastructure/Core.Builder/Core.Builder.csproj src/Infrastructure/Core.Builder/
COPY src/Infrastructure/Core.Infrastructure/Core.Infrastructure.csproj src/Infrastructure/Core.Infrastructure/
COPY src/Infrastructure/Core.Cache/Core.Cache.csproj src/Infrastructure/Core.Cache/
COPY src/Infrastructure/Core.PubSub/Core.PubSub.csproj src/Infrastructure/Core.PubSub/
COPY src/Infrastructure/Core.Db/Core.Db.csproj src/Infrastructure/Core.Db/
COPY src/Infrastructure/Core.Migrations/Core.Migrations.csproj src/Infrastructure/Core.Migrations/

COPY src/Domain/Core.Contracts/Core.Contracts.csproj src/Domain/Core.Contracts/
COPY src/Domain/Core.Services/Core.Services.csproj src/Domain/Core.Services/

COPY src/Apps/Core.API/Core.API.csproj src/Apps/Core.API/

RUN dotnet restore
COPY . .

ARG APP_NAME
ENV APP_PROJECT=src/Apps/$APP_NAME/$APP_NAME.csproj
RUN dotnet publish $APP_PROJECT -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
ARG APP_NAME
ENV APP_NAME_EXE=$APP_NAME.dll
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT dotnet ${APP_NAME_EXE}